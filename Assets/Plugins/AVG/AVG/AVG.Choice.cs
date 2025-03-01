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
        // 初始化選擇狀態
        isChoiceSelected = false;
        // 啟用選項專用半透明幕(遮住場景但可顯示StoryBox)
        ChoiceCover.SetActive(true);
        // 從storyCutDict解析並建立選項資料
        string[] options = storyCutDict["選項"].ToString().Split('\n').Select(option => TxR.Inst.Render(ParseEx(option))).ToArray();
        string[] results = storyCutDict["前往"].ToString().Split('\n').Select(result => ParseEx(result)).ToArray();
        Action<string, GameObject> cbk = OnChoiceSelected;
        Dialog.Inst.Choices(options, results, CallbackY:cbk, Layer: ChoiceLayer);
        // 等待選擇完成
        yield return new WaitUntil(() => isChoiceSelected);
        // 關閉選項專用半透明幕
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
