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
    public float DefaultCharFocusDur = 0f;
    public float DefaultCharUnfocusDur = 0f;
    public float DefaultCharMoveDur = 0f;

    public float MusicVolume = 1f;
    public float SEVolume = 1f;

    public Dictionary<string, string> imagePathsSimbols = new(); // 符號路徑表
    public Dictionary<string, string> imagePathsBackground = new(); // 背景路徑表
    public Dictionary<string, string> imagePathsPortrait = new(); // 頭圖路徑表
    public string DefaultPortraitImgUrl = "Resources://Sprites/Dummy/portrait/PortraitDefault.png";

    // 定義函數字典
    private Dictionary<string, Action<object[]>> actions;
    private void Start()
    {
        // 從PlayerPrefs讀取config設定值
        LoadSettings();
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
            { "章節", args => GoTo(args) },
            { "結束", args => Cut(args) },
            { "背景", args => SetBackground(args) },
            { "彈出", args => PopUp(args) },
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

    private void LoadSettings()
    {
        // 讀取 PlayerPrefs
        AVG.Inst.DisplayChar = PlayerPrefs.GetInt("DisplayChar", 1) == 1;
        AVG.Inst.DisplayPortrait = PlayerPrefs.GetInt("DisplayPortrait", 1) == 1;
        AVG.Inst.DisplayStoryBox = PlayerPrefs.GetInt("DisplayStoryBox", 1) == 1;
        AVG.Inst.DisplayBubble = PlayerPrefs.GetInt("DisplayBubble", 0) == 1;
        AVG.Inst.SingleCharMode = PlayerPrefs.GetInt("SingleCharMode", 0) == 1;
        AVG.Inst.CGMode = PlayerPrefs.GetInt("CGMode", 0) == 1;

        DefaultBgTransDur = PlayerPrefs.GetFloat("DefaultBgTransDur", 1f);
        DefaultCharTransDur = PlayerPrefs.GetFloat("DefaultCharTransDur", 1f);
        DefaultCharFocusDur = PlayerPrefs.GetFloat("DefaultCharFocusDur", 0.5f);
        DefaultCharUnfocusDur = PlayerPrefs.GetFloat("DefaultCharUnfocusDur", 0f);
        DefaultCharMoveDur = PlayerPrefs.GetFloat("DefaultCharMoveDur", 0f);
        DefaultTypingInterval = PlayerPrefs.GetFloat("DefaultTypingInterval", 0.05f);

        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        SEVolume = PlayerPrefs.GetFloat("SEVolume", 1f);
    }

    private void ChangeExpression(object[] args)
    {
        string charUID = args[0]?.ToString();
        string charEmo = args[1]?.ToString();
        Debug.Log($"[Director]嘗試將{charUID}的表情轉變為{charEmo}");
        // 在 LayerChar 中尋找子物件
        IChar Char = GetCharByUID(charUID);
        if (Char != null)
        {
            Debug.Log($"畫面上存在{charUID}");
            if (!string.IsNullOrEmpty(charEmo) && charEmo != Char.Expression)
            {
                Char.SetExpression(charEmo, "fade", DefaultCharTransDur); // 設定表情
                Debug.Log($"{charUID}的表情轉變為：{charEmo}");
            }
        }
        else
        {
            Debug.Log($"畫面上不存在{charUID}");
        }
    }
}
