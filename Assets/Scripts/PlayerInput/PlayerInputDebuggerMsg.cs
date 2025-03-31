using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputDebuggerMsg : MonoBehaviour
{
    public void OnMove(InputValue value)
    {
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
