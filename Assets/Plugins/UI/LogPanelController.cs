using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogPanelController : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect; // ScrollView 的 ScrollRect
    [SerializeField] private Text logText;          // 顯示訊息的 Text
    [SerializeField] private Slider pageSlider;     // 用於控制頁數的橫向 Slider
    [SerializeField] private int maxCharactersPerPage = 2000; // 每頁最大字數

    private System.Text.StringBuilder logBuilder;   // 儲存所有日誌的 StringBuilder
    private List<string> logPages;                  // 分頁存儲的日誌
    private int currentPage = 0;                    // 當前頁碼

    public static LogPanelController Inst { get; private set; }
    private void Awake()
    {
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        logBuilder = new System.Text.StringBuilder();
        logPages = new List<string>();
        Application.logMessageReceived += HandleLog; // 註冊日誌回呼

        // 初始化 Slider
        pageSlider.onValueChanged.AddListener(OnPageChanged);
        UpdateSlider();
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog; // 解除註冊
        pageSlider.onValueChanged.RemoveListener(OnPageChanged);
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // 將新日誌追加到 logBuilder
        logBuilder.AppendLine(logString);
        // 重新生成分頁內容
        GeneratePages();
        // 切換到最新一頁
        currentPage = logPages.Count - 1;
        // 更新 Slider的value與maxValue
        UpdateSlider();
        // 更新文字內容並重設高度
        UpdateLogText();
        // 滾動到最底部
        StartCoroutine(RefreshScrollView());
    }

    private void GeneratePages()
    {
        logPages.Clear();
        string fullLog = logBuilder.ToString();

        for (int i = 0; i < fullLog.Length; i += maxCharactersPerPage)
        {
            int length = Mathf.Min(maxCharactersPerPage, fullLog.Length - i);
            logPages.Add(fullLog.Substring(i, length));
        }
    }

    private void UpdateLogText()
    {
        if (logPages.Count > 0 && currentPage >= 0 && currentPage < logPages.Count)
        {
            logText.text = logPages[currentPage];
        }
        else
        {
            logText.text = string.Empty;
        }
        AdjustLogTextHeight();
    }

    private void UpdateSlider()
    {
        pageSlider.maxValue = Mathf.Max(0, logPages.Count - 1);
        pageSlider.value = currentPage;
    }

    private void OnPageChanged(float value)
    {
        currentPage = Mathf.RoundToInt(value);
        // 更新文字內容並重設高度
        UpdateLogText();
        // 滾動到最底部
        StartCoroutine(RefreshScrollView());
    }

    private void AdjustLogTextHeight()
    {
        // 計算文本的首選高度
        float preferredHeight = logText.preferredHeight;

        // 更新 RectTransform 的高度
        RectTransform logRectTransform = logText.rectTransform;
        logRectTransform.sizeDelta = new Vector2(logRectTransform.sizeDelta.x, preferredHeight);
    }

    private IEnumerator RefreshScrollView()
    {
        yield return null; // 等待一幀
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0; // 滾動到最底部

    }
}
