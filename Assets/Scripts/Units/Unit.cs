using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Grid Position")]
    public int gridX;
    public int gridZ;

    [Header("Movement")]
    public int movementRange = 3;
    [SerializeField] private float movementSpeed = 5f;

private Vector3 targetPosition;

public bool IsMoving { get; private set; }

void Awake()
{
    targetPosition = transform.position;
}

void Update()
{
    if (!IsMoving)
        return;

    transform.position = Vector3.MoveTowards(
        transform.position,
        targetPosition,
        movementSpeed * Time.deltaTime
    );

    if (transform.position == targetPosition)
    {
        IsMoving = false;
    }
}

public void SetPosition(int x, int z)
{
    gridX = x;
    gridZ = z;

    targetPosition = new Vector3(
        x,
        transform.position.y,
        z
    );

    IsMoving = transform.position != targetPosition;
}

    public bool CanMoveTo(int targetX, int targetZ)
    {
        int distance = Mathf.Abs(targetX - gridX)
        + Mathf.Abs(targetZ - gridZ);

        return distance <= movementRange;
    }
}