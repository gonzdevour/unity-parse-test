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

    public Dictionary<string, string> imagePathsPortrait = new(); // �Y�ϸ��|��
    public string DefaultPortraitImgUrl = "Resources://Sprites/Dummy/portrait/PortraitDefault.png";

    // �w�q��Ʀr��
    private Dictionary<string, Action<object[]>> actions;
    private void Start()
    {
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
            { "����", args => Cut(args) },
            { "�I��", args => SetBackground(args) },
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

    private void ChangeExpression(object[] args)
    {
        string character = args[0]?.ToString();
        string expression = args[1]?.ToString();
        Debug.Log($"Change {character}'s expression to {expression}");
    }

    public void InitImagePathsPortrait(List<Dictionary<string, string>> charDataList)
    {
        // �w�q���ݩ���ȦC��
        string emoTypes = PPM.Inst.Get("�������C��"); // "�L,��,��,��,��,��,�w"
        string[] emos = emoTypes.Split(",");
        foreach (var charData in charDataList)
        {
            // �զX�C�Ӫ�����ȹ�
            foreach (string emo in emos)
            {
                if (charData.ContainsKey(emo) && !string.IsNullOrEmpty(charData[emo]))
                {
                    imagePathsPortrait[charData["UID"] + emo] = charData["AssetID"] + "-" + charData[emo] + ".png";
                    // ex: imagePathsPortrait["���w�g��"] = A-anger
                }
            }
        }
    }
}
