using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogName : MonoBehaviour
{
    public GameObject Modal;
    public GameObject Board;
    public Button BtnY;
    public Text TxTitle;
    public Text TxContent;
    public InputField Inp_Name0;
    public InputField Inp_Name1;

    private string KeyName0;
    private string KeyName1;
    private Action<string> cbkY;

    private void OnEnable()
    {
        BtnY.onClick.AddListener(OnYesClicked);
    }

    private void OnDisable()
    {
        BtnY.onClick.RemoveListener(OnYesClicked);
    }

    public void Open(string Name0, string Name1, string Title = "", string Content = "", Action<string> CallbackY = null)
    {
        //BtnY.interactable = false; // ���T��J�m�W�~enable

        TxTitle.gameObject.SetActive(!string.IsNullOrEmpty(Title));
        TxContent.gameObject.SetActive(!string.IsNullOrEmpty(Content));
        BtnY.gameObject.SetActive(true);

        KeyName0 = Name0;
        KeyName1 = Name1;

        TxTitle.text = Title;
        TxContent.text = Content;

        cbkY = CallbackY;
        // ���楻�a��
        Loc.Inst.Setup(gameObject);
    }

    public void Close()
    {
        Destroy(gameObject);
    }

    private void OnYesClicked()
    {
        if (string.IsNullOrEmpty(Inp_Name0.text) || string.IsNullOrEmpty(Inp_Name1.text))
        {
            Dialog.Inst.Y("�п�J�m�W");
        }
        else if (GetCharacterLength() > 24) //12�ӥ��άO24
        {
            Dialog.Inst.Y("�̦h�i�䴩12�ӥ��Φr");
        }
        else
        {
            PPM.Inst.Set(KeyName0, Inp_Name0.text);
            PPM.Inst.Set(KeyName1, Inp_Name1.text);
            Debug.Log($"[PPM]�]�w{KeyName0}��{PPM.Inst.Get(KeyName0)}");
            Debug.Log($"[PPM]�]�w{KeyName1}��{PPM.Inst.Get(KeyName1)}");
            cbkY?.Invoke($"{Inp_Name0.text}{Inp_Name1.text}");
            Close();
        }
    }

    public int GetCharacterLength()
    {
        string input0 = Inp_Name0.text;
        string input1 = Inp_Name1.text;
        string combinedInput = input0 + input1;
        int totalLength = combinedInput.Sum(c => IsFullWidth(c) ? 2 : 1);
        return totalLength;
    }

    private bool IsFullWidth(char c)
    {
        // �P�_�O�_�����Φr
        return (c >= 0x4E00 && c <= 0x9FFF) || // CJK ����B���B����
               (c >= 0xFF00 && c <= 0xFFEF);   // ���βŸ� & �����W
    }
}

