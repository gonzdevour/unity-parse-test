using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AVGSaveSlotList : MonoBehaviour
{
    // �_�l��k�A��l�ƩҦ��l������s
    public void InitSave()
    {
        // ���o��e����U���Ҧ� Button ����
        Button[] buttons = GetComponentsInChildren<Button>();

        for (int i = 0; i < buttons.Length; i++)
        {
            int buttonIndex = i; // �T�O���]�������ޭȥ��T

            // �]�m���s����r
            Transform titleTransform = buttons[i].transform.Find("Tx_Title");
            Transform contentTransform = buttons[i].transform.Find("Tx_Content");

            Dictionary<string, string> saveData = AVG.Inst.GetSlotData($"AVGSaveSlot{buttonIndex}");
            if (saveData == null)
            {
                Text titleText = titleTransform.GetComponent<Text>();
                titleText.text = "�ɮ�" + buttonIndex.ToString("D2");

                Text contentText = contentTransform.GetComponent<Text>();
                contentText.text = "�|�L���";

                // �����s�s�W�I���ƥ�
                buttons[i].onClick.AddListener(() => Dialog.Inst.YN(
                    Content: $"�T�w�n����ɮ�{buttonIndex.ToString("D2")}�x�s�i�׶ܡH",
                    CallbackY: () => OnButtonClicked(buttonIndex)
                    ));
            }
            else
            {
                Text titleText = titleTransform.GetComponent<Text>();
                titleText.text = "�ɮ�" + buttonIndex.ToString("D2") + $"  �m{saveData["�s�ɼ��D"]}�n";

                Text contentText = contentTransform.GetComponent<Text>();
                contentText.text = saveData["�s�ɮɶ�"];

                // �����s�s�W�I���ƥ�
                buttons[i].onClick.AddListener(() => Dialog.Inst.YN(
                    Content: $"�ɮ�{buttonIndex.ToString("D2")}�w�x�s�i�סA�T�w�n�л\�ܡH",
                    CallbackY: () => OnButtonClicked(buttonIndex)
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
            int buttonIndex = i; // �T�O���]�������ޭȥ��T

            // �]�m���s����r
            Transform titleTransform = buttons[i].transform.Find("Tx_Title");
            Transform contentTransform = buttons[i].transform.Find("Tx_Content");

            Dictionary<string, string> saveData = AVG.Inst.GetSlotData($"AVGSaveSlot{buttonIndex}");
            if (saveData == null)
            {
                Text titleText = titleTransform.GetComponent<Text>();
                titleText.text = "�ɮ�" + buttonIndex.ToString("D2");

                Text contentText = contentTransform.GetComponent<Text>();
                contentText.text = "�|�L���";

                // �����s�s�W�I���ƥ�
                buttons[i].onClick.AddListener(() => Dialog.Inst.YN(
                    Content: $"�T�w�n����ɮ�{buttonIndex.ToString("D2")}�x�s�i�׶ܡH",
                    CallbackY: () => OnButtonClicked(buttonIndex)
                    ));
            }
            else
            {
                Text titleText = titleTransform.GetComponent<Text>();
                titleText.text = "�ɮ�" + buttonIndex.ToString("D2") + $"  �m{saveData["�s�ɼ��D"]}�n";

                Text contentText = contentTransform.GetComponent<Text>();
                contentText.text = saveData["�s�ɮɶ�"];

                // �����s�s�W�I���ƥ�
                buttons[i].onClick.AddListener(() => Dialog.Inst.YN(
                    Content: $"�ɮ�{buttonIndex.ToString("D2")}�w�x�s�i�סA�T�w�n�л\�ܡH",
                    CallbackY: () => OnButtonClicked(buttonIndex)
                    ));
            }
        }
    }

    // ���s�I���^�I�禡
    private void OnButtonClicked(int index)
    {
        Debug.Log($"���s {index} �Q�I���I");
    }
}
