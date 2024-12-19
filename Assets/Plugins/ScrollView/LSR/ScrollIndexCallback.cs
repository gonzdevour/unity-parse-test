using UnityEngine;
using UnityEngine.UI;

namespace LSR
{
    public class ScrollIndexCallback : MonoBehaviour
    {
        public Text txName;
        public Text txMessage;
        public LayoutElement element;

        public HorizontalLayoutGroup Aligner;
        public VerticalLayoutGroup bubbleVLG;
        public RectTransform portrait;
        public Image cellBg; // 假設 cell 背景是一個 Image
        public Color MyColor;
        public Color NotMyColor;

        void ScrollCellIndex(int idx)
        {
            UpdateMessage(idx);
            element.preferredHeight = GetCellHeight();
            Debug.Log(element.preferredHeight);
            gameObject.name = $"Cell{idx}";
        }

        float GetCellHeight()
        {
            float totalHeight = 0f;

            float portraitHeight = 0f;
            float msgHeight = 0f;
            float portraitMsgHeight = 0f;

            // 加上 bubbleVLG 的 padding
            if (bubbleVLG != null)
            {
                totalHeight += bubbleVLG.padding.top;
                totalHeight += bubbleVLG.padding.bottom;
            }

            // 計算 txName 的高度
            if (txName != null)
            {
                totalHeight += txName.rectTransform.rect.height;
            }

            // 計算 portrait 的高度
            if (portrait != null)
            {
                portraitHeight = portrait.rect.height;
            }
            // 計算 txMessage 的高度
            if (txMessage != null)
            {
                msgHeight = CalculateTextHeight(txMessage);

                // 修改 RectTransform 的 sizeDelta 來設置高度
                RectTransform rectTransform = txMessage.rectTransform;
                Vector2 sizeDelta = rectTransform.sizeDelta;
                sizeDelta.y = msgHeight; // 設置高度
                rectTransform.sizeDelta = sizeDelta;
            }

            portraitMsgHeight = msgHeight > portraitHeight ? msgHeight : portraitHeight;
            totalHeight += portraitMsgHeight;

            return totalHeight;
        }

        float CalculateTextHeight(Text text)
        {
            if (text == null) return 0f;

            // 文字物件寬度固定，用來決定行數
            float width = text.rectTransform.rect.width;

            // 使用字體生成器計算文字的所需空間
            var generator = new TextGenerator();
            var generationSettings = text.GetGenerationSettings(new Vector2(width, 0f));
            float textHeight = generator.GetPreferredHeight(text.text, generationSettings);

            // 調整文字高度計算結果
            return textHeight;
        }

        void UpdateMessage(int idx)
        {
            // 從父物件中取得 LoopScrollView
            var loopScrollView = GetComponentInParent<LoopScrollView>();
            if (loopScrollView != null)
            {
                var bubbleMsg = loopScrollView.chatBubbleMessages[idx];
                if (bubbleMsg != null)
                {
                    string userName = bubbleMsg["userName"] as string; // 使用 `as` 進行類型轉換
                    string message = bubbleMsg["message"] as string;
                    bool isMyMessage = (bool)bubbleMsg["isMyMessage"]; // 使用顯式轉換

                    if (txName != null)
                    {
                        txName.text = userName;
                    }

                    if (txMessage != null)
                    {
                        txMessage.text = message;
                    }

                    // 根據 isMyMessage 設置 Aligner 和 cellBg
                    if (Aligner != null)
                    {
                        Aligner.childAlignment = isMyMessage ? TextAnchor.UpperRight : TextAnchor.UpperLeft;
                    }

                    if (cellBg != null)
                    {
                        cellBg.color = isMyMessage ? MyColor : NotMyColor;
                    }
                }
            }
        }
    }
}
