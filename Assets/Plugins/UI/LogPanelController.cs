using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogPanelController : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect; // ScrollView �� ScrollRect
    [SerializeField] private Text logText;          // ��ܰT���� Text
    [SerializeField] private Slider pageSlider;     // �Ω󱱨�ƪ���V Slider
    [SerializeField] private int maxCharactersPerPage = 2000; // �C���̤j�r��

    private System.Text.StringBuilder logBuilder;   // �x�s�Ҧ���x�� StringBuilder
    private List<string> logPages;                  // �����s�x����x
    private int currentPage = 0;                    // ��e���X

    public static LogPanelController Inst { get; private set; }
    private void Awake()
    {
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        logBuilder = new System.Text.StringBuilder();
        logPages = new List<string>();
        Application.logMessageReceived += HandleLog; // ���U��x�^�I

        // ��l�� Slider
        pageSlider.onValueChanged.AddListener(OnPageChanged);
        UpdateSlider();
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog; // �Ѱ����U
        pageSlider.onValueChanged.RemoveListener(OnPageChanged);
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // �N�s��x�l�[�� logBuilder
        logBuilder.AppendLine(logString);
        // ���s�ͦ��������e
        GeneratePages();
        // ������̷s�@��
        currentPage = logPages.Count - 1;
        // ��s Slider��value�PmaxValue
        UpdateSlider();
        // ��s��r���e�í��]����
        UpdateLogText();
        // �u�ʨ�̩���
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
        // ��s��r���e�í��]����
        UpdateLogText();
        // �u�ʨ�̩���
        StartCoroutine(RefreshScrollView());
    }

    private void AdjustLogTextHeight()
    {
        // �p��奻�����ﰪ��
        float preferredHeight = logText.preferredHeight;

        // ��s RectTransform ������
        RectTransform logRectTransform = logText.rectTransform;
        logRectTransform.sizeDelta = new Vector2(logRectTransform.sizeDelta.x, preferredHeight);
    }

    private IEnumerator RefreshScrollView()
    {
        yield return null; // ���ݤ@�V
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0; // �u�ʨ�̩���

    }
}
