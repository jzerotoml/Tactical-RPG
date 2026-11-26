using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ActionMenu : MonoBehaviour
{
    [Header("Menu Buttons")]
    public Button moveButton;
    public Button attackButton;
    public Button waitButton;

    private Button[] buttons;
    private int selectedIndex = 0;
    [Header("References")]
    [SerializeField] private GridCursor gridCursor;

    void Awake()
    {
        buttons = new Button[]
        {
            moveButton,
            attackButton,
            waitButton
        };
    }

    void OnEnable()
    {
        selectedIndex = -1;
        MoveSelection(1);
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.sKey.wasPressedThisFrame ||
            Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            SelectNext();
        }

        if (Keyboard.current.wKey.wasPressedThisFrame ||
            Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            SelectPrevious();
        }
    }

    public void SelectNext()
    {
        MoveSelection(1);
    }

    public void SelectPrevious()
    {
        MoveSelection(-1);
    }

    public void SetActionAvailability(bool canMove, bool canAttack)
    {
        moveButton.interactable = canMove;
        attackButton.interactable = canAttack;
        waitButton.interactable = true;

        if (!IsButtonAvailable(selectedIndex))
        {
            selectedIndex = -1;
            MoveSelection(1);
        }
    }

    public void ConfirmSelection()
    {
        if (!IsButtonAvailable(selectedIndex))
            return;

        switch (selectedIndex)
        {
            case 0:
                Debug.Log("Move selected");
                gridCursor.EnterMovementMode();
                break;

            case 1:
                Debug.Log("Attack selected");
                gridCursor.EnterAttackMode();
                break;

            case 2:
                Debug.Log("Wait selected");
                gridCursor.WaitActiveUnit();
                break;
        }
    }

    private void UpdateSelection()
    {
        if (IsButtonAvailable(selectedIndex))
            buttons[selectedIndex].Select();
    }

    private bool IsButtonAvailable(int index)
    {
        return buttons != null &&
            index >= 0 &&
            index < buttons.Length &&
            buttons[index] != null &&
            buttons[index].interactable;
    }

    private void MoveSelection(int direction)
    {
        if (buttons == null || buttons.Length == 0)
            return;

        int candidate = selectedIndex;

        for (int i = 0; i < buttons.Length; i++)
        {
            candidate = (candidate + direction + buttons.Length) % buttons.Length;

            if (buttons[candidate] == null || !buttons[candidate].interactable)
                continue;

            selectedIndex = candidate;
            UpdateSelection();
            return;
        }
    }
}
