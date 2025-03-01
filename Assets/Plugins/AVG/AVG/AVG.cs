using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

// 引入關於Story的資料結構(如BgData、CharData等容易重名的class)
using Story;
// 引入LSR以製作Logger
using LSR;

public partial class AVG : MonoBehaviour
{
    public static AVG Inst { get; private set; }
    private void Awake()
    {
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    [Header("記錄")]
    public bool ClearReadList = true; //初始化時清除已讀標題
    public GameObject SaveLoadPanel;

    [Header("舞台")]
    public RectTransform MainPanel; //主舞台
    public GameObject TEffectPanel; // 淡入淡出特效面板
    public AVGBackground Background; //背景

    [Header("選項")]
    public Transform ChoiceLayer; // 選項層
    public GameObject ChoiceCover; //選項彈出時遮住Char與Bg

    [Header("角色")]
    public RectTransform LayerChar; //角色層
    public GameObject CharPrefab; // 角色Prefab

    [Header("對話框")]
    public AVGPortrait Portrait; //頭像
    public GameObject StoryBox; //對話框
    public StoryBoxName StoryBoxName; //對話框的說話者名稱
    public StoryBoxContent StoryBoxContent; //對話框的說話內容
    public GameObject StoryBubble; //對話氣泡
    public StoryBubbleName StoryBubbleName; //對話氣泡的說話者名稱
    public StoryBubbleContent StoryBubbleContent; //對話氣泡的說話內容
    public GameObject StoryCG; //CG面板
    public StoryCGContent StoryCGContent; //CG面板的說話內容

    [Header("操作面板")]
    public Button Btn_Next; //下一步面板(按鈕)
    public GameObject Toolbar;//功能列選單
    public Toggle ToggleMenu; //功能列開關
    public Toggle ToogleAuto; //自動播放(遇到選項則自動停止)
    public Toggle ToogleSkipping; //自動跳過已讀(遇到未讀&選項則自動停止)
    public Button Btn_HideUI; //隱藏介面(啟動wait hide，介面alpha歸0，要再點一下next才會恢復介面)
    public Button Btn_Save; //呼叫記錄面板
    public Button Btn_Load; //呼叫讀取面板

    [Header("LOG面板")]
    public GameObject AVGLoggerModal;
    public LoopScrollView AVGLogger;

    public string ReadlistKey = "AVGReadList"; // PlayerPrefs 中存放的 key
    public string CurrentStoryTitle; //目前播放中的劇本標題
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

        // 初始隱藏Panel
        ToggleMenu.isOn = false;
        ChoiceCover.SetActive(false);
        MainPanel.gameObject.SetActive(false);

        // 確保全畫面下一步按鈕可用並添加監聽
        if (Btn_Next != null) Btn_Next.onClick.AddListener(OnProcessButtonClicked);

        // 功能列初始化
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

    private int gotoIndex = -1; // 選項選擇後將前往的cutIndex
    public int nextCutIndex; //下一卡的索引值，可以讓外部控制
    public bool isChoiceSelected = true;

    public IEnumerator Init() //外部呼叫初始化資料
    {
        yield return null;
        if (ClearReadList) PlayerPrefs.DeleteKey("AVGReadList");
        UpdatePPM("Preset");//更新預設值
        Director.Inst.InitAssets();
        yield return new WaitForSeconds(2f);
    }

    public IEnumerator AVGStart(List<StoryMeta> stories)
    {
        PendingStories = stories;
        On();
        yield return null;
        StartCoroutine(Director.Inst.TEffectFadeInWithDelay(1f));//等待背景讀入後再TEffectFadeIn
        //啟動AVG
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
        yield return StartCoroutine(AVGEnd());   // 確保等待 AVGEnd 完成
        yield return StartCoroutine(AVGStart(stories)); // 確保等待 AVGStart 執行
    }

    public void On()
    {
        if (MainPanel != null) MainPanel.gameObject.SetActive(true);
        TEffectPanel.SetActive(true);
    }

    public void Off()
    {
        // 重置數值
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

        // 清空掛在ChoiceLayer底下的選項物件群
        foreach (Transform child in ChoiceLayer){Destroy(child.gameObject);}

        // 清空 Director 管制的物件群，如 Background 與 TEffect
        Director.Inst.Off(); 

        // 清空Coroutine
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
        // 排除標點符號計算中文字數
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
            PendingStoriesForSave = PendingStories.ToList(); // 為Save建立副本
            PendingStories.RemoveAt(0);
            Save("Preset", $"AVGSaveSlotAuto"); //讀故事前自動存檔
            yield return StartCoroutine(StorySingleStart(story));
            PendingStoriesForSave.RemoveAt(0);
            Save("Preset", $"AVGSaveSlotAuto"); //讀完故事自動存檔
        }

        Debug.Log("All stories processed.");
        onComplete?.Invoke(); // 執行回調
    }

    /// <summary>
    /// 播放單一故事
    /// </summary>
    public IEnumerator StorySingleStart(StoryMeta story, Action onComplete = null)
    {
        // 清除日誌
        AVGLogger.Clear();
        // 清場
        Director.Inst.CharDestroyAll();

        isStoryEnd = false;
        Debug.Log($"故事開始: {CurrentStoryTitle}");

        CoroutineStory = StartCoroutine(StoryStart<StoryCut>(CurrentStoryTitle));
        yield return CoroutineStory;

        // 若 Once 為 "Y"，將故事標題加入已讀列表
        if (story.Once == "Y") AddTitleToReadList(CurrentStoryTitle);

        Debug.Log($"故事結束: {CurrentStoryTitle}");
        onComplete?.Invoke(); // 執行回調
    }


    public IEnumerator StoryStart<T>(string StoryTitle) where T : class, new()
    {
        // 將泛型List轉成JSON dict List，取值較靈活
        StoryEventDicts.Clear();//JSON dict List先清空再注入

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
        yield return new WaitUntil(() => isStoryEnd); // 等待直到這個故事結束
    }

    public IEnumerator StoryCutStart(int cutIndex)
    {
        nextCutIndex = cutIndex;//先隨意賦值

        // 等待玩家操作
        isReadyToNext = false;

        //Debug.Log($"StoryCutStart: {cutIndex}");
        var storyCutDict = StoryEventDicts[cutIndex];

        // 顯示當前cut的內容
        string charUID = TxR.Inst.Render(ParseEx(storyCutDict["說話者"].ToString()));
        string charPos = TxR.Inst.Render(ParseEx(storyCutDict["位置"].ToString()));
        string charEmo = TxR.Inst.Render(ParseEx(storyCutDict["表情"].ToString()));
        string charSimbol = TxR.Inst.Render(ParseEx(storyCutDict["符號"].ToString()));
        string charTone = TxR.Inst.Render(ParseEx(storyCutDict["語氣"].ToString()));
        string charEffect = TxR.Inst.Render(ParseEx(storyCutDict["特效"].ToString()));

        string DisplayName = charUID; //這裡的charUID指的是解析後的說話者字串，為DisplayName的預設值
        // 指定角色
        Dictionary<string, string> charData = GetCharDataByUID("Chars", charUID);
        if (charData != null) DisplayName = $"{charData["姓"]}{charData["名"]}";
        // 名稱
        if (HasValidValue(storyCutDict, "顯示名稱"))
        {
            DisplayName = TxR.Inst.Render(ParseEx(storyCutDict["顯示名稱"].ToString()));
        }
        // 執行說話前函數集
        if (HasValidValue(storyCutDict, "說話前"))
        {
            string[] commands = storyCutDict["說話前"].ToString().Split('\n');
            for (int i = 0; i < commands.Length; i++)
            {
                commands[i] = ParseEx(commands[i]);
            }
            Director.Inst.ExecuteActionPackage(commands);
        }
        // 說話
        string Content = TxR.Inst.Render(ParseEx(storyCutDict["說話內容"].ToString()));
        // 解析換行符號
        Content = Content.Replace("\\n", "\n");
        Debug.Log($"Cut{cutIndex} - {DisplayName}：{Content}");
        // 記錄目前對白，以估計auto模式的等待時間
        curContent = Content;
        // 顯示名稱並開始說話
        StoryCutDisplay(charData, charUID, charPos, charEmo, charSimbol, charTone, charEffect, DisplayName, Content);

        //Debug.Log($"=> 前往的值：{storyCutDict["前往"]}");
        if (HasValidValue(storyCutDict, "前往"))
        {
            string[] targets = storyCutDict["前往"].ToString().Split('\n');
            if (HasValidValue(storyCutDict, "選項"))
            {
                // 用選項的callback回傳index
                CoroutineStoryChoose = StartCoroutine(StartChoose(storyCutDict));
                yield return CoroutineStoryChoose;
                nextCutIndex = gotoIndex;
                CheckIfReadyToNext(); //強制判斷是否可以執行下一步
            }
            else
            {
                nextCutIndex = int.Parse(ParseEx(targets[0]));
                //Debug.Log($"有前往但是沒有選項，指定前往第一個index{nextCutIndex}");
            }
        }
        else
        {
            nextCutIndex = cutIndex + 1;
            //Debug.Log($"前往無值，直接+1: 前往{nextCutIndex}");
        }

        //說話後的函數可以改變前往的nextCutIndex
        if (HasValidValue(storyCutDict, "說話後"))
        {
            string[] commands = storyCutDict["說話後"].ToString().Split('\n');
            for (int i = 0; i < commands.Length; i++)
            {
                commands[i] = ParseEx(commands[i]);
            }
            Director.Inst.ExecuteActionPackage(commands);
        }

        yield return new WaitUntil(() => isReadyToNext);
        if (nextCutIndex >= StoryEventDicts.Count)
        {
            //Debug.Log("cutIndex超出範圍，故事結束");
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
        // 創建一個新的log資料字典
        var logData = new Dictionary<string, object>
            {
                { "userName", DisplayName },
                { "message", Content },
                { "imgUrl", imgUrl },
            };
        var name1st = PPM.Inst.Get("姓");
        var name2nd = PPM.Inst.Get("名");
        var myCurrentName = $"{name1st}{name2nd}";
        AVGLogger.UpdateChatBox(logData, myCurrentName);

        if (CGMode)
        {
            if (!StoryCG.activeSelf) StoryCG.SetActive(true);//顯示CG面板
            StoryCGContent.Display(Content, isDifferentSayer, OnTypingComplete);
        }
        else
        {
            if (StoryCG.activeSelf) StoryCG.SetActive(false);//隱藏CG面板
            var gbjLayerChar = LayerChar.gameObject;
            var gbjPortrait = Portrait.gameObject;
            if (charData != null) //有角色資料
            {
                bool HasChar = charData["立繪"].ToLower() == "y";
                bool HasPortrait = charData["頭圖"].ToLower() == "y";

                if (DisplayChar)
                {
                    Director.Inst.CharsUnfocusAll(); //其他角色變黑
                    if (!gbjLayerChar.activeSelf) gbjLayerChar.SetActive(true);//顯示角色
                    if (HasChar) Director.Inst.CharIn(charData, charUID, charPos, charEmo, charSimbol); //角色進場

                    if (DisplayBubble)
                    {
                        if (!StoryBubble.activeSelf) StoryBubble.SetActive(true);//顯示bubble
                        StoryBubbleName.Display(DisplayName, isDifferentSayer);
                        StoryBubbleContent.Display(Content, isDifferentSayer, OnTypingComplete);
                    }
                    else
                    {
                        if (StoryBubble.activeSelf) StoryBubble.SetActive(false);//隱藏bubble
                    }
                }
                else
                {
                    if (gbjLayerChar.activeSelf) gbjLayerChar.SetActive(false);//隱藏角色
                }
                if (DisplayPortrait && HasPortrait) //有頭圖且允許頭圖，才顯示頭圖
                {
                    if (!gbjPortrait.activeSelf) gbjPortrait.SetActive(true);//顯示頭圖
                    Director.Inst.PortraitIn(charUID, charEmo);
                }
                else
                {
                    if (gbjPortrait.activeSelf) gbjPortrait.SetActive(false);//隱藏頭圖
                }
            }
            else //無角色資料，旁白或主角發言
            {
                Director.Inst.CharsUnfocusAll(); //其他角色變黑
                if (gbjPortrait.activeSelf) gbjPortrait.SetActive(false);//隱藏頭圖
                if (StoryBubble.activeSelf) StoryBubble.SetActive(false);//隱藏bubble
            }
            if (DisplayStoryBox)
            {
                if (!StoryBox.activeSelf) StoryBox.SetActive(true);//顯示box
                StoryBoxName.Display(DisplayName, isDifferentSayer);
                StoryBoxContent.Display(Content, isDifferentSayer, OnTypingComplete);
            }
            else
            {
                if (StoryBox.activeSelf) StoryBox.SetActive(false);//隱藏box
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
    /// 解析條件運算子字串並返回結果。
    /// </summary>
    /// <param name="expression">條件運算子字串，例如 "金錢>=蛋糕價格?3:8"</param>
    /// <returns>解析並判斷後的結果</returns>
    public static string ParseEx(string expression)
    {
        if (string.IsNullOrEmpty(expression))
        {
            //Debug.LogError("表達式為空或 null");
            return string.Empty;
        }

        try
        {
            expression = expression.Trim();
            // 找到條件運算子的位置
            int questionMarkIndex = expression.IndexOf('?');
            int colonIndex = expression.IndexOf(':');

            // 檢查格式是否正確
            if (questionMarkIndex == -1 || colonIndex == -1 || questionMarkIndex > colonIndex)
            {
                //Debug.Log($"非三元式：{expression}，回傳值為{expression}");
                if (!expression.StartsWith("\"") || !expression.EndsWith("\""))
                {
                    return PPM.Inst.Get(expression, expression);
                }
                else
                {
                    return expression;
                }
            }

            // 解析條件、為真結果、為假結果
            string condition = expression.Substring(0, questionMarkIndex).Trim();
            string trueResult = expression.Substring(questionMarkIndex + 1, colonIndex - questionMarkIndex - 1).Trim();
            string falseResult = expression.Substring(colonIndex + 1).Trim();

            // 使用 Judge.EvaluateCondition 判斷條件是否成立
            bool conditionResult = Judge.EvaluateSingleCondition(condition);

            // 根據條件結果返回相應的值
            string result = conditionResult ? trueResult : falseResult;
            Debug.Log($"三元式為：{expression}，回傳值為{result}");
            return result;
        }
        catch (Exception ex)
        {
            Debug.LogError($"解析或判斷表達式時發生錯誤: {ex.Message}");
            return string.Empty;
        }
    }
}
