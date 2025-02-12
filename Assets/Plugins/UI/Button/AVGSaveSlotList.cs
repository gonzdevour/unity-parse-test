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

    // �_�l��k�A��l�ƩҦ��l������s
    public void Init(string activatorName)
    {
        if (activatorName == "Btn_Save")
        {
            ListTitle.text = "�x�s�ɮ�";
            ListLabel.color = SaveColor;
            InitSave();
        }
        else if (activatorName == "Btn_Load")
        {
            ListTitle.text = "Ū���ɮ�";
            ListLabel.color = LoadColor;
            InitLoad();
        }
    }

    public void InitSave()
    {
        // ���o��e����U���Ҧ� Button ����
        Button[] buttons = GetComponentsInChildren<Button>();

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

                // �����s�s�W�I���ƥ�
                currentButton.onClick.AddListener(() => Dialog.Inst.YN(
                    Content: $"<color=red>�ɮ�{buttonIndex.ToString("D2")}</color>�w�x�s�i�סA�T�w�n�л\�ܡH",
                    CallbackY: () => OnAVGSave(currentButton, buttonIndex)
                ));
            }
        }
    }

    public void InitLoad()
    {
        // ���o��e����U���Ҧ� Button ����
        Button[] buttons = GetComponentsInChildren<Button>();

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

                // �����s�s�W�I���ƥ�
                currentButton.onClick.AddListener(() => Dialog.Inst.Y(
                    Content: $"<color=green>�ɮ�{buttonIndex.ToString("D2")}</color>����Ƭ��šA�L�kŪ���C",
                    CallbackY: null
                ));
            }
            else
            {
                Text titleText = titleTransform.GetComponent<Text>();
                titleText.text = "�ɮ�" + buttonIndex.ToString("D2") + $"  �m{saveData["�s�ɼ��D"]}�n";

                Text contentText = contentTransform.GetComponent<Text>();
                contentText.text = saveData["�s�ɤ��"];

                // �����s�s�W�I���ƥ�
                currentButton.onClick.AddListener(() => Dialog.Inst.YN(
                    Content: $"�ثe�i�ױN�Q<color=green>�ɮ�{buttonIndex.ToString("D2")}</color>���N�A�T�w�nŪ���ܡH",
                    CallbackY: () => OnAVGLoad(currentButton, buttonIndex)
                ));
            }
        }
    }


    private void OnAVGSave(Button button, int index)
    {
        Debug.Log($"�x�s�� AVGSaveSlot{index}");
        AVG.Inst.Save("Preset", $"AVGSaveSlot{index}");

        button.transform.DOScale(1.1f, 0.2f).SetLoops(2, LoopType.Yoyo); // UI�ʺA
        InitSave(); // UI���e��s
    }

    private void OnAVGLoad(Button button, int index)
    {
        Debug.Log($"Ū�� AVGSaveSlot{index}");
        AVG.Inst.Load("Preset", $"AVGSaveSlot{index}");

        button.transform.DOScale(1.1f, 0.2f).SetLoops(2, LoopType.Yoyo); // UI�ʺA
    }
}
