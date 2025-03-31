using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst;
    void Awake() { if (Inst == null) Inst = this; else Destroy(gameObject); }

    private Vector3 playerStartPosition;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerStartPosition = player.transform.position;
    }

    public void OnPlayerDead()
    {
        Debug.Log("💀 玩家死亡，傳送回初始位置");

        player.transform.position = playerStartPosition;

        // 重置速度（避免飛出）
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }
}
