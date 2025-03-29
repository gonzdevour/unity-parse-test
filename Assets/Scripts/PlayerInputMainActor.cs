using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputMainActor : MonoBehaviour
{
    public GameObject Collider;

    private Rigidbody2D body;
    private Vector2 moveInput;
    private float moveSpeed = 5f;

    public void Awake()
    {
        body = Collider.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // 以物理方式移動 Rigidbody2D
        body.linearVelocity = new Vector2(moveInput.x * moveSpeed, body.linearVelocity.y);
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        Debug.Log($"[PLAYER] Move: {value.Get<Vector2>()}");
    }

    public void OnLook(InputValue value)
    {
        Debug.Log($"[PLAYER] Look: {value.Get<Vector2>()}");
    }

    public void OnJump()
    {
        Debug.Log("[PLAYER] Jump");
    }

    public void OnSprint()
    {
        Debug.Log("[PLAYER] Sprint");
    }

    public void OnAttack()
    {
        Debug.Log("[PLAYER] Attack");
    }

    public void OnCrouch()
    {
        Debug.Log("[PLAYER] Crouch");
    }

    public void OnInteract()
    {
        Debug.Log("[PLAYER] Interact");
    }

    public void OnNext()
    {
        Debug.Log("[PLAYER] Next");
    }

    public void OnPrevious()
    {
        Debug.Log("[PLAYER] Previous");
    }

    public void OnNavigate(InputValue value)
    {
        Debug.Log($"[UI] Navigate: {value.Get<Vector2>()}");
    }

    public void OnSubmit()
    {
        Debug.Log("[UI] Submit");
    }

    public void OnCancel()
    {
        Debug.Log("[UI] Cancel");
    }

    public void OnPoint(InputValue value)
    {
        Debug.Log($"[UI] Point: {value.Get<Vector2>()}");
    }

    public void OnClick()
    {
        Debug.Log("[UI] Click");
    }

    public void OnRightClick()
    {
        Debug.Log("[UI] Right Click");
    }

    public void OnMiddleClick()
    {
        Debug.Log("[UI] Middle Click");
    }

    public void OnScrollWheel(InputValue value)
    {
        Debug.Log($"[UI] ScrollWheel: {value.Get<Vector2>()}");
    }

    public void OnTrackedDevicePosition(InputValue value)
    {
        Debug.Log($"[UI] Tracked Device Pos: {value.Get<Vector3>()}");
    }

    public void OnTrackedDeviceOrientation(InputValue value)
    {
        Debug.Log($"[UI] Tracked Device Rot: {value.Get<Quaternion>()}");
    }
}
