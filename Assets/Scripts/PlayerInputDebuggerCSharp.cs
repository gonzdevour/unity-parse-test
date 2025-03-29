using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerInputDebuggerCSharp : MonoBehaviour
{
    private PlayerInput playerInput;
    public Button defaultButton;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        // 訂閱觸發事件
        playerInput.onActionTriggered += OnAction;
    }

    private void Start()
    {
        if (defaultButton) EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);
    }

    private void OnAction(InputAction.CallbackContext context)
    {
        string actionName = context.action.name;
        string phase = context.phase.ToString();
        string value = "";

        // 額外印出值（如果是 Value 類型，例如 Move）
        if (context.action.type == InputActionType.Value)
        {
            if (context.control.valueType == typeof(Vector2))
                value = context.ReadValue<Vector2>().ToString();
            else if (context.control.valueType == typeof(float))
                value = context.ReadValue<float>().ToString("F2");
        }

        Debug.Log($"🕹️ Action: {actionName}, Phase: {phase} {(value != "" ? ", Value: " + value : "")}");
    }
}
