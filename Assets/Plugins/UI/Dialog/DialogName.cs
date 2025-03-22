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
        //BtnY.interactable = false; // 正確填入姓名才enable

        TxTitle.gameObject.SetActive(!string.IsNullOrEmpty(Title));
        TxContent.gameObject.SetActive(!string.IsNullOrEmpty(Content));
        BtnY.gameObject.SetActive(true);

        KeyName0 = Name0;
        KeyName1 = Name1;

        TxTitle.text = Title;
        TxContent.text = Content;

        cbkY = CallbackY;
        // 執行本地化
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
            Dialog.Inst.Y("請輸入姓名");
        }
        else if (GetCharacterLength() > 24) //12個全形是24
        {
            Dialog.Inst.Y("最多可支援12個全形字");
        }
        else
        {
            PPM.Inst.Set(KeyName0, Inp_Name0.text);
            PPM.Inst.Set(KeyName1, Inp_Name1.text);
            Debug.Log($"[PPM]設定{KeyName0}為{PPM.Inst.Get(KeyName0)}");
            Debug.Log($"[PPM]設定{KeyName1}為{PPM.Inst.Get(KeyName1)}");
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
        // 判斷是否為全形字
        return (c >= 0x4E00 && c <= 0x9FFF) || // CJK 中文、日文、韓文
               (c >= 0xFF00 && c <= 0xFFEF);   // 全形符號 & 片假名
    }
}

