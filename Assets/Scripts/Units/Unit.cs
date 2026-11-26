using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Unit : MonoBehaviour
{
    [Header("Unit Data")]
    [SerializeField] private UnitData unitData;

    [Header("Team")]
    [SerializeField] private Team team;
    [SerializeField] private Shader teamOutlineShader;
    [SerializeField, Min(1.001f)] private float teamOutlineScale = 1.04f;

    [Header("Grid Position")]
    public int gridX;
    public int gridZ;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5f;

    private Vector3 targetPosition;
    private Material teamOutlineMaterial;
    private readonly Queue<Vector2Int> movementPath = new Queue<Vector2Int>();
    private Vector2Int movementDestination;

    public bool IsMoving { get; private set; }
    public UnitData Data => unitData;
    public Team Team => team;
    public int CurrentHealth { get; private set; }
    internal event Action<Unit, Vector2Int> MovementCompleted;

    private void Awake()
    {
        targetPosition = transform.position;

        if (unitData == null)
        {
            Debug.LogError($"Unit '{name}' requires a UnitData asset.", this);
        }
        else
        {
            CurrentHealth = unitData.MaxHealth;
        }

        CreateTeamOutline();
        ApplyTeamColor();
    }

    private void Update()
    {
        if (!IsMoving)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            movementSpeed * Time.deltaTime
        );

        if (transform.position != targetPosition)
            return;

        if (movementPath.Count > 0)
        {
            SetNextMovementTarget();
            return;
        }

        CompleteMovement();
    }

    public void SetPosition(int x, int z)
    {
        movementPath.Clear();
        gridX = x;
        gridZ = z;

        targetPosition = new Vector3(
            x,
            transform.position.y,
            z
        );

        IsMoving = transform.position != targetPosition;
    }

    internal bool MoveAlongPath(IReadOnlyList<Vector2Int> path)
    {
        if (IsMoving || path == null || path.Count == 0)
            return false;

        movementPath.Clear();

        for (int i = 0; i < path.Count; i++)
        {
            movementPath.Enqueue(path[i]);
        }

        movementDestination = path[path.Count - 1];
        SetNextMovementTarget();
        IsMoving = true;
        return true;
    }

    private void SetNextMovementTarget()
    {
        Vector2Int nextTile = movementPath.Dequeue();
        targetPosition = new Vector3(
            nextTile.x,
            transform.position.y,
            nextTile.y
        );
    }

    private void CompleteMovement()
    {
        Vector2Int origin = new Vector2Int(gridX, gridZ);
        gridX = movementDestination.x;
        gridZ = movementDestination.y;
        IsMoving = false;
        MovementCompleted?.Invoke(this, origin);
    }

    public void SetTeam(Team newTeam)
    {
        team = newTeam;
        ApplyTeamColor();
    }

    private void CreateTeamOutline()
    {
        if (teamOutlineShader == null)
        {
            Debug.LogError($"Unit '{name}' requires a team outline shader.", this);
            return;
        }

        teamOutlineMaterial = new Material(teamOutlineShader)
        {
            name = $"{name} Team Outline (Runtime)"
        };
        teamOutlineMaterial.SetFloat(
            "_OutlineScale",
            Mathf.Max(1.001f, teamOutlineScale)
        );

        int outlineCount = 0;
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer sourceRenderer in meshRenderers)
        {
            if (!sourceRenderer.enabled)
                continue;

            MeshFilter sourceMeshFilter = sourceRenderer.GetComponent<MeshFilter>();

            if (sourceMeshFilter == null || sourceMeshFilter.sharedMesh == null)
                continue;

            GameObject outlineObject = new GameObject(
                $"{sourceRenderer.gameObject.name}_TeamOutline"
            );
            outlineObject.layer = sourceRenderer.gameObject.layer;
            outlineObject.transform.SetParent(sourceRenderer.transform, false);

            MeshFilter outlineMeshFilter = outlineObject.AddComponent<MeshFilter>();
            outlineMeshFilter.sharedMesh = sourceMeshFilter.sharedMesh;

            MeshRenderer outlineRenderer = outlineObject.AddComponent<MeshRenderer>();
            outlineRenderer.sharedMaterial = teamOutlineMaterial;
            outlineRenderer.shadowCastingMode = ShadowCastingMode.Off;
            outlineRenderer.receiveShadows = false;
            outlineRenderer.lightProbeUsage = LightProbeUsage.Off;
            outlineRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;

            outlineCount++;
        }

        if (outlineCount == 0)
        {
            Debug.LogError(
                $"Unit '{name}' requires an enabled MeshRenderer with a MeshFilter for its team outline.",
                this
            );
        }
    }

    private void ApplyTeamColor()
    {
        if (teamOutlineMaterial == null)
            return;

        Color teamColor = team == Team.Blue ? Color.blue : Color.red;
        teamOutlineMaterial.SetColor("_OutlineColor", teamColor);
    }

    private void OnDestroy()
    {
        if (teamOutlineMaterial != null)
            Destroy(teamOutlineMaterial);
    }

}