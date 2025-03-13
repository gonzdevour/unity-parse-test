using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public partial class Typing : MonoBehaviour
{
    public Text uiText; // UI Text (Legacy) 元件
    public float DefaultTypingInterval = 0.05f; // 每個字的顯示間隔
    public GameObject MarkNext;
    public AudioSource typingSound; // 短音效
                                    
    private Dictionary<string, Action<object[]>> typingActions; // 定義action字典
    private string[] typingTriggers; // 記錄要在第幾個字觸發什麼action
    private Coroutine typingCoroutine;
    private bool isTyping; // 是否正在進行打字機效果
    private string fullMessage; //目前全文
    private Action cbk;

    private float action_waitSec; //action控制的字間等待時間

    // 初始化字間效果
    private void Start()
    {
        // 初始化字典並綁定函數
        typingActions = new Dictionary<string, Action<object[]>>
        {
            { "wait", args => Wait(args) },
        };
    }

    private void Wait(object[] args)
    {
        float sec = float.Parse(args[0]?.ToString());
        action_waitSec = sec;
        Debug.Log($"[TypingAction]Wait:{sec}sec");
    }

    // 開始打字機效果
    public void StartTyping(string message, float typingSpeed = -1f, Action onComplete = null)
    {
        typingTriggers = GDCodeReindex(message); // 字間控制初始化，記錄在第幾個字的時候執行效果
        message = Regex.Replace(message, @"\[[^\]]+\]", ""); // 移除所有GDCode的 [tag]
        //Debug.Log($"[GDCodedMsg]{message}");

        if (onComplete != null) cbk = onComplete;
        fullMessage = message;
        if (Director.Inst != null) DefaultTypingInterval = Director.Inst.DefaultTypingInterval;
        if (typingSpeed == -1f) typingSpeed = DefaultTypingInterval;
        if (AVG.Inst.isSkipping) typingSpeed = 0f;
        //if (typingSpeed == -1f) typingSpeed = 1f;
        //Debug.Log($"typingSpeed: {typingSpeed}");
        // 停止之前的協程（如果有）
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // 開始新的協程
        MarkNext.SetActive(false); // 隱藏繼續箭頭
        typingCoroutine = StartCoroutine(TypeText(message, typingSpeed));
    }

    // 跳過打字機效果並直接顯示全文
    public void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        MarkNext.SetActive(true); // 顯示繼續箭頭
        uiText.text = fullMessage; // 直接顯示完整文本
        Debug.Log($"[SkipTyping]{fullMessage}");
        isTyping = false;
        cbk?.Invoke();
        cbk = null;
    }

    // 協程：逐字顯示文本
    private IEnumerator TypeText(string message, float typingSpeed = 0.05f)
    {
        List<string> richCharList = ParseRichTextCharacters(message);

        isTyping = true;
        uiText.text = ""; // 清空文本
        int charIdx = -1;
        float typingSpeedWithWait = 0;
        foreach (string currentChar in richCharList)
        {
            charIdx++;
            //初始化action系列變數
            action_waitSec = 0f;
            //執行action
            TypingExecuteAction(charIdx);
            //增加字間等待時間(如果有)
            typingSpeedWithWait = Math.Max(0, typingSpeed + action_waitSec);

            //Debug.Log(currentChar);

            // 添加當前字元到文本
            uiText.text += currentChar;

            // 播放短音效
            if (typingSound != null)
            {
                typingSound.Play();
            }

            // 檢查是否加速打字
            float currentSpeed = Input.GetKey(KeyCode.Space) ? typingSpeedWithWait / 5 : typingSpeedWithWait;

            // 等待當前字符的打字間隔
            yield return new WaitForSeconds(currentSpeed);
        }
        // 顯示繼續箭頭
        MarkNext.SetActive(true);
        // 確保最終顯示完整文本
        uiText.text = message;
        // 打字機完成
        isTyping = false;
        typingCoroutine = null;
        cbk?.Invoke();
        cbk = null;
    }

    // 檢查打字機效果是否正在運行
    public bool IsTyping()
    {
        return isTyping;
    }

    // 執行字間特效
    public void TypingExecuteAction(int idx)
    {
        string[] parts = GDCodeParse(typingTriggers[idx]);
        //Debug.Log($"[TypingExecuteAction]{string.Concat(parts)}");
        if (parts.Length >= 1 && typingActions.ContainsKey(parts[0]))
        {
            // 提取動作名稱和參數
            string actionName = parts[0];
            object[] parameters = parts[1..]; // 提取剩下的參數

            // 執行對應的函數
            typingActions[actionName](parameters);
        }
    }
}
