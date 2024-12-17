using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScrollView_Chatbox : MonoBehaviour
{
    public Transform chatBoxContent;       // ScrollView 的 Content 物件
    public GameObject cellMsgBubblePrefab; // Cell_MsgBubble 預製件
    public RectTransform hiddenLayoutRoot; // 用於在螢幕外的暫存 Layout 容器(需在Editor中指定)
    private Queue<GameObject> messagePool = new Queue<GameObject>();
    private int poolSize = 10; // 池大小

    void Start()
    {
        // 確保 hiddenLayoutRoot 是 active，以便能在其中進行 Layout 計算
        if (hiddenLayoutRoot == null)
        {
            Debug.LogError("hiddenLayoutRoot is not assigned! Please assign a RectTransform in the inspector.");
            return;
        }

        // hiddenLayoutRoot 可以放在畫面外的座標，
        // 例如 hiddenLayoutRoot.anchoredPosition = new Vector2(-10000, -10000);
        // 確保看不見但仍然是 active。
        hiddenLayoutRoot.gameObject.SetActive(true);

        // 初始化對象池
        for (int i = 0; i < poolSize; i++)
        {
            GameObject messageObj = Instantiate(cellMsgBubblePrefab, hiddenLayoutRoot);
            messageObj.SetActive(false);
            messagePool.Enqueue(messageObj);
        }
    }

    public void UpdateChatBox(string message)
    {
        StartCoroutine(PrepareMessageBubble(message));
    }

    IEnumerator PrepareMessageBubble(string message)
    {
        GameObject messageObj;

        // 從池中取出一個 Cell_MsgBubble
        if (messagePool.Count > 0)
        {
            messageObj = messagePool.Dequeue();
        }
        else
        {
            messageObj = Instantiate(cellMsgBubblePrefab, hiddenLayoutRoot);
        }

        // 設置訊息內容
        messageObj.SetActive(true);
        Text messageText = messageObj.GetComponentInChildren<Text>();

        if (messageText != null)
        {
            messageText.text = message;
        }
        else
        {
            Debug.LogWarning("Text component not found in Cell_MsgBubble prefab.");
        }

        // 等待一幀，讓Unity先完成初步的Layout計算
        yield return null;

        // 強制更新Canvas並重建Layout - 多次呼叫以確保多層Layout都計算完成
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(hiddenLayoutRoot);
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(hiddenLayoutRoot);

        // 此時 messageObj 在 hiddenLayoutRoot 中應該已完成最終大小計算
        // 將排版完成的物件移動到 ScrollView 的 Content
        messageObj.transform.SetParent(chatBoxContent, false);
        messageObj.transform.SetAsLastSibling();

        // 如需再次強制更新ScrollView本身的排版，可再調用一次（視需求而定）
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)chatBoxContent);

        // 回收最舊的物件（超過池大小時）
        if (chatBoxContent.childCount > poolSize)
        {
            Transform oldestMessage = chatBoxContent.GetChild(0);
            oldestMessage.gameObject.SetActive(false);
            oldestMessage.SetParent(hiddenLayoutRoot, false); // 移回暫存區
            messagePool.Enqueue(oldestMessage.gameObject);
        }
    }
}
