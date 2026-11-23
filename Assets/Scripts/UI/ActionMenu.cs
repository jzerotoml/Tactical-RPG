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
    selectedIndex = 0;

    if (buttons != null && buttons.Length > 0 && buttons[0] != null)
    {
        buttons[0].Select();
    }
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
        selectedIndex = (selectedIndex + 1) % buttons.Length;
        UpdateSelection();
    }

    public void SelectPrevious()
    {
        selectedIndex = (selectedIndex - 1 + buttons.Length) % buttons.Length;
        UpdateSelection();
    }

    public void ConfirmSelection()
{
    switch (selectedIndex)
        {
            case 0:
                Debug.Log("Move selected");
                gridCursor.EnterMovementMode();
                break;

            case 1:
                Debug.Log("Attack selected");
                break;

            case 2:
                Debug.Log("Wait selected");
                break;
        }
    }
    private void UpdateSelection()
    {
        buttons[selectedIndex].Select();
    }
}
