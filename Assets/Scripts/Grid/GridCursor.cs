using UnityEngine;
using UnityEngine.InputSystem;

public class GridCursor : MonoBehaviour
{
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

        gridManager.Turns.ActiveUnitChanged += HandleActiveUnitChanged;
        gridManager.Battle.BattleEnded += HandleBattleEnded;
        HandleActiveUnitChanged(gridManager.Turns.ActiveUnit);
    }

    private void OnDestroy()
    {
        if (gridManager != null && gridManager.Turns != null)
            gridManager.Turns.ActiveUnitChanged -= HandleActiveUnitChanged;

        if (gridManager != null && gridManager.Battle != null)
            gridManager.Battle.BattleEnded -= HandleBattleEnded;
    }

    private void HandleActiveUnitChanged(Unit unit)
    {
        PositionCursorAtUnit(unit);
    }

    private void HandleBattleEnded()
    {
        selectedUnit = null;
        currentState = CursorState.Exploration;
        actionMenu.SetActive(false);
        previousDirection = Vector2.zero;
    }

    private void PositionCursorAtUnit(Unit unit)
    {
        if (unit == null)
            return;

        x = unit.gridX;
        z = unit.gridZ;
        transform.position = new Vector3(x, 0.2f, z);
        previousDirection = Vector2.zero;
    }

    void Update()
    {
        // Wait until the unit finishes moving.
        if (currentState == CursorState.Locked)
        {
            if (selectedUnit != null && !selectedUnit.IsMoving)
            {
                Unit movedUnit = selectedUnit;

                if (!gridManager.Turns.CompleteMove(movedUnit))
                    Debug.LogError($"Could not consume the move action for '{movedUnit.name}'.");

                ReturnAfterAction(movedUnit);
            }

            return;
        }

        if (Keyboard.current == null)
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

                Unit unitAtCursor = gridManager.GetUnitAt(x, z);

                if (unitAtCursor != null && gridManager.Turns.IsActiveUnit(unitAtCursor))
                {
                    selectedUnit = unitAtCursor;
                    OpenActionMenu();

                    Debug.Log($"Selected unit: {selectedUnit.name}");
                }
                else if (unitAtCursor != null)
                {
                    Debug.Log($"Unit '{unitAtCursor.name}' is not active.");
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

                if (!gridManager.Turns.CanMove(selectedUnit))
                {
                    Debug.Log("Move action is unavailable.");
                    CancelMovementSelection();
                    break;
                }

                if (gridManager.MoveUnit(selectedUnit, x, z))
                {
                    gridManager.ClearMovementRange();
                    currentState = CursorState.Locked;
                    previousDirection = Vector2.zero;

                    Debug.Log($"Unit moving to ({x}, {z})");
                }
                else
                {
                    Debug.Log("Invalid movement destination");
                    CancelMovementSelection();
                }

                break;

            case CursorState.Attack:

                if (!gridManager.Combat.TryAttackUnit(selectedUnit, x, z))
                    Debug.Log($"Invalid attack target at ({x}, {z})");

                CancelAttackSelection();
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

                CancelMovementSelection();

                Debug.Log("Returned to action menu");
                break;

            case CursorState.Attack:

                CancelAttackSelection();

                Debug.Log("Returned to action menu");
                break;
        }

        previousDirection = Vector2.zero;
    }

    private void CancelMovementSelection()
    {
        gridManager.ClearMovementRange();

        if (selectedUnit != null)
            PositionCursorAtUnit(selectedUnit);

        OpenActionMenu();
        previousDirection = Vector2.zero;
    }

    public void EnterMovementMode()
    {
        if (selectedUnit == null || !gridManager.Turns.CanMove(selectedUnit))
            return;

        currentState = CursorState.Movement;

        actionMenu.SetActive(false);

        gridManager.ShowMovementRange(selectedUnit);

        previousDirection = Vector2.zero;

        Debug.Log("Movement mode activated");
    }

    public void EnterAttackMode()
    {
        if (selectedUnit == null || !gridManager.Turns.CanAttack(selectedUnit))
            return;

        currentState = CursorState.Attack;
        actionMenu.SetActive(false);
        gridManager.Combat.ShowAttackRange(selectedUnit);
        previousDirection = Vector2.zero;

        Debug.Log("Attack mode activated");
    }

    private void CancelAttackSelection()
    {
        gridManager.Combat.ClearAttackRange();
        ReturnAfterAction(selectedUnit);
        previousDirection = Vector2.zero;
    }

    public void WaitActiveUnit()
    {
        Unit waitingUnit = selectedUnit;

        if (waitingUnit == null || !gridManager.Turns.EndTurn(waitingUnit))
        {
            Debug.Log("Wait action was rejected.");
            return;
        }

        Debug.Log($"Unit '{waitingUnit.name}' ended its turn.");
        selectedUnit = null;
        currentState = CursorState.Exploration;
        actionMenu.SetActive(false);
        previousDirection = Vector2.zero;
    }

    private void OpenActionMenu()
    {
        if (selectedUnit == null || !gridManager.Turns.IsActiveUnit(selectedUnit))
        {
            ReturnAfterAction(selectedUnit);
            return;
        }

        actionMenuController.SetActionAvailability(
            gridManager.Turns.CanMove(selectedUnit),
            gridManager.Turns.CanAttack(selectedUnit)
        );
        currentState = CursorState.ActionMenu;
        actionMenu.SetActive(true);
    }

    private void ReturnAfterAction(Unit actingUnit)
    {
        PositionCursorAtUnit(gridManager.Turns.ActiveUnit);

        if (actingUnit != null && gridManager.Turns.IsActiveUnit(actingUnit))
        {
            selectedUnit = actingUnit;
            OpenActionMenu();
            return;
        }

        selectedUnit = null;
        currentState = CursorState.Exploration;
        actionMenu.SetActive(false);
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

        x = newX;
        z = newZ;

        transform.position = new Vector3(x, 0.2f, z);

        Debug.Log($"Cursor: ({x}, {z})");
    }
}