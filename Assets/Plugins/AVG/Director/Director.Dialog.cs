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
            case "�m�W��J��":
                AVG.Inst.isChoiceSelected = false; //�i����auto�Pskip
                Action<string> cbk = ChoiceContinue;
                Transform ChoiceLayer = AVG.Inst.ChoiceLayer;
                Dialog.Inst.Name("�m", "�W", "�п�J�m�W", "(�̦h�i�䴩12�ӥ��Φr)", cbk, Layer:ChoiceLayer);
                break;
            case "error":
                Debug.Log("��ܿ��~��ܮ�");
                break;
            case "info":
                Debug.Log("��ܸ�T��ܮ�");
                break;
            default:
                Debug.Log("��ܹw�]��ܮ�");
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
