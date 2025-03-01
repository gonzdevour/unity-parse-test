using UnityEngine;
using System.Linq;
using System;

public partial class Director
{
    private void PopUp(object[] args = null)
    {
        string DialogType = args?.ElementAtOrDefault(0)?.ToString() ?? "defaultKey";
        switch (DialogType)
        {
            case "姓名輸入框":
                AVG.Inst.isChoiceSelected = false; //可停止auto與skip
                Action<string> cbk = ChoiceContinue;
                Transform ChoiceLayer = AVG.Inst.ChoiceLayer;
                Dialog.Inst.Name("姓", "名", "請輸入姓名", "(最多可支援12個全形字)", cbk, Layer:ChoiceLayer);
                break;
            case "error":
                Debug.Log("顯示錯誤對話框");
                break;
            case "info":
                Debug.Log("顯示資訊對話框");
                break;
            default:
                Debug.Log("顯示預設對話框");
                break;
        }
    }

    private void ChoiceContinue(string cbkdata = "")
    {
        // Debug.Log($"ChoiceContinue: {cbkdata}");
        AVG.Inst.isChoiceSelected = true;
        AVG.Inst.CheckIfReadyToNext();
    }
}
