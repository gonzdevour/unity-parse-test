using UnityEngine;
using UnityEngine.InputSystem;

public class InputDebugTester : MonoBehaviour
{
    private InputSystem_Actions input;
    private Vector2 moveInput;

    private void Awake()
    {
        input = new InputSystem_Actions();

        input.Player.Move.performed += ctx =>
        {
            moveInput = ctx.ReadValue<Vector2>();
            PrintDirection(moveInput);
        };

        input.Player.Move.canceled += ctx =>
        {
            moveInput = Vector2.zero;
            Debug.Log("輸入已取消（方向歸零）");
        };
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void PrintDirection(Vector2 dir)
    {
        if (dir.y > 0.5f)
            Debug.Log("⬆️ 向上");
        if (dir.y < -0.5f)
            Debug.Log("⬇️ 向下");
        if (dir.x < -0.5f)
            Debug.Log("⬅️ 向左");
        if (dir.x > 0.5f)
            Debug.Log("➡️ 向右");
    }
}
