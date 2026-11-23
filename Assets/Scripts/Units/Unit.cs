using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Grid Position")]
    public int gridX;
    public int gridZ;

    [Header("Movement")]
    public int movementRange = 3;

    public void SetPosition(int x, int z)
    {
        gridX = x;
        gridZ = z;
    }

    public bool CanMoveTo(int targetX, int targetZ)
    {
        int distance = Mathf.Abs(targetX - gridX)
        + Mathf.Abs(targetZ - gridZ);

        return distance <= movementRange;
    }
}