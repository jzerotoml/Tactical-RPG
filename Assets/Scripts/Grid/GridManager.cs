using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public int width = 8;
    public int height = 8;
    public GameObject tilePrefab;
    [SerializeField] private GameObject unitPrefab;
    [Header("Movement Overlay")]
    [SerializeField] private GameObject movementOverlayPrefab;

    private List<GameObject> movementOverlays = new List<GameObject>();
    private Unit[,] occupiedTiles;
void Start()
    {
        occupiedTiles = new Unit[width, height];
        GenerateGrid();
        SpawnUnit();
    }

    void GenerateGrid()
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
    void SpawnUnit()
    {
        int x = 2;
        int z = 2;
        Vector3 position = new Vector3(x, 0.5f, z);
        GameObject unit = Instantiate(
            unitPrefab,
            position,
            Quaternion.identity
        );
        unit.name = "Unit_Player_01";
        Unit unitScript = unit.GetComponent<Unit>();
        unitScript.SetPosition(x, z);
        occupiedTiles[x, z] = unitScript;
    }
    public bool IsTileOccupied(int x, int z)
{
    if (x < 0 || x >= width || z < 0 || z >= height)
    {
        return false;
    }
    return occupiedTiles[x, z] != null;
}

    public Unit GetUnitAt(int x, int z)
    {
        if (x < 0 || x >= width || z < 0 || z >= height)
        {
            return null;
        }
        return occupiedTiles[x, z];
        
    }
        public bool MoveUnit(Unit unit, int targetX, int targetZ)
    {
        if (unit == null)
            return false;

        // Validate grid boundaries.
        if (targetX < 0 || targetX >= width ||
            targetZ < 0 || targetZ >= height)
            return false;

        // Validate destination occupancy.
        if (IsTileOccupied(targetX, targetZ))
            return false;

        // Validate movement range.
        if (!unit.CanMoveTo(targetX, targetZ))
            return false;

        // Clear previous tile.
        occupiedTiles[unit.gridX, unit.gridZ] = null;

        // Update unit position.
        unit.SetPosition(targetX, targetZ);

        // Register new occupied tile.
        occupiedTiles[targetX, targetZ] = unit;

        return true;
    }
    public void ShowMovementRange(Unit unit)
    {
        ClearMovementRange();
        if (unit == null || movementOverlayPrefab == null)
            return;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // Ignorar la posición de la unidad.
                if (x == unit.gridX && z == unit.gridZ)
                    continue;
                // Ignorar casillas ocupadas.
                if (IsTileOccupied(x, z))
                    continue;
                // Ignorar casillas fuera del rango.
                if (!unit.CanMoveTo(x, z))
                    continue;
                Vector3 position = new Vector3(x, 0.12f, z);
                GameObject overlay = Instantiate(
                    movementOverlayPrefab,
                    position,
                    Quaternion.identity,
                    transform
                );
                movementOverlays.Add(overlay);
            }
        }
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
