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

    public Dictionary<string, string> imagePathsSimbols = new(); // �Ÿ����|��
    public Dictionary<string, string> imagePathsBackground = new(); // �I�����|��
    public Dictionary<string, string> imagePathsPortrait = new(); // �Y�ϸ��|��
    public string DefaultPortraitImgUrl = "Resources://Sprites/Dummy/portrait/PortraitDefault.png";

    // �w�q��Ʀr��
    private Dictionary<string, Action<object[]>> actions;
    private void Start()
    {
        // �qPlayerPrefsŪ��config�]�w��
        LoadSettings();
        // ���oTEffect�ե�
        TEffect = GetComponent<TEffectsManager>().Init(CurTEffectName);
        // ��l�Ʀr��øj�w���
        actions = new Dictionary<string, Action<object[]>>
        {
            { "��", args => ChangeExpression(args) },
            { "����x", args => MoveCharX(args) },
            { "����y", args => MoveCharY(args) },
            { "�H����", args => SetRandomValue(args) },
            { "����", args => SetMoney(args) },
            { "�H�J", args => FadeIn(args) },
            { "�H�X", args => FadeOut(args) },
            { "���`", args => GoTo(args) },
            { "����", args => Cut(args) },
            { "�I��", args => SetBackground(args) },
            { "�u�X", args => PopUp(args) },
        };

        // ����
        //ExecuteAction("����,����1,10,20");
        //ExecuteAction("����,100");
        //ExecuteAction("����+100-20");
        //ExecuteAction("���� = 10");
        //ExecuteAction("�m�W = \"\"��i�Ҵ���\"\"");
        //ExecuteAction("�}�� = ��Ӱ}��");
        //ExecuteAction("HP=MaxHP*50");
        //ExecuteAction("��,�i�N�M,��");
    }

    private void LoadSettings()
    {
        // Ū�� PlayerPrefs
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
        Debug.Log($"[Director]���ձN{charUID}�������ܬ�{charEmo}");
        // �b LayerChar ���M��l����
        IChar Char = GetCharByUID(charUID);
        if (Char != null)
        {
            Debug.Log($"�e���W�s�b{charUID}");
            if (!string.IsNullOrEmpty(charEmo) && charEmo != Char.Expression)
            {
                Char.SetExpression(charEmo, "fade", DefaultCharTransDur); // �]�w��
                Debug.Log($"{charUID}�������ܬ��G{charEmo}");
            }
        }
        else
        {
            Debug.Log($"�e���W���s�b{charUID}");
        }
    }
}
