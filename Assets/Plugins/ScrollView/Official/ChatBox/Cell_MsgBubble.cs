using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Collections.Generic;

class Cell_MsgBubble : MonoBehaviour
{
    [SerializeField] private Text txName = default;
    [SerializeField] private Text txMessage = default;
    [SerializeField] private Image portrait = default;
    [SerializeField] private Image cellImage = default;
    [SerializeField] private Button cellButton = default;

    public void UpdateData(Dictionary<string, object> serverMessage, string currentUserName)
    {
        string userName = serverMessage.ContainsKey("userName") ? serverMessage["userName"]?.ToString() : "Unknown";
        string message = serverMessage.ContainsKey("message") ? serverMessage["message"]?.ToString() : "";
        bool isMyMessage = currentUserName == userName;
        SetAlignment(isMyMessage);

        if (txName != null)
            txName.text = userName;
        else
            Debug.LogWarning("txName is not assigned in the Inspector.");

        if (txMessage != null)
            txMessage.text = message;
        else
            Debug.LogWarning("txMessage is not assigned in the Inspector.");
    }

    public void SetPortrait(Sprite newPortrait)
    {
        if (portrait != null && newPortrait != null)
            portrait.sprite = newPortrait;
    }

    public void SetCellBackground(Sprite background)
    {
        if (cellImage != null && background != null)
            cellImage.sprite = background;
    }

    /// <summary>
    /// 設置對齊方式：如果是自己發送的訊息，將 Child Alignment 設為 Upper Right
    /// </summary>
    public void SetAlignment(bool isMine)
    {
        // 取得父物件Aligner中的 Horizontal Layout Group
        HorizontalLayoutGroup layoutGroup = GetComponentInParent<HorizontalLayoutGroup>();

        if (layoutGroup != null)
        {
            // 根據 isMine 設定 Child Alignment
            if (isMine)
            {
                layoutGroup.childAlignment = TextAnchor.UpperRight; // 右上對齊
                cellButton.image.color = Color.blue;
            }
            else
            {
                layoutGroup.childAlignment = TextAnchor.UpperLeft; // 左上對齊
            }
        }
        else
        {
            Debug.LogWarning("HorizontalLayoutGroup not found on the parent object.");
        }
    }
}
