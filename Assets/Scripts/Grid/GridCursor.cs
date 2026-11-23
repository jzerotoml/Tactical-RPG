using UnityEngine;
using UnityEngine.InputSystem;

public class GridCursor : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 8;
    public int gridHeight = 8;

    [Header("Movement Settings")]
    public float initialDelay = 0.25f;
    public float repeatInterval = 0.10f;

    [Header("Cursor State")]
    public CursorState currentState = CursorState.Exploration;

    [Header("UI References")]
    public GameObject actionMenu;

    [Header("Grid References")]
    [SerializeField] private GridManager gridManager;

    private ActionMenu actionMenuController;

    private int x = 0;
    private int z = 0;

    private Unit selectedUnit;

    private Vector2 previousDirection;
    private float nextMoveTime;

    void Start()
    {
        actionMenuController = actionMenu.GetComponent<ActionMenu>();

        actionMenu.SetActive(false);

        transform.position = new Vector3(x, 0.2f, z);
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        // Locked: no player input allowed.
        if (currentState == CursorState.Locked)
            return;

        // Cancel or go back.
        if (Keyboard.current.escapeKey.wasPressedThisFrame ||
            Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            HandleCancel();
            return;
        }

        // Confirm selection.
        if (Keyboard.current.enterKey.wasPressedThisFrame ||
            Keyboard.current.numpadEnterKey.wasPressedThisFrame ||
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            HandleConfirm();
            return;
        }

        // The action menu handles its own navigation.
        if (currentState == CursorState.ActionMenu)
        {
            previousDirection = Vector2.zero;
            return;
        }

        HandleCursorMovement();
    }

    void HandleConfirm()
    {
        switch (currentState)
        {
            case CursorState.Exploration:

                selectedUnit = gridManager.GetUnitAt(x, z);

                if (selectedUnit != null)
                {
                    currentState = CursorState.ActionMenu;
                    actionMenu.SetActive(true);

                    Debug.Log($"Selected unit: {selectedUnit.name}");
                }
                else
                {
                    Debug.Log($"No unit at ({x}, {z})");
                }

                break;

            case CursorState.ActionMenu:

                actionMenuController.ConfirmSelection();
                break;

            case CursorState.Movement:

                Debug.Log($"Movement destination selected: ({x}, {z})");
                // Actual unit movement will be implemented later.
                break;

            case CursorState.Attack:

                Debug.Log($"Attack target selected: ({x}, {z})");
                // Attack logic will be implemented later.
                break;
        }
    }

    void HandleCancel()
    {
        switch (currentState)
        {
            case CursorState.ActionMenu:

                actionMenu.SetActive(false);
                selectedUnit = null;

                currentState = CursorState.Exploration;

                Debug.Log("Returned to exploration");
                break;

            case CursorState.Movement:

                gridManager.ClearMovementRange();
                if (selectedUnit != null)
                {
                    x = selectedUnit.gridX;
                    z = selectedUnit.gridZ;

                    transform.position = new Vector3(x, 0.2f, z);
                }

                currentState = CursorState.ActionMenu;
                actionMenu.SetActive(true);

                Debug.Log("Returned to action menu");
                break;

            case CursorState.Attack:

                currentState = CursorState.ActionMenu;
                actionMenu.SetActive(true);

                Debug.Log("Returned to action menu");
                break;
        }

        previousDirection = Vector2.zero;
    }

    public void EnterMovementMode()
    {
        if (selectedUnit == null)
            return;

        currentState = CursorState.Movement;

        actionMenu.SetActive(false);

        gridManager.ShowMovementRange(selectedUnit);

        previousDirection = Vector2.zero;

        Debug.Log("Movement mode activated");
    }

    void HandleCursorMovement()
    {
        Vector2 direction = Vector2.zero;

        if (Keyboard.current.wKey.isPressed ||
            Keyboard.current.upArrowKey.isPressed)
        {
            direction.y = 1;
        }
        else if (Keyboard.current.sKey.isPressed ||
                Keyboard.current.downArrowKey.isPressed)
        {
            direction.y = -1;
        }

        if (Keyboard.current.dKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed)
        {
            direction.x = 1;
        }
        else if (Keyboard.current.aKey.isPressed ||
                Keyboard.current.leftArrowKey.isPressed)
        {
            direction.x = -1;
        }

        // Prevent diagonal movement.
        if (direction.x != 0)
            direction.y = 0;

        if (direction == Vector2.zero)
        {
            previousDirection = Vector2.zero;
            return;
        }

        if (direction != previousDirection)
        {
            MoveCursor(direction);
            nextMoveTime = Time.time + initialDelay;
        }
        else if (Time.time >= nextMoveTime)
        {
            MoveCursor(direction);
            nextMoveTime = Time.time + repeatInterval;
        }

        previousDirection = direction;
    }

    void MoveCursor(Vector2 direction)
    {
        int newX = Mathf.Clamp(
            x + (int)direction.x,
            0,
            gridManager.width - 1
        );

        int newZ = Mathf.Clamp(
            z + (int)direction.y,
            0,
            gridManager.height - 1
        );

        // During movement selection, restrict the cursor
        // to tiles within the unit's movement range.
        if (currentState == CursorState.Movement)
        {
            if (selectedUnit == null)
                return;

            if (!selectedUnit.CanMoveTo(newX, newZ))
                return;

            if (gridManager.IsTileOccupied(newX, newZ) &&
                (newX != selectedUnit.gridX ||
                newZ != selectedUnit.gridZ))
                return;
        }

        x = newX;
        z = newZ;

        transform.position = new Vector3(x, 0.2f, z);

        Debug.Log($"Cursor: ({x}, {z})");
    }
}