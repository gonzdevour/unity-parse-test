using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst;
    void Awake() { if (Inst == null) Inst = this; else Destroy(gameObject); }

    private Vector3 playerStartPosition;
    private GameObject player;

    [Header("平台預製件")]
    public GameObject platformPrefab;

    [Header("捲軸與產生設定")]
    public float scrollSpeed = 2f;               // 捲軸速度（單位/秒）
    public float spawnIntervalDistance = 2.5f;   // 每隔多少距離產生平台

    private float accumulatedDistance = 0f;
    private float spawnThreshold;
    private float lastSpawnY = -5.4f;

    private List<GameObject> platforms = new List<GameObject>();

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerStartPosition = player.transform.position;

        // 計算多少距離會觸發一次平台產生
        spawnThreshold = spawnIntervalDistance;
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

    private void FixedUpdate()
    {
        float deltaY = scrollSpeed * Time.fixedDeltaTime;
        accumulatedDistance += deltaY;

        // 移動所有平台
        for (int i = platforms.Count - 1; i >= 0; i--)
        {
            GameObject platform = platforms[i];
            platform.transform.position += Vector3.up * deltaY;

            // 超出畫面上方則刪除
            if (platform.transform.position.y > 5.4f)
            {
                Destroy(platform);
                platforms.RemoveAt(i);
            }
        }

        // 如果累積距離超過門檻，產生新平台
        if (accumulatedDistance >= spawnThreshold)
        {
            SpawnPlatform();
            accumulatedDistance = 0f;
        }
    }

    private void SpawnPlatform()
    {
        float x = Random.Range(-3f, 3f);
        Vector3 spawnPos = new Vector3(x, lastSpawnY, 0f);
        GameObject newPlatform = Instantiate(platformPrefab, spawnPos, Quaternion.identity);
        platforms.Add(newPlatform);
    }
}
