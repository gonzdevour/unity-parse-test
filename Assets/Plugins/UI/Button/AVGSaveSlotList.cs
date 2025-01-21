using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AVGSaveSlotList : MonoBehaviour
{
    // 起始方法，初始化所有子物件按鈕
    public void InitSave()
    {
        // 取得當前物件下的所有 Button 元件
        Button[] buttons = GetComponentsInChildren<Button>();

        for (int i = 0; i < buttons.Length; i++)
        {
            int buttonIndex = i; // 確保閉包中的索引值正確

            // 設置按鈕的文字
            Transform titleTransform = buttons[i].transform.Find("Tx_Title");
            Transform contentTransform = buttons[i].transform.Find("Tx_Content");

            Dictionary<string, string> saveData = AVG.Inst.GetSlotData($"AVGSaveSlot{buttonIndex}");
            if (saveData == null)
            {
                Text titleText = titleTransform.GetComponent<Text>();
                titleText.text = "檔案" + buttonIndex.ToString("D2");

                Text contentText = contentTransform.GetComponent<Text>();
                contentText.text = "尚無資料";

                // 為按鈕新增點擊事件
                buttons[i].onClick.AddListener(() => Dialog.Inst.YN(
                    Content: $"確定要選擇檔案{buttonIndex.ToString("D2")}儲存進度嗎？",
                    CallbackY: () => OnButtonClicked(buttonIndex)
                    ));
            }
            else
            {
                Text titleText = titleTransform.GetComponent<Text>();
                titleText.text = "檔案" + buttonIndex.ToString("D2") + $"  《{saveData["存檔標題"]}》";

                Text contentText = contentTransform.GetComponent<Text>();
                contentText.text = saveData["存檔時間"];

                // 為按鈕新增點擊事件
                buttons[i].onClick.AddListener(() => Dialog.Inst.YN(
                    Content: $"檔案{buttonIndex.ToString("D2")}已儲存進度，確定要覆蓋嗎？",
                    CallbackY: () => OnButtonClicked(buttonIndex)
                    ));
            }
        }
    }

    public void InitLoad()
    {
        // 取得當前物件下的所有 Button 元件
        Button[] buttons = GetComponentsInChildren<Button>();

        for (int i = 0; i < buttons.Length; i++)
        {
            int buttonIndex = i; // 確保閉包中的索引值正確

            // 設置按鈕的文字
            Transform titleTransform = buttons[i].transform.Find("Tx_Title");
            Transform contentTransform = buttons[i].transform.Find("Tx_Content");

            Dictionary<string, string> saveData = AVG.Inst.GetSlotData($"AVGSaveSlot{buttonIndex}");
            if (saveData == null)
            {
                Text titleText = titleTransform.GetComponent<Text>();
                titleText.text = "檔案" + buttonIndex.ToString("D2");

                Text contentText = contentTransform.GetComponent<Text>();
                contentText.text = "尚無資料";

                // 為按鈕新增點擊事件
                buttons[i].onClick.AddListener(() => Dialog.Inst.YN(
                    Content: $"確定要選擇檔案{buttonIndex.ToString("D2")}儲存進度嗎？",
                    CallbackY: () => OnButtonClicked(buttonIndex)
                    ));
            }
            else
            {
                Text titleText = titleTransform.GetComponent<Text>();
                titleText.text = "檔案" + buttonIndex.ToString("D2") + $"  《{saveData["存檔標題"]}》";

                Text contentText = contentTransform.GetComponent<Text>();
                contentText.text = saveData["存檔時間"];

                // 為按鈕新增點擊事件
                buttons[i].onClick.AddListener(() => Dialog.Inst.YN(
                    Content: $"檔案{buttonIndex.ToString("D2")}已儲存進度，確定要覆蓋嗎？",
                    CallbackY: () => OnButtonClicked(buttonIndex)
                    ));
            }
        }
    }

    // 按鈕點擊回呼函式
    private void OnButtonClicked(int index)
    {
        Debug.Log($"按鈕 {index} 被點擊！");
    }
}
