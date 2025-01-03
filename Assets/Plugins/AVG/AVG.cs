using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using DG.Tweening;

public class AVG : MonoBehaviour
{
    public static AVG Inst { get; private set; }
    private void Awake()
    {
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public GameObject choicePanel; // 選擇面板
    public GameObject choicePrefab; // 選項按鈕Prefab
    public Button Btn_Next; //下一步面板(按鈕)
    public List<string> PendingStoryTitles;
    private List<Dictionary<string, object>> StoryEventDicts = new();
    SQLiteManager dbManager;

    private void Start()
    {
        dbManager = new SQLiteManager(Path.Combine(Application.persistentDataPath, "dynamicDatabase.db"));
        // 隱藏choicePanel
        choicePanel.SetActive(false);
        // 確保按鈕可用並添加監聽
        if (Btn_Next != null)
        {
            Btn_Next.onClick.AddListener(OnProcessButtonClicked);
        }
    }

    private bool isReadyToNext = false;
    private bool isTyping = false;
    private bool isWaiting = false;
    private bool isChoiceSelected = true;
    private bool isStoryEnd = false;
    private int gotoIndex = -1; // 選項選擇後將前往的cutIndex

    private void OnProcessButtonClicked()
    {
        CheckIfReadyToNext();
    }

    private void CheckIfReadyToNext()
    {
        if (!isTyping && !isWaiting && isChoiceSelected)
        {
            isReadyToNext = true;
        }
    }

    public IEnumerator StoryQueueStart<T>(Action onComplete) where T : class, new()
    {
        yield return null;
        while (PendingStoryTitles.Count > 0)
        {
            // 取出並移除第一個元素
            string currentTitle = PendingStoryTitles[0];
            PendingStoryTitles.RemoveAt(0);
            
            isStoryEnd = false;
            Debug.Log($"故事開始: {currentTitle}");
            yield return StoryStart<T>(currentTitle);
            Debug.Log($"故事已讀: {currentTitle}");
        }

        Debug.Log("All stories processed.");
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
        StartCoroutine(StoryCutStart(startIndex));
        yield return new WaitUntil(() => isStoryEnd); // 等待直到這個故事結束
    }

    public IEnumerator StoryCutStart(int cutIndex)
    {
        int nextCutIndex = cutIndex;//先隨意賦值

        // 等待玩家操作
        isReadyToNext = false;

        //Debug.Log($"StoryCutStart: {cutIndex}");
        var storyCutDict = StoryEventDicts[cutIndex];
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
        // 顯示當前cut的內容
        string Name = TxR.Inst.Render(ParseEx(storyCutDict["說話者"].ToString()));
        string DisplayName = Name;
        if (HasValidValue(storyCutDict, "顯示名稱"))
        {
            DisplayName = TxR.Inst.Render(ParseEx(storyCutDict["顯示名稱"].ToString()));
        }
        string Content = TxR.Inst.Render(ParseEx(storyCutDict["說話內容"].ToString()));
        Debug.Log($"Cut{cutIndex} - {DisplayName}：{Content}");

        if (HasValidValue(storyCutDict, "說話後"))
        {
            string[] commands = storyCutDict["說話後"].ToString().Split('\n');
            for (int i = 0; i < commands.Length; i++)
            {
                commands[i] = ParseEx(commands[i]);
            }
            Director.Inst.ExecuteActionPackage(commands);
        }

        //Debug.Log($"=> 前往的值：{storyCutDict["前往"]}");
        if (HasValidValue(storyCutDict, "前往"))
        {
            string[] targets = storyCutDict["前往"].ToString().Split('\n');
            if (HasValidValue(storyCutDict, "選項"))
            {
                // 用選項的callback回傳index
                yield return StartChoose(storyCutDict);
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

    IEnumerator StartChoose(Dictionary<string, object> storyCutDict)
    {
        // 初始化面板
        choicePanel.SetActive(true);
        ClearExistingButtons();

        // 初始化選擇狀態
        isChoiceSelected = false;

        string[] targets = storyCutDict["前往"].ToString().Split('\n');
        string[] options = storyCutDict["選項"].ToString().Split('\n');
        List<GameObject> buttons = new List<GameObject>();

        for (int i = 0; i < options.Length && i < targets.Length; i++)
        {
            // 創建按鈕
            GameObject button = Instantiate(choicePrefab, choicePanel.transform);
            button.GetComponentInChildren<Text>().text = TxR.Inst.Render(ParseEx(options[i])); // 設置按鈕文字
            int resultCutIndex = int.Parse(ParseEx(targets[i]));

            // 設置按鈕回調
            Button btn = button.GetComponent<Button>();
            btn.onClick.AddListener(() => OnChoiceSelected(resultCutIndex, button));

            // 按鈕飛入動畫
            //button.transform.localPosition = new Vector3(0, 500, 0); // 初始位置
            //button.transform.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutBounce); // 飛入動畫
            buttons.Add(button);
        }

        // 等待選擇完成
        yield return new WaitUntil(() => isChoiceSelected);

        // 按鈕飛出動畫
        foreach (var button in buttons)
        {
            button.transform.DOLocalMoveY(500, 0.5f).SetEase(Ease.InBack)
                .OnComplete(() => Destroy(button)); // 動畫完成後銷毀按鈕
        }

        // 等待飛出動畫完成
        yield return new WaitForSeconds(0.5f);

        // 隱藏面板
        choicePanel.SetActive(false);
    }

    private void ClearExistingButtons()
    {
        foreach (Transform child in choicePanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnChoiceSelected(int resultIndex, GameObject button)
    {
        gotoIndex = resultIndex;
        isChoiceSelected = true;
        Debug.Log($"choose to go to cut: {gotoIndex}");
    }

    private bool HasValidValue(Dictionary<string, object> dict, string key)
    {
        return dict.ContainsKey(key) && dict[key] != null && !string.IsNullOrWhiteSpace(dict[key].ToString());
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
            Debug.LogError("表達式為空或 null");
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
