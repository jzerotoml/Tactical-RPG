using System;
using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public int width = 8;
    public int height = 8;
    public GameObject tilePrefab;

    [Header("Unit Prefabs")]
    [SerializeField] private List<UnitPrefabConfiguration> unitPrefabs =
        new List<UnitPrefabConfiguration>();

    [Header("Unit Spawn Configuration")]
    [SerializeField] private List<UnitSpawnDefinition> unitSpawns =
        new List<UnitSpawnDefinition>();
    [SerializeField] private float unitSpawnHeight = 0.5f;

    [Header("Movement Overlay")]
    [SerializeField] private GameObject movementOverlayPrefab;

    private List<GameObject> movementOverlays = new List<GameObject>();
    private Unit[,] occupiedTiles;
    private static readonly Vector2Int[] MovementDirections =
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };

    [Serializable]
    private class UnitPrefabConfiguration
    {
        [SerializeField] private UnitClass unitClass;
        [SerializeField] private GameObject prefab;

        public UnitClass Class => unitClass;
        public GameObject Prefab => prefab;
    }

    [Serializable]
    private class UnitSpawnDefinition
    {
        [SerializeField] private Team team;
        [SerializeField] private UnitClass unitClass;
        [SerializeField] private int x;
        [SerializeField] private int z;

        public Team Team => team;
        public UnitClass Class => unitClass;
        public int X => x;
        public int Z => z;
    }

    private void Start()
    {
        occupiedTiles = new Unit[width, height];
        GenerateGrid();
        SpawnConfiguredUnits();
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(x, 0, z);

                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                tile.name = $"Tile_{x}_{z}";
            }
        }
    }

    private void SpawnConfiguredUnits()
    {
        if (!TryBuildPrefabMap(out Dictionary<UnitClass, GameObject> prefabByClass))
            return;

        if (!ValidateSpawnDefinitions(prefabByClass))
            return;

        for (int i = 0; i < unitSpawns.Count; i++)
        {
            UnitSpawnDefinition definition = unitSpawns[i];
            GameObject prefab = prefabByClass[definition.Class];
            Vector3 position = new Vector3(
                definition.X,
                unitSpawnHeight,
                definition.Z
            );

            GameObject unitObject = Instantiate(prefab, position, Quaternion.identity);
            Unit unit = unitObject.GetComponent<Unit>();
            unitObject.name = $"{definition.Class}_{definition.Team}_{i + 1}";
            unit.SetTeam(definition.Team);
            unit.SetPosition(definition.X, definition.Z);
            occupiedTiles[definition.X, definition.Z] = unit;
        }
    }

    private bool TryBuildPrefabMap(
        out Dictionary<UnitClass, GameObject> prefabByClass
    )
    {
        prefabByClass = new Dictionary<UnitClass, GameObject>();
        bool isValid = true;

        for (int i = 0; i < unitPrefabs.Count; i++)
        {
            UnitPrefabConfiguration configuration = unitPrefabs[i];

            if (configuration == null)
            {
                Debug.LogError($"Unit prefab configuration at index {i} is empty.", this);
                isValid = false;
                continue;
            }

            if (!Enum.IsDefined(typeof(UnitClass), configuration.Class))
            {
                Debug.LogError(
                    $"Unit prefab configuration at index {i} has an invalid class.",
                    this
                );
                isValid = false;
                continue;
            }

            if (configuration.Prefab == null)
            {
                Debug.LogError(
                    $"No prefab is assigned for {configuration.Class}.",
                    this
                );
                isValid = false;
                continue;
            }

            if (prefabByClass.ContainsKey(configuration.Class))
            {
                Debug.LogError(
                    $"More than one unit prefab is assigned for {configuration.Class}.",
                    this
                );
                isValid = false;
                continue;
            }

            Unit prefabUnit = configuration.Prefab.GetComponent<Unit>();

            if (prefabUnit == null)
            {
                Debug.LogError(
                    $"Prefab '{configuration.Prefab.name}' requires a Unit component on its root.",
                    configuration.Prefab
                );
                isValid = false;
                continue;
            }

            if (prefabUnit.Data == null)
            {
                Debug.LogError(
                    $"Unit prefab '{configuration.Prefab.name}' requires a UnitData asset.",
                    configuration.Prefab
                );
                isValid = false;
                continue;
            }

            if (prefabUnit.Data.Class != configuration.Class)
            {
                Debug.LogError(
                    $"Prefab '{configuration.Prefab.name}' uses {prefabUnit.Data.Class} data, " +
                    $"but is mapped as {configuration.Class}.",
                    configuration.Prefab
                );
                isValid = false;
                continue;
            }

            prefabByClass.Add(configuration.Class, configuration.Prefab);
        }

        return isValid;
    }

    private bool ValidateSpawnDefinitions(
        Dictionary<UnitClass, GameObject> prefabByClass
    )
    {
        if (unitSpawns.Count == 0)
        {
            Debug.LogWarning("No unit spawn definitions are configured.", this);
            return true;
        }

        bool isValid = true;
        HashSet<Vector2Int> configuredPositions = new HashSet<Vector2Int>();

        for (int i = 0; i < unitSpawns.Count; i++)
        {
            UnitSpawnDefinition definition = unitSpawns[i];

            if (definition == null)
            {
                Debug.LogError($"Unit spawn definition at index {i} is empty.", this);
                isValid = false;
                continue;
            }

            if (!Enum.IsDefined(typeof(Team), definition.Team))
            {
                Debug.LogError(
                    $"Unit spawn definition at index {i} has an invalid team.",
                    this
                );
                isValid = false;
            }

            if (!Enum.IsDefined(typeof(UnitClass), definition.Class))
            {
                Debug.LogError(
                    $"Unit spawn definition at index {i} has an invalid class.",
                    this
                );
                isValid = false;
            }
            else if (!prefabByClass.ContainsKey(definition.Class))
            {
                Debug.LogError(
                    $"No valid prefab is configured for {definition.Class}.",
                    this
                );
                isValid = false;
            }

            if (definition.X < 0 || definition.X >= width ||
                definition.Z < 0 || definition.Z >= height)
            {
                Debug.LogError(
                    $"Unit spawn definition at index {i} is outside the {width}x{height} board " +
                    $"at ({definition.X}, {definition.Z}).",
                    this
                );
                isValid = false;
                continue;
            }

            Vector2Int position = new Vector2Int(definition.X, definition.Z);

            if (IsTileOccupied(definition.X, definition.Z) ||
                !configuredPositions.Add(position))
            {
                Debug.LogError(
                    $"Unit spawn definition at index {i} overlaps an occupied tile at " +
                    $"({definition.X}, {definition.Z}).",
                    this
                );
                isValid = false;
            }
        }

        return isValid;
    }

    public bool IsTileOccupied(int x, int z)
    {
        if (occupiedTiles == null || !IsWithinBoard(x, z))
            return false;

        return occupiedTiles[x, z] != null;
    }

    public Unit GetUnitAt(int x, int z)
    {
        if (occupiedTiles == null || !IsWithinBoard(x, z))
            return null;

        return occupiedTiles[x, z];
    }

    public bool MoveUnit(Unit unit, int targetX, int targetZ)
    {
        if (unit == null || occupiedTiles == null || unit.IsMoving)
            return false;

        if (!IsWithinBoard(targetX, targetZ) || IsTileOccupied(targetX, targetZ))
            return false;

        if (!IsWithinBoard(unit.gridX, unit.gridZ) ||
            GetUnitAt(unit.gridX, unit.gridZ) != unit)
            return false;

        Dictionary<Vector2Int, List<Vector2Int>> reachablePaths =
            GetReachablePaths(unit);
        Vector2Int destination = new Vector2Int(targetX, targetZ);

        if (!reachablePaths.TryGetValue(destination, out List<Vector2Int> path))
            return false;

        occupiedTiles[targetX, targetZ] = unit;
        unit.MovementCompleted += HandleMovementCompleted;

        if (unit.MoveAlongPath(path))
            return true;

        unit.MovementCompleted -= HandleMovementCompleted;
        occupiedTiles[targetX, targetZ] = null;
        return false;
    }

    public void ShowMovementRange(Unit unit)
    {
        ClearMovementRange();

        if (unit == null || movementOverlayPrefab == null)
            return;

        Dictionary<Vector2Int, List<Vector2Int>> reachablePaths =
            GetReachablePaths(unit);

        foreach (Vector2Int destination in reachablePaths.Keys)
        {
            Vector3 position = new Vector3(destination.x, 0.12f, destination.y);
            GameObject overlay = Instantiate(
                movementOverlayPrefab,
                position,
                Quaternion.identity,
                transform
            );
            movementOverlays.Add(overlay);
        }
    }

    private Dictionary<Vector2Int, List<Vector2Int>> GetReachablePaths(Unit unit)
    {
        Dictionary<Vector2Int, List<Vector2Int>> reachablePaths =
            new Dictionary<Vector2Int, List<Vector2Int>>();

        if (unit == null || unit.Data == null || occupiedTiles == null ||
            !IsWithinBoard(unit.gridX, unit.gridZ) ||
            GetUnitAt(unit.gridX, unit.gridZ) != unit ||
            unit.Data.MovementRange <= 0)
        {
            return reachablePaths;
        }

        int[,] distances = new int[width, height];
        Vector2Int[,] predecessors = new Vector2Int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                distances[x, z] = -1;
            }
        }

        Vector2Int origin = new Vector2Int(unit.gridX, unit.gridZ);
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        distances[origin.x, origin.y] = 0;
        frontier.Enqueue(origin);

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();
            int currentDistance = distances[current.x, current.y];

            if (currentDistance >= unit.Data.MovementRange)
                continue;

            foreach (Vector2Int direction in MovementDirections)
            {
                Vector2Int next = current + direction;

                if (!IsWithinBoard(next.x, next.y) ||
                    distances[next.x, next.y] >= 0)
                {
                    continue;
                }

                Unit occupant = occupiedTiles[next.x, next.y];

                if (occupant != null && occupant.Team != unit.Team)
                    continue;

                distances[next.x, next.y] = currentDistance + 1;
                predecessors[next.x, next.y] = current;
                frontier.Enqueue(next);

                if (occupant == null)
                {
                    reachablePaths.Add(
                        next,
                        ReconstructPath(origin, next, predecessors)
                    );
                }
            }
        }

        return reachablePaths;
    }

    private List<Vector2Int> ReconstructPath(
        Vector2Int origin,
        Vector2Int destination,
        Vector2Int[,] predecessors
    )
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = destination;

        while (current != origin)
        {
            path.Add(current);
            current = predecessors[current.x, current.y];
        }

        path.Reverse();
        return path;
    }

    private void HandleMovementCompleted(Unit unit, Vector2Int origin)
    {
        if (unit == null)
            return;

        unit.MovementCompleted -= HandleMovementCompleted;

        if (!IsWithinBoard(origin.x, origin.y) ||
            !IsWithinBoard(unit.gridX, unit.gridZ))
        {
            Debug.LogError(
                $"Unit '{unit.name}' completed movement outside the board.",
                unit
            );
            return;
        }

        if (occupiedTiles[origin.x, origin.y] == unit)
            occupiedTiles[origin.x, origin.y] = null;

        occupiedTiles[unit.gridX, unit.gridZ] = unit;
    }

    private bool IsWithinBoard(int x, int z)
    {
        return x >= 0 && x < width && z >= 0 && z < height;
    }



    public void ClearMovementRange()
    {
        foreach (GameObject overlay in movementOverlays)
        {
            if (overlay != null)
                Destroy(overlay);
        }
        movementOverlays.Clear();
    }

}
