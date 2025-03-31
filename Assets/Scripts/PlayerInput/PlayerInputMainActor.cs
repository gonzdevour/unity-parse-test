using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputMainActor : MonoBehaviour
{
    [Header("基本跳躍參數")]
    public float moveSpeed = 5f;
    public float jumpHeight = 3f;
    private float jumpForce;

    [Header("重力調整")]
    public float fallMultiplier = 2.5f;      // 墜落加速倍數
    public float lowJumpMultiplier = 2.0f;   // 短跳加速倍數

    [Header("地面判斷")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private BoxCollider2D box;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isJumpPressed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();

        // 在 Awake 時根據重力和跳高計算出需要的初始速度
        RecalculateJumpForce();
        Debug.Log($"[JUMP CALC] 目標跳高 = {jumpHeight} 單位, 計算出跳躍速度 = {jumpForce:F2}");
    }

    private void RecalculateJumpForce()
    {
        float effectiveGravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        jumpForce = Mathf.Sqrt(2f * effectiveGravity * jumpHeight);
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        isJumpPressed = value.isPressed;
        Debug.Log($"[PLAYER] Jump input received. isPressed: {isJumpPressed}");

        if (isGrounded)
        {
            Debug.Log("[PLAYER] ✅ Grounded = true");
        }
        else
        {
            Debug.Log("[PLAYER] ❌ Grounded = false");
        }

        if (isGrounded && isJumpPressed)
        {
            Debug.Log("[PLAYER] 🟢 Jump conditions met → applying jump force");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        else
        {
            Debug.Log("[PLAYER] 🔴 Jump blocked → condition not met");
        }
    }


    private void Update()
    {
        Vector2 boxCenter = (Vector2)groundCheck.position;
        Vector2 boxSize = new Vector2(box.size.x*0.9f, box.size.y*0.1f); // 可以設為比角色 collider 稍微小的底部區域

        isGrounded = Physics2D.OverlapBox(boxCenter, boxSize, 0f, groundLayer);
    }

    private void FixedUpdate()
    {
        // 水平移動
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // 下墜加速（Hollow Knight 風格）
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        // 短跳處理（提早放開跳鍵）
        else if (rb.linearVelocity.y > 0 && !isJumpPressed)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        if (box == null || groundCheck == null) return;

        Vector2 boxCenter = groundCheck.position;
        Vector2 boxSize = new(box.size.x * 0.9f, box.size.y * 0.1f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }

    public void OnLook(InputValue value)
    {
        Debug.Log($"[PLAYER] Look: {value.Get<Vector2>()}");
    }

    //public void OnJump()
    //{
    //    Debug.Log("[PLAYER] Jump");
    //}

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
