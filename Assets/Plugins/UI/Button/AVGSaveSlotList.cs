using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json;
using Story;

public class AVGSaveSlotList : MonoBehaviour
{
    public Text ListTitle;
    public Image ListLabel;
    public Color SaveColor;
    public Color LoadColor;

    // 起始方法，初始化所有子物件按鈕
    public void Init(string activatorName)
    {
        if (activatorName == "Btn_Save")
        {
            ListTitle.text = "儲存檔案";
            ListLabel.color = SaveColor;
            InitSave();
        }
        else if (activatorName == "Btn_Load")
        {
            ListTitle.text = "讀取檔案";
            ListLabel.color = LoadColor;
            InitLoad();
        }
    }

    public void InitSave()
    {
        // 取得當前物件下的所有 Button 元件
        Button[] buttons = GetComponentsInChildren<Button>();

        for (int i = 0; i < buttons.Length; i++)
        {
            int buttonIndex = i + 1; // 確保閉包中的索引值正確
            Button currentButton = buttons[i]; // 新增局部變數，確保閉包獲取的是當前的按鈕

            // 設置按鈕的文字
            Transform titleTransform = currentButton.transform.Find("Tx_Title");
            Transform contentTransform = currentButton.transform.Find("Tx_Content");

            Dictionary<string, string> saveData = AVG.Inst.GetSlotData($"AVGSaveSlot{buttonIndex}");
            if (saveData == null)
            {
                Text titleText = titleTransform.GetComponent<Text>();
                titleText.text = "檔案" + buttonIndex.ToString("D2");

                Text contentText = contentTransform.GetComponent<Text>();
                contentText.text = "尚無資料";

                // 為按鈕新增點擊事件
                currentButton.onClick.AddListener(() => Dialog.Inst.YN(
                    Content: $"確定要選擇<color=red>檔案{buttonIndex.ToString("D2")}</color>儲存進度嗎？",
                    CallbackY: () => OnAVGSave(currentButton, buttonIndex)
                ));
            }
            else
            {
                Text titleText = titleTransform.GetComponent<Text>();
                titleText.text = "檔案" + buttonIndex.ToString("D2") + $"  《{saveData["存檔標題"]}》";

                Text contentText = contentTransform.GetComponent<Text>();
                contentText.text = saveData["存檔日期"];

                // 為按鈕新增點擊事件
                currentButton.onClick.AddListener(() => Dialog.Inst.YN(
                    Content: $"<color=red>檔案{buttonIndex.ToString("D2")}</color>已儲存進度，確定要覆蓋嗎？",
                    CallbackY: () => OnAVGSave(currentButton, buttonIndex)
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
            int buttonIndex = i + 1; // 確保閉包中的索引值正確
            Button currentButton = buttons[i]; // 新增局部變數，確保閉包獲取的是當前的按鈕

            // 設置按鈕的文字
            Transform titleTransform = currentButton.transform.Find("Tx_Title");
            Transform contentTransform = currentButton.transform.Find("Tx_Content");

            Dictionary<string, string> saveData = AVG.Inst.GetSlotData($"AVGSaveSlot{buttonIndex}");
            if (saveData == null)
            {
                Text titleText = titleTransform.GetComponent<Text>();
                titleText.text = "檔案" + buttonIndex.ToString("D2");

                Text contentText = contentTransform.GetComponent<Text>();
                contentText.text = "尚無資料";

                // 為按鈕新增點擊事件
                currentButton.onClick.AddListener(() => Dialog.Inst.Y(
                    Content: $"<color=green>檔案{buttonIndex.ToString("D2")}</color>的資料為空，無法讀取。",
                    CallbackY: null
                ));
            }
            else
            {
                Text titleText = titleTransform.GetComponent<Text>();
                titleText.text = "檔案" + buttonIndex.ToString("D2") + $"  《{saveData["存檔標題"]}》";

                Text contentText = contentTransform.GetComponent<Text>();
                contentText.text = saveData["存檔日期"];

                // 為按鈕新增點擊事件
                currentButton.onClick.AddListener(() => Dialog.Inst.YN(
                    Content: $"目前進度將被<color=green>檔案{buttonIndex.ToString("D2")}</color>取代，確定要讀取嗎？",
                    CallbackY: () => OnAVGLoad(currentButton, buttonIndex)
                ));
            }
        }
    }


    private void OnAVGSave(Button button, int index)
    {
        Debug.Log($"儲存至 AVGSaveSlot{index}");
        AVG.Inst.Save("Preset", $"AVGSaveSlot{index}");

        button.transform.DOScale(1.1f, 0.2f).SetLoops(2, LoopType.Yoyo); // UI動態
        InitSave(); // UI內容更新
    }

    private void OnAVGLoad(Button button, int index)
    {
        Debug.Log($"讀取 AVGSaveSlot{index}");
        AVG.Inst.Load("Preset", $"AVGSaveSlot{index}");

        button.transform.DOScale(1.1f, 0.2f).SetLoops(2, LoopType.Yoyo); // UI動態
    }
}
