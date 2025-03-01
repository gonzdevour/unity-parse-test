using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

// �ޤJ����Story����Ƶ��c(�pBgData�BCharData���e�����W��class)
using Story;
// �ޤJLSR�H�s�@Logger
using LSR;

public partial class AVG : MonoBehaviour
{
    public static AVG Inst { get; private set; }
    private void Awake()
    {
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    [Header("�O��")]
    public bool ClearReadList = true; //��l�ƮɲM���wŪ���D
    public GameObject SaveLoadPanel;

    [Header("�R�x")]
    public RectTransform MainPanel; //�D�R�x
    public GameObject TEffectPanel; // �H�J�H�X�S�ĭ��O
    public AVGBackground Background; //�I��

    [Header("�ﶵ")]
    public Transform ChoiceLayer; // �ﶵ�h
    public GameObject ChoiceCover; //�ﶵ�u�X�ɾB��Char�PBg

    [Header("����")]
    public RectTransform LayerChar; //����h
    public GameObject CharPrefab; // ����Prefab

    [Header("��ܮ�")]
    public AVGPortrait Portrait; //�Y��
    public GameObject StoryBox; //��ܮ�
    public StoryBoxName StoryBoxName; //��ܮت����ܪ̦W��
    public StoryBoxContent StoryBoxContent; //��ܮت����ܤ��e
    public GameObject StoryBubble; //��ܮ�w
    public StoryBubbleName StoryBubbleName; //��ܮ�w�����ܪ̦W��
    public StoryBubbleContent StoryBubbleContent; //��ܮ�w�����ܤ��e
    public GameObject StoryCG; //CG���O
    public StoryCGContent StoryCGContent; //CG���O�����ܤ��e

    [Header("�ާ@���O")]
    public Button Btn_Next; //�U�@�B���O(���s)
    public GameObject Toolbar;//�\��C���
    public Toggle ToggleMenu; //�\��C�}��
    public Toggle ToogleAuto; //�۰ʼ���(�J��ﶵ�h�۰ʰ���)
    public Toggle ToogleSkipping; //�۰ʸ��L�wŪ(�J�쥼Ū&�ﶵ�h�۰ʰ���)
    public Button Btn_HideUI; //���ä���(�Ұ�wait hide�A����alpha�k0�A�n�A�I�@�Unext�~�|��_����)
    public Button Btn_Save; //�I�s�O�����O
    public Button Btn_Load; //�I�sŪ�����O

    [Header("LOG���O")]
    public GameObject AVGLoggerModal;
    public LoopScrollView AVGLogger;

    public string ReadlistKey = "AVGReadList"; // PlayerPrefs ���s�� key
    public string CurrentStoryTitle; //�ثe���񤤪��@�����D
    public List<StoryMeta> PendingStories = new();
    public List<StoryMeta> PendingStoriesForSave = new();

    public Coroutine CoroutineStoryQueue;
    public Coroutine CoroutineStory;
    public Coroutine CoroutineStoryCut;
    public Coroutine CoroutineStoryWatingAutoNext;
    public Coroutine CoroutineStoryChoose;
    public Coroutine CoroutineStoryQueueEnd;
    
    private List<Dictionary<string, object>> StoryEventDicts = new();
    public SQLiteManager dbManager;

    private void Start()
    {
        dbManager = new SQLiteManager(Path.Combine(Application.persistentDataPath, "dynamicDatabase.db"));

        // ��l����Panel
        ToggleMenu.isOn = false;
        ChoiceCover.SetActive(false);
        MainPanel.gameObject.SetActive(false);

        // �T�O���e���U�@�B���s�i�ΨòK�[��ť
        if (Btn_Next != null) Btn_Next.onClick.AddListener(OnProcessButtonClicked);

        // �\��C��l��
        if (ToogleAuto != null) ToogleAuto.onValueChanged.AddListener(OnAutoToggleChanged);
        if (ToogleSkipping != null) ToogleSkipping.onValueChanged.AddListener(OnSkippingToggleChanged);
        if (Btn_HideUI != null) Btn_HideUI.onClick.AddListener(OnHideUIButtonClicked);
        //if (Btn_Save != null) Btn_Save.onClick.AddListener(OnSaveButtonClicked);
        //if (Btn_Load != null) Btn_Load.onClick.AddListener(OnLoadButtonClicked);
    }

    public bool DisplayChar = true;
    public bool DisplayPortrait = true;
    public bool DisplayStoryBox = true;
    public bool DisplayBubble = false;
    public bool SingleCharMode = false;
    public bool CGMode = false;
    public bool isAuto = false;
    public bool isSkipping = false;

    private bool isReadyToNext = false;
    private bool isTyping = false;
    private bool isWaiting = false;
    private bool isStoryEnd = false;
    private string lastDisplayName = string.Empty;
    private string curContent = string.Empty;

    private int gotoIndex = -1; // �ﶵ��ܫ�N�e����cutIndex
    public int nextCutIndex; //�U�@�d�����ޭȡA�i�H���~������
    public bool isChoiceSelected = true;

    public IEnumerator Init() //�~���I�s��l�Ƹ��
    {
        yield return null;
        if (ClearReadList) PlayerPrefs.DeleteKey("AVGReadList");
        UpdatePPM("Preset");//��s�w�]��
        Director.Inst.InitAssets();
        yield return new WaitForSeconds(2f);
    }

    public IEnumerator AVGStart(List<StoryMeta> stories)
    {
        PendingStories = stories;
        On();
        yield return null;
        StartCoroutine(Director.Inst.TEffectFadeInWithDelay(1f));//���ݭI��Ū�J��ATEffectFadeIn
        //�Ұ�AVG
        CoroutineStoryQueue = StartCoroutine(StoryQueueStart(() =>
        {
            CoroutineStoryQueueEnd = StartCoroutine(AVGEnd());
        }));
    }

    public IEnumerator AVGEnd()
    {
        Director.Inst.TEffectFadeOut();
        yield return new WaitForSeconds(1f);
        Debug.Log("AVGEnd");
        Off();
    }

    public IEnumerator AVGRestart(List<StoryMeta> stories)
    {
        yield return StartCoroutine(AVGEnd());   // �T�O���� AVGEnd ����
        yield return StartCoroutine(AVGStart(stories)); // �T�O���� AVGStart ����
    }

    public void On()
    {
        if (MainPanel != null) MainPanel.gameObject.SetActive(true);
        TEffectPanel.SetActive(true);
    }

    public void Off()
    {
        // ���m�ƭ�
        isReadyToNext = false;
        isTyping = false;
        isWaiting = false;
        isChoiceSelected = true;
        isStoryEnd = false;

        isAuto = false;
        isSkipping = false;

        ToggleMenu.isOn = false;

        PendingStories.Clear();
        AVGLogger.Clear();

        // �M�ű��bChoiceLayer���U���ﶵ����s
        foreach (Transform child in ChoiceLayer){Destroy(child.gameObject);}

        // �M�� Director �ި����s�A�p Background �P TEffect
        Director.Inst.Off(); 

        // �M��Coroutine
        if (CoroutineStoryQueue != null) StopCoroutine(CoroutineStoryQueue);
        if (CoroutineStory != null) StopCoroutine(CoroutineStory);
        if (CoroutineStoryCut != null) StopCoroutine(CoroutineStoryCut);
        if (CoroutineStoryWatingAutoNext != null) StopCoroutine(CoroutineStoryWatingAutoNext);
        if (CoroutineStoryChoose != null) StopCoroutine(CoroutineStoryChoose);
        if (CoroutineStoryQueueEnd != null) StopCoroutine(CoroutineStoryQueueEnd);

        if (ChoiceCover != null) ChoiceCover.SetActive(false);
        if (MainPanel != null) MainPanel.gameObject.SetActive(false);
        if (SaveLoadPanel != null) SaveLoadPanel.SetActive(false);
        if (AVGLoggerModal != null) AVGLoggerModal.SetActive(false);
    }

    private void OnProcessButtonClicked()
    {
        if (Toolbar.activeSelf)
        {
            CheckIfReadyToNext();
        }
        else
        {
            UIUnhide();
        }
    }

    public void CheckIfReadyToNext()
    {
        isTyping = CheckIfTyping();
        if (!isTyping && !isWaiting && isChoiceSelected)
        {
            isReadyToNext = true;
        }
    }

    private void OnTypingComplete()
    {
        // �ư����I�Ÿ��p�⤤��r��
        int characterCount = curContent.Count(c => char.IsLetterOrDigit(c));
        float watingSec = Math.Clamp((float)characterCount / 4f, 1f, 5f);
        if (isAuto)
        {
            CoroutineStoryWatingAutoNext = StartCoroutine(AutoNext(watingSec));
        }
        else if (isSkipping)
        {
            CoroutineStoryWatingAutoNext = StartCoroutine(AutoNext(0f));
        }
    }

    private IEnumerator AutoNext(float waitingSec)
    {
        yield return new WaitForSeconds(waitingSec);
        CheckIfReadyToNext();
    }

    public IEnumerator StoryQueueStart(Action onComplete)
    {
        while (PendingStories.Count > 0)
        {
            StoryMeta story = PendingStories[0];
            CurrentStoryTitle = story.Title;
            PendingStoriesForSave = PendingStories.ToList(); // ��Save�إ߰ƥ�
            PendingStories.RemoveAt(0);
            Save("Preset", $"AVGSaveSlotAuto"); //Ū�G�ƫe�۰ʦs��
            yield return StartCoroutine(StorySingleStart(story));
            PendingStoriesForSave.RemoveAt(0);
            Save("Preset", $"AVGSaveSlotAuto"); //Ū���G�Ʀ۰ʦs��
        }

        Debug.Log("All stories processed.");
        onComplete?.Invoke(); // ����^��
    }

    /// <summary>
    /// �����@�G��
    /// </summary>
    public IEnumerator StorySingleStart(StoryMeta story, Action onComplete = null)
    {
        // �M����x
        AVGLogger.Clear();
        // �M��
        Director.Inst.CharDestroyAll();

        isStoryEnd = false;
        Debug.Log($"�G�ƶ}�l: {CurrentStoryTitle}");

        CoroutineStory = StartCoroutine(StoryStart<StoryCut>(CurrentStoryTitle));
        yield return CoroutineStory;

        // �Y Once �� "Y"�A�N�G�Ƽ��D�[�J�wŪ�C��
        if (story.Once == "Y") AddTitleToReadList(CurrentStoryTitle);

        Debug.Log($"�G�Ƶ���: {CurrentStoryTitle}");
        onComplete?.Invoke(); // ����^��
    }


    public IEnumerator StoryStart<T>(string StoryTitle) where T : class, new()
    {
        // �N�x��List�নJSON dict List�A���ȸ��F��
        StoryEventDicts.Clear();//JSON dict List���M�ŦA�`�J

        List<T> StoryEvents = dbManager.QueryTable<T>(StoryTitle);
        foreach (var item in StoryEvents)
        {
            var json = JsonConvert.SerializeObject(item);
            var storyCutDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            StoryEventDicts.Add(storyCutDict);
        }
        var startIndex = 0;
        var endIndex = StoryEvents.Count;
        CoroutineStoryCut = StartCoroutine(StoryCutStart(startIndex));
        yield return new WaitUntil(() => isStoryEnd); // ���ݪ���o�ӬG�Ƶ���
    }

    public IEnumerator StoryCutStart(int cutIndex)
    {
        nextCutIndex = cutIndex;//���H�N���

        // ���ݪ��a�ާ@
        isReadyToNext = false;

        //Debug.Log($"StoryCutStart: {cutIndex}");
        var storyCutDict = StoryEventDicts[cutIndex];

        // ��ܷ�ecut�����e
        string charUID = TxR.Inst.Render(ParseEx(storyCutDict["���ܪ�"].ToString()));
        string charPos = TxR.Inst.Render(ParseEx(storyCutDict["��m"].ToString()));
        string charEmo = TxR.Inst.Render(ParseEx(storyCutDict["��"].ToString()));
        string charSimbol = TxR.Inst.Render(ParseEx(storyCutDict["�Ÿ�"].ToString()));
        string charTone = TxR.Inst.Render(ParseEx(storyCutDict["�y��"].ToString()));
        string charEffect = TxR.Inst.Render(ParseEx(storyCutDict["�S��"].ToString()));

        string DisplayName = charUID; //�o�̪�charUID�����O�ѪR�᪺���ܪ̦r��A��DisplayName���w�]��
        // ���w����
        Dictionary<string, string> charData = GetCharDataByUID("Chars", charUID);
        if (charData != null) DisplayName = $"{charData["�m"]}{charData["�W"]}";
        // �W��
        if (HasValidValue(storyCutDict, "��ܦW��"))
        {
            DisplayName = TxR.Inst.Render(ParseEx(storyCutDict["��ܦW��"].ToString()));
        }
        // ���满�ܫe��ƶ�
        if (HasValidValue(storyCutDict, "���ܫe"))
        {
            string[] commands = storyCutDict["���ܫe"].ToString().Split('\n');
            for (int i = 0; i < commands.Length; i++)
            {
                commands[i] = ParseEx(commands[i]);
            }
            Director.Inst.ExecuteActionPackage(commands);
        }
        // ����
        string Content = TxR.Inst.Render(ParseEx(storyCutDict["���ܤ��e"].ToString()));
        // �ѪR����Ÿ�
        Content = Content.Replace("\\n", "\n");
        Debug.Log($"Cut{cutIndex} - {DisplayName}�G{Content}");
        // �O���ثe��աA�H���pauto�Ҧ������ݮɶ�
        curContent = Content;
        // ��ܦW�٨ö}�l����
        StoryCutDisplay(charData, charUID, charPos, charEmo, charSimbol, charTone, charEffect, DisplayName, Content);

        //Debug.Log($"=> �e�����ȡG{storyCutDict["�e��"]}");
        if (HasValidValue(storyCutDict, "�e��"))
        {
            string[] targets = storyCutDict["�e��"].ToString().Split('\n');
            if (HasValidValue(storyCutDict, "�ﶵ"))
            {
                // �οﶵ��callback�^��index
                CoroutineStoryChoose = StartCoroutine(StartChoose(storyCutDict));
                yield return CoroutineStoryChoose;
                nextCutIndex = gotoIndex;
                CheckIfReadyToNext(); //�j��P�_�O�_�i�H����U�@�B
            }
            else
            {
                nextCutIndex = int.Parse(ParseEx(targets[0]));
                //Debug.Log($"���e�����O�S���ﶵ�A���w�e���Ĥ@��index{nextCutIndex}");
            }
        }
        else
        {
            nextCutIndex = cutIndex + 1;
            //Debug.Log($"�e���L�ȡA����+1: �e��{nextCutIndex}");
        }

        //���ܫ᪺��ƥi�H���ܫe����nextCutIndex
        if (HasValidValue(storyCutDict, "���ܫ�"))
        {
            string[] commands = storyCutDict["���ܫ�"].ToString().Split('\n');
            for (int i = 0; i < commands.Length; i++)
            {
                commands[i] = ParseEx(commands[i]);
            }
            Director.Inst.ExecuteActionPackage(commands);
        }

        yield return new WaitUntil(() => isReadyToNext);
        if (nextCutIndex >= StoryEventDicts.Count)
        {
            //Debug.Log("cutIndex�W�X�d��A�G�Ƶ���");
            isStoryEnd = true;
        }
        else
        {
            StartCoroutine(StoryCutStart(nextCutIndex));
        }
    }

    public void StoryCutDisplay(
        Dictionary<string, string> charData,
        string charUID,
        string charPos,
        string charEmo,
        string charSimbol,
        string charTone,
        string charEffect,
        string DisplayName, 
        string Content
        )
    {
        bool isDifferentSayer = DisplayName != lastDisplayName;
        lastDisplayName = DisplayName;

        var imgUrl = Director.Inst.GetPortraitImgUrl(charUID + charEmo);
        // �Ыؤ@�ӷs��log��Ʀr��
        var logData = new Dictionary<string, object>
            {
                { "userName", DisplayName },
                { "message", Content },
                { "imgUrl", imgUrl },
            };
        var name1st = PPM.Inst.Get("�m");
        var name2nd = PPM.Inst.Get("�W");
        var myCurrentName = $"{name1st}{name2nd}";
        AVGLogger.UpdateChatBox(logData, myCurrentName);

        if (CGMode)
        {
            if (!StoryCG.activeSelf) StoryCG.SetActive(true);//���CG���O
            StoryCGContent.Display(Content, isDifferentSayer, OnTypingComplete);
        }
        else
        {
            if (StoryCG.activeSelf) StoryCG.SetActive(false);//����CG���O
            var gbjLayerChar = LayerChar.gameObject;
            var gbjPortrait = Portrait.gameObject;
            if (charData != null) //��������
            {
                bool HasChar = charData["��ø"].ToLower() == "y";
                bool HasPortrait = charData["�Y��"].ToLower() == "y";

                if (DisplayChar)
                {
                    Director.Inst.CharsUnfocusAll(); //��L�����ܶ�
                    if (!gbjLayerChar.activeSelf) gbjLayerChar.SetActive(true);//��ܨ���
                    if (HasChar) Director.Inst.CharIn(charData, charUID, charPos, charEmo, charSimbol); //����i��

                    if (DisplayBubble)
                    {
                        if (!StoryBubble.activeSelf) StoryBubble.SetActive(true);//���bubble
                        StoryBubbleName.Display(DisplayName, isDifferentSayer);
                        StoryBubbleContent.Display(Content, isDifferentSayer, OnTypingComplete);
                    }
                    else
                    {
                        if (StoryBubble.activeSelf) StoryBubble.SetActive(false);//����bubble
                    }
                }
                else
                {
                    if (gbjLayerChar.activeSelf) gbjLayerChar.SetActive(false);//���è���
                }
                if (DisplayPortrait && HasPortrait) //���Y�ϥB���\�Y�ϡA�~����Y��
                {
                    if (!gbjPortrait.activeSelf) gbjPortrait.SetActive(true);//����Y��
                    Director.Inst.PortraitIn(charUID, charEmo);
                }
                else
                {
                    if (gbjPortrait.activeSelf) gbjPortrait.SetActive(false);//�����Y��
                }
            }
            else //�L�����ơA�ǥթΥD���o��
            {
                Director.Inst.CharsUnfocusAll(); //��L�����ܶ�
                if (gbjPortrait.activeSelf) gbjPortrait.SetActive(false);//�����Y��
                if (StoryBubble.activeSelf) StoryBubble.SetActive(false);//����bubble
            }
            if (DisplayStoryBox)
            {
                if (!StoryBox.activeSelf) StoryBox.SetActive(true);//���box
                StoryBoxName.Display(DisplayName, isDifferentSayer);
                StoryBoxContent.Display(Content, isDifferentSayer, OnTypingComplete);
            }
            else
            {
                if (StoryBox.activeSelf) StoryBox.SetActive(false);//����box
            }
        }
    }

    private bool HasValidValue(Dictionary<string, object> dict, string key)
    {
        return dict.ContainsKey(key) && dict[key] != null && !string.IsNullOrWhiteSpace(dict[key].ToString());
    }

    public bool CheckIfTyping() 
    {
        bool boxIsTyping = StoryBoxContent.IsTyping();
        bool bubbleIsTyping = StoryBubbleContent.IsTyping();
        bool cgIsTyping = StoryCGContent.IsTyping();
        if (boxIsTyping || bubbleIsTyping || cgIsTyping) 
        {
            if (StoryBoxContent.IsTyping()) StoryBoxContent.SkipTyping();
            if (StoryBubbleContent.IsTyping()) StoryBubbleContent.SkipTyping();
            if (StoryCGContent.IsTyping()) StoryCGContent.SkipTyping();
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// �ѪR����B��l�r��ê�^���G�C
    /// </summary>
    /// <param name="expression">����B��l�r��A�Ҧp "����>=�J�|����?3:8"</param>
    /// <returns>�ѪR�çP�_�᪺���G</returns>
    public static string ParseEx(string expression)
    {
        if (string.IsNullOrEmpty(expression))
        {
            //Debug.LogError("��F�����ũ� null");
            return string.Empty;
        }

        try
        {
            expression = expression.Trim();
            // ������B��l����m
            int questionMarkIndex = expression.IndexOf('?');
            int colonIndex = expression.IndexOf(':');

            // �ˬd�榡�O�_���T
            if (questionMarkIndex == -1 || colonIndex == -1 || questionMarkIndex > colonIndex)
            {
                //Debug.Log($"�D�T�����G{expression}�A�^�ǭȬ�{expression}");
                if (!expression.StartsWith("\"") || !expression.EndsWith("\""))
                {
                    return PPM.Inst.Get(expression, expression);
                }
                else
                {
                    return expression;
                }
            }

            // �ѪR����B���u���G�B�������G
            string condition = expression.Substring(0, questionMarkIndex).Trim();
            string trueResult = expression.Substring(questionMarkIndex + 1, colonIndex - questionMarkIndex - 1).Trim();
            string falseResult = expression.Substring(colonIndex + 1).Trim();

            // �ϥ� Judge.EvaluateCondition �P�_����O�_����
            bool conditionResult = Judge.EvaluateSingleCondition(condition);

            // �ھڱ��󵲪G��^��������
            string result = conditionResult ? trueResult : falseResult;
            Debug.Log($"�T�������G{expression}�A�^�ǭȬ�{result}");
            return result;
        }
        catch (Exception ex)
        {
            Debug.LogError($"�ѪR�ΧP�_��F���ɵo�Ϳ��~: {ex.Message}");
            return string.Empty;
        }
    }
}
