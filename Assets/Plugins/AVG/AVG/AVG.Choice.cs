using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using System;

public partial class AVG
{
    IEnumerator StartChoose(Dictionary<string, object> storyCutDict)
    {
        // ��l�ƿ�ܪ��A
        isChoiceSelected = false;
        // �ҥοﶵ�M�Υb�z����(�B��������i���StoryBox)
        ChoiceCover.SetActive(true);
        // �qstoryCutDict�ѪR�ëإ߿ﶵ���
        string[] options = storyCutDict["�ﶵ"].ToString().Split('\n').Select(option => TxR.Inst.Render(ParseEx(option))).ToArray();
        string[] results = storyCutDict["�e��"].ToString().Split('\n').Select(result => ParseEx(result)).ToArray();
        Action<string, GameObject> cbk = OnChoiceSelected;
        Dialog.Inst.Choices(options, results, CallbackY:cbk, Layer: ChoiceLayer);
        // ���ݿ�ܧ���
        yield return new WaitUntil(() => isChoiceSelected);
        // �����ﶵ�M�Υb�z����
        ChoiceCover.SetActive(false);
    }

    public void OnChoiceSelected(string resultIndexString, GameObject button)
    {
        //Debug.Log($"OnChoiceSelected {resultIndexString}");
        gotoIndex = int.Parse(resultIndexString);
        isChoiceSelected = true;
        Debug.Log($"choose to go to cut: {gotoIndex}");
    }
}
