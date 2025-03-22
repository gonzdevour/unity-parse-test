using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogSave : MonoBehaviour
{
    public GameObject Modal;
    public GameObject Board;
    public Text TxTitle;
    public Transform ButtonsParent;
    public Button BtnClose;
    public float transitionDuration = 0.5f;

    private Button[] buttons;
    private Action<string, GameObject> cbkY;

    private void OnEnable()
    {
        BtnClose.onClick.AddListener(OnCloseButtonClicked);
    }

    private void OnDisable()
    {
        BtnClose.onClick.RemoveListener(OnCloseButtonClicked);
    }

    public void Open(string Title = "", Action<string, GameObject> CallbackY = null)
    {
        TxTitle.gameObject.SetActive(!string.IsNullOrEmpty(Title));
        TxTitle.text = Title;
        cbkY = CallbackY;

        // 取得父物件下的所有 Button 元件
        buttons = ButtonsParent.GetComponentsInChildren<Button>();
        StartCoroutine(WaitForAnim());
        InitSave();
    }

    private void InitSave()
    {
        // 取得父物件下的所有 Button 元件
        Button[] buttons = ButtonsParent.GetComponentsInChildren<Button>();
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

                // 先清除舊的監聽事件，避免重複綁定
                currentButton.onClick.RemoveAllListeners();
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

                // 先清除舊的監聽事件，避免重複綁定
                currentButton.onClick.RemoveAllListeners();
                // 為按鈕新增點擊事件
                currentButton.onClick.AddListener(() => Dialog.Inst.YN(
                    Content: $"<color=red>檔案{buttonIndex.ToString("D2")}</color>已儲存進度，確定要覆蓋嗎？",
                    CallbackY: () => OnAVGSave(currentButton, buttonIndex)
                ));
            }
        }
        // 執行本地化
        Loc.Inst.Setup(gameObject);
    }

    private IEnumerator WaitForAnim()
    {
        // 等待LayoutGroup排列完成
        yield return new WaitForEndOfFrame();
        // 按鈕動畫
        foreach (var button in buttons)
        {
            GameObject buttonObj = button.gameObject;
            Image buttonImg = buttonObj.GetComponent<Image>();
            SetImageRGBA(buttonImg, a: 0f);
            buttonImg.DOFade(1f, transitionDuration).SetEase(Ease.Linear); // 顯示動畫
            Vector3 posTarget = buttonObj.transform.localPosition;// 設定飛入目標位置
            Vector3 posStart = new Vector3(posTarget.x, posTarget.y+100, posTarget.z);// 設定飛入初始位置
            buttonObj.transform.localPosition = posStart; 
            buttonObj.transform.DOLocalMove(posTarget, transitionDuration).SetEase(Ease.OutBack); // 飛入動畫
        }
    }

    public void Close(string resultString, GameObject resultButtonObj)
    {
        // 停止子物件的操作
        SetChildrenInteractable(Board, false);
        // 收合動畫
        //Sequence sequence = DOTween.Sequence();

        //Image boardImg = Board.GetComponent<Image>();
        //SetImageRGBA(boardImg, a: 1f);
        //sequence.Join(boardImg.DOFade(0f, transitionDuration).SetEase(Ease.Linear)); // 隱藏動畫

        //Vector3 posStart = Board.transform.localPosition;// 設定飛出初始位置
        //Vector3 posTarget = new Vector3(posStart.x, posStart.y + 100, posStart.z);// 設定飛出目標位置
        //sequence.Join(Board.transform.DOLocalMove(posTarget, transitionDuration).SetEase(Ease.OutBack)); // 飛入動畫
        //sequence.OnComplete(() =>
        //{
        //    cbkY?.Invoke(resultString, resultButtonObj);
        //    Destroy(gameObject);
        //});
        cbkY?.Invoke(resultString, resultButtonObj);
        Destroy(gameObject);
    }

    private void SetChildrenInteractable(GameObject parent, bool state)
    {
        foreach (Selectable selectable in parent.GetComponentsInChildren<Selectable>(true))
        {
            selectable.interactable = state;
        }
    }

    private void SetImageRGBA(Image img, float r = -1f, float g = -1f, float b = -1f, float a = -1f)
    {
        Color newColor = img.color;
        if (r > -1f) { newColor.r = r; };
        if (g > -1f) { newColor.g = g; };
        if (b > -1f) { newColor.b = b; };
        if (a > -1f) { newColor.a = a; };
        img.color = newColor;
    }

    private void OnAVGSave(Button button, int index)
    {
        Debug.Log($"儲存至 AVGSaveSlot{index}");
        AVG.Inst.Save("Preset", $"AVGSaveSlot{index}");

        button.transform.DOScale(1.1f, 0.2f).SetLoops(2, LoopType.Yoyo); // UI動態
        InitSave(); // UI內容更新
    }

    private void OnCloseButtonClicked()
    {
        Close("save dialog closed", BtnClose.gameObject);
    }
}

