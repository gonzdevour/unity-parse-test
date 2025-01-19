using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace LSR
{
    [RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
    [DisallowMultipleComponent]
    public class LoopScrollView : MonoBehaviour, LoopScrollPrefabSource, LoopScrollDataSource
    {
        public GameObject item;
        public int totalCount = -1;

        public List<Dictionary<string, object>> chatBubbleMessages = new();

        // Implement your own Cache Pool here. The following is just for example.
        Stack<Transform> pool = new Stack<Transform>();

        private LoopScrollRect ls;

        void Start()
        {
            //StartCoroutine(TestUpdateChatBox(100));
        }

        private void OnEnable()
        {
            ls = GetComponent<LoopScrollRect>();
            ls.prefabSource = this;
            ls.dataSource = this;
            Refresh();
        }

        void Refresh()
        {
            //Debug.Log($"total count: {chatBubbleMessages.Count}");
            ls.totalCount = chatBubbleMessages.Count;
            //ls.RefillCells();//ls.RefillCells會讓scroll歸0，其實設定totoalCount時會自動ls.refresh，好像不必每次都手動ls.Refresh或ls.Refill
            bool AtBottom = IsScrolledToBottom();
            Debug.Log("At Bottom: " + AtBottom);
            if (AtBottom) //如果在最底部才允許自動scroll
            {
                ls.ScrollToCell(chatBubbleMessages.Count - 1, 2000);
            }
        }

        public void Clear()
        {
            chatBubbleMessages.Clear();
        }

        public GameObject GetObject(int index)
        {
            if (pool.Count == 0)
            {
                return Instantiate(item);
            }
            Transform candidate = pool.Pop();
            candidate.gameObject.SetActive(true);
            return candidate.gameObject;
        }

        public void ReturnObject(Transform trans)
        {
            // Use `DestroyImmediate` here if you don't need Pool
            trans.SendMessage("ScrollCellReturn", SendMessageOptions.DontRequireReceiver);
            trans.gameObject.SetActive(false);
            trans.SetParent(transform, false);
            pool.Push(trans);
        }

        public void ProvideData(Transform transform, int idx)
        {
            transform.SendMessage("ScrollCellIndex", idx);
        }

        private bool IsScrolledToBottom()
        {
            RectTransform content = ls.content;
            RectTransform view = ls.viewport;

            // 滾動內容的總高度和滾動視窗的高度
            float contentHeight = content.rect.height;
            float viewHeight = view.rect.height;

            // 錨定位置的 Y 值
            float anchoredPositionY = content.anchoredPosition.y;

            // 滾動到最下方的條件：錨定位置 + 視窗高度 >= 總內容高度-誤差容許值
            //Debug.Log($"{anchoredPositionY}+{viewHeight}>={contentHeight}-10({contentHeight - 10f})  {anchoredPositionY + viewHeight >= contentHeight - 10f}");
            return anchoredPositionY + viewHeight >= contentHeight-10f;
        }

        public void UpdateChatBox(Dictionary<string, object> serverMessage, string currentUserName)
        {
            // 從 serverMessage 中提取訊息資料
            string userName = serverMessage.ContainsKey("userName") ? serverMessage["userName"]?.ToString() : "Unknown";
            string message = serverMessage.ContainsKey("message") ? serverMessage["message"]?.ToString() : "";
            string imgUrl = serverMessage.ContainsKey("imgUrl") ? serverMessage["imgUrl"]?.ToString() : "";
            bool isMyMessage = currentUserName == userName;

            // 創建一個新的訊息資料字典
            var chatData = new Dictionary<string, object>
            {
                { "userName", userName },
                { "message", message },
                { "imgUrl", imgUrl },
                { "isMyMessage", isMyMessage }
            };

            // 將新的訊息資料推入 chatBubbleMessages
            chatBubbleMessages.Add(chatData);
            if (gameObject.activeInHierarchy) Refresh();
        }

        public IEnumerator TestUpdateChatBox(int cnt)
        {
            for (int i = 0; i < cnt; i++)
            {
                string randomName = GenerateRandomString(UnityEngine.Random.Range(3, 11));
                string userName = UnityEngine.Random.value > 0.5f ? "CurrentUser" : randomName;
                string message = i.ToString("000") + "_" + GenerateRandomString(UnityEngine.Random.Range(20, 150));

                var serverMessage = new Dictionary<string, object>
            {
                { "userName", userName },
                { "message", message },
            };

                UpdateChatBox(serverMessage, "CurrentUser");
                yield return new WaitForSeconds(3f);
            }
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            char[] stringChars = new char[length];

            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[UnityEngine.Random.Range(0, chars.Length)];
            }

            return new string(stringChars);
        }
    }
}