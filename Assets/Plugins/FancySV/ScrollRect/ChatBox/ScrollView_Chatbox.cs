using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScrollView_Chatbox : MonoBehaviour
{
    public Transform chatBoxContent;       // ScrollView 的 Content 物件
    public GameObject AlignerCellMsgBubblePrefab; // Cell_MsgBubble 預製件
    public RectTransform hiddenLayoutRoot; // 用於在螢幕外的暫存 Layout 容器(需在Editor中指定)

    private ScrollRect scrollRect;

    void Start()
    {
        if (hiddenLayoutRoot == null)
        {
            Debug.LogError("hiddenLayoutRoot is not assigned! Please assign a RectTransform in the inspector.");
            return;
        }

        // hiddenLayoutRoot保持active並放在螢幕外，確保能正常計算Layout
        hiddenLayoutRoot.gameObject.SetActive(true);

        // 嘗試取得 ScrollRect，用於稍後捲動至底部
        scrollRect = GetComponentInParent<ScrollRect>();
        if (scrollRect == null)
        {
            Debug.LogWarning("No ScrollRect found in parent. Automatic scrolling might not work.");
        }
    }

    public void UpdateChatBox(Dictionary<string, object> serverMessage, string currentUserName)
    {
        Debug.Log("UpdateChatBox");
        StartCoroutine(PrepareMessageBubble(serverMessage, currentUserName));
    }

    IEnumerator PrepareMessageBubble(Dictionary<string, object> serverMessage, string currentUserName)
    {
        // 建立一個新的 Cell_MsgBubble 在 hiddenLayoutRoot 中
        GameObject messageObj = Instantiate(AlignerCellMsgBubblePrefab, hiddenLayoutRoot);

        messageObj.SetActive(true);
        // 設置訊息內容
        Debug.Log("PrepareMessageBubble");
        Cell_MsgBubble bubble = messageObj.GetComponentInChildren<Cell_MsgBubble>();
        if (bubble != null)
        {
            bubble.UpdateData(serverMessage, currentUserName);
        }
        else
        {
            Debug.Log("Cell_MsgBubble not exist in messageObj");
        }
        

        // 等待一幀，讓Unity先完成初步的Layout計算
        yield return null;

        // 將排版完成的物件移動到 ScrollView 的 Content
        messageObj.transform.SetParent(chatBoxContent, false);
        messageObj.transform.SetAsLastSibling();

        // 強制重新計算 chatBoxContent 的布局與大小
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)chatBoxContent);
        Canvas.ForceUpdateCanvases();

        // 將 ScrollRect 捲動到底部 (verticalNormalizedPosition = 0f 為最下方)
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}
