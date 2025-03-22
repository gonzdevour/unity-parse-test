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

        // ���o������U���Ҧ� Button ����
        buttons = ButtonsParent.GetComponentsInChildren<Button>();
        StartCoroutine(WaitForAnim());
        InitSave();
    }

    private void InitSave()
    {
        // ���o������U���Ҧ� Button ����
        Button[] buttons = ButtonsParent.GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            int buttonIndex = i + 1; // �T�O���]�������ޭȥ��T
            Button currentButton = buttons[i]; // �s�W�����ܼơA�T�O���]������O��e�����s

            // �]�m���s����r
            Transform titleTransform = currentButton.transform.Find("Tx_Title");
            Transform contentTransform = currentButton.transform.Find("Tx_Content");

            Dictionary<string, string> saveData = AVG.Inst.GetSlotData($"AVGSaveSlot{buttonIndex}");
            if (saveData == null)
            {
                Text titleText = titleTransform.GetComponent<Text>();
                titleText.text = "�ɮ�" + buttonIndex.ToString("D2");

                Text contentText = contentTransform.GetComponent<Text>();
                contentText.text = "�|�L���";

                // ���M���ª���ť�ƥ�A�קK���Ƹj�w
                currentButton.onClick.RemoveAllListeners();
                // �����s�s�W�I���ƥ�
                currentButton.onClick.AddListener(() => Dialog.Inst.YN(
                    Content: $"�T�w�n���<color=red>�ɮ�{buttonIndex.ToString("D2")}</color>�x�s�i�׶ܡH",
                    CallbackY: () => OnAVGSave(currentButton, buttonIndex)
                ));
            }
            else
            {
                Text titleText = titleTransform.GetComponent<Text>();
                titleText.text = "�ɮ�" + buttonIndex.ToString("D2") + $"  �m{saveData["�s�ɼ��D"]}�n";

                Text contentText = contentTransform.GetComponent<Text>();
                contentText.text = saveData["�s�ɤ��"];

                // ���M���ª���ť�ƥ�A�קK���Ƹj�w
                currentButton.onClick.RemoveAllListeners();
                // �����s�s�W�I���ƥ�
                currentButton.onClick.AddListener(() => Dialog.Inst.YN(
                    Content: $"<color=red>�ɮ�{buttonIndex.ToString("D2")}</color>�w�x�s�i�סA�T�w�n�л\�ܡH",
                    CallbackY: () => OnAVGSave(currentButton, buttonIndex)
                ));
            }
        }
        // ���楻�a��
        Loc.Inst.Setup(gameObject);
    }

    private IEnumerator WaitForAnim()
    {
        // ����LayoutGroup�ƦC����
        yield return new WaitForEndOfFrame();
        // ���s�ʵe
        foreach (var button in buttons)
        {
            GameObject buttonObj = button.gameObject;
            Image buttonImg = buttonObj.GetComponent<Image>();
            SetImageRGBA(buttonImg, a: 0f);
            buttonImg.DOFade(1f, transitionDuration).SetEase(Ease.Linear); // ��ܰʵe
            Vector3 posTarget = buttonObj.transform.localPosition;// �]�w���J�ؼЦ�m
            Vector3 posStart = new Vector3(posTarget.x, posTarget.y+100, posTarget.z);// �]�w���J��l��m
            buttonObj.transform.localPosition = posStart; 
            buttonObj.transform.DOLocalMove(posTarget, transitionDuration).SetEase(Ease.OutBack); // ���J�ʵe
        }
    }

    public void Close(string resultString, GameObject resultButtonObj)
    {
        // ����l���󪺾ާ@
        SetChildrenInteractable(Board, false);
        // ���X�ʵe
        //Sequence sequence = DOTween.Sequence();

        //Image boardImg = Board.GetComponent<Image>();
        //SetImageRGBA(boardImg, a: 1f);
        //sequence.Join(boardImg.DOFade(0f, transitionDuration).SetEase(Ease.Linear)); // ���ðʵe

        //Vector3 posStart = Board.transform.localPosition;// �]�w���X��l��m
        //Vector3 posTarget = new Vector3(posStart.x, posStart.y + 100, posStart.z);// �]�w���X�ؼЦ�m
        //sequence.Join(Board.transform.DOLocalMove(posTarget, transitionDuration).SetEase(Ease.OutBack)); // ���J�ʵe
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
        Debug.Log($"�x�s�� AVGSaveSlot{index}");
        AVG.Inst.Save("Preset", $"AVGSaveSlot{index}");

        button.transform.DOScale(1.1f, 0.2f).SetLoops(2, LoopType.Yoyo); // UI�ʺA
        InitSave(); // UI���e��s
    }

    private void OnCloseButtonClicked()
    {
        Close("save dialog closed", BtnClose.gameObject);
    }
}

