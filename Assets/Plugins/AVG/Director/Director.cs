using System;
using System.Collections.Generic;
using UnityEngine;

public partial class Director : MonoBehaviour
{
    public static Director Inst { get; private set; }
    private void Awake()
    {
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public AVG Avg;
    private ITransitionEffect TEffect;
    public string CurTEffectName = "Anim2";
    public float DefaultTypingInterval = 0.05f;
    public float DefaultBgTransDur = 1f;
    public float DefaultCharTransDur = 1f;
    public float DefaultCharFocusDur = 0.5f;
    public float DefaultCharUnfocusDur = 0f;
    public float DefaultCharMoveDur = 0f;

    public float MusicVolume = 1f;
    public float SEVolume = 1f;

    public Dictionary<string, string> imagePathsPortrait = new(); // 頭圖路徑表
    public string DefaultPortraitImgUrl = "Resources://Sprites/Dummy/portrait/PortraitDefault.png";

    // 定義函數字典
    private Dictionary<string, Action<object[]>> actions;
    private void Start()
    {
        // 取得TEffect組件
        TEffect = GetComponent<TEffectsManager>().Init(CurTEffectName);
        // 初始化字典並綁定函數
        actions = new Dictionary<string, Action<object[]>>
        {
            { "表情", args => ChangeExpression(args) },
            { "移動x", args => MoveCharX(args) },
            { "移動y", args => MoveCharY(args) },
            { "隨機值", args => SetRandomValue(args) },
            { "金錢", args => SetMoney(args) },
            { "淡入", args => FadeIn(args) },
            { "淡出", args => FadeOut(args) },
            { "結束", args => Cut(args) },
            { "背景", args => SetBackground(args) },
        };

        // 測試
        //ExecuteAction("移動,角色1,10,20");
        //ExecuteAction("金錢,100");
        //ExecuteAction("金錢+100-20");
        //ExecuteAction("等級 = 10");
        //ExecuteAction("姓名 = \"\"柴可夫斯基\"\"");
        //ExecuteAction("陣營 = 獲勝陣營");
        //ExecuteAction("HP=MaxHP*50");
        //ExecuteAction("表情,張沐霖,怒");
    }

    private void ChangeExpression(object[] args)
    {
        string character = args[0]?.ToString();
        string expression = args[1]?.ToString();
        Debug.Log($"Change {character}'s expression to {expression}");
    }

    public void InitImagePathsPortrait(List<Dictionary<string, string>> charDataList)
    {
        // 定義表情屬性鍵值列表
        string emoTypes = PPM.Inst.Get("表情類型列表"); // "無,喜,怒,樂,驚,疑,暈"
        string[] emos = emoTypes.Split(",");
        foreach (var charData in charDataList)
        {
            // 組合每個表情的鍵值對
            foreach (string emo in emos)
            {
                if (charData.ContainsKey(emo) && !string.IsNullOrEmpty(charData[emo]))
                {
                    imagePathsPortrait[charData["UID"] + emo] = charData["AssetID"] + "-" + charData[emo] + ".png";
                    // ex: imagePathsPortrait["高德君怒"] = A-anger
                }
            }
        }
    }
}
