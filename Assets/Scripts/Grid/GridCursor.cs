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

    private int x = 0;
    private int z = 0;

    private bool isSelected = false;
    public bool isMenuOpen = false;
    [Header("UI References")]
    public GameObject actionMenu;
    private ActionMenu actionMenuController;

    void Start()
    {
        actionMenuController = actionMenu.GetComponent<ActionMenu>();
    }

    private Vector2 previousDirection;
    private float nextMoveTime;

    void Update()
    {
        Vector2 direction = Vector2.zero;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.enterKey.wasPressedThisFrame ||
            Keyboard.current.numpadEnterKey.wasPressedThisFrame ||
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (isMenuOpen)
            {
                actionMenuController.ConfirmSelection();
            }
            else
            {
                isSelected = true;
                isMenuOpen = true;
                actionMenu.SetActive(true);

                Debug.Log($"Action menu opened at ({x}, {z})");
            }
        }

        if (Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            if (isMenuOpen)
            {
                actionMenu.SetActive(false);
                isMenuOpen = false;
                isSelected = false;

                previousDirection = Vector2.zero;

                Debug.Log("Action menu closed");
            }
        }

        if (isMenuOpen)
        {
            previousDirection = Vector2.zero;
            return;
        }

        if (Keyboard.current.wKey.isPressed ||
            Keyboard.current.upArrowKey.isPressed)
            direction.y = 1;

        else if (Keyboard.current.sKey.isPressed ||
                Keyboard.current.downArrowKey.isPressed)
            direction.y = -1;

        if (Keyboard.current.dKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed)
            direction.x = 1;

        else if (Keyboard.current.aKey.isPressed ||
                Keyboard.current.leftArrowKey.isPressed)
            direction.x = -1;

        // Prioritize one axis to avoid diagonal movement.
        if (direction.x != 0)
            direction.y = 0;

        if (direction == Vector2.zero)
        {
            previousDirection = Vector2.zero;
            return;
        }

        // Move immediately when a key is first pressed.
        if (direction != previousDirection)
        {
            MoveCursor(direction);
            nextMoveTime = Time.time + initialDelay;
        }
        // Repeat movement while the key remains pressed.
        else if (Time.time >= nextMoveTime)
        {
            MoveCursor(direction);
            nextMoveTime = Time.time + repeatInterval;
        }

        previousDirection = direction;
    }

    void MoveCursor(Vector2 direction)
    {
        x = Mathf.Clamp(x + (int)direction.x, 0, gridWidth - 1);
        z = Mathf.Clamp(z + (int)direction.y, 0, gridHeight - 1);

        transform.position = new Vector3(x, 0.2f, z);

        Debug.Log($"Cursor: ({x}, {z})");
    }
}