using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Typing : MonoBehaviour
{
    public Text uiText; // UI Text (Legacy) 元件
    public float DefaultTypingInterval = 0.05f; // 每個字的顯示間隔
    public AudioSource typingSound; // 短音效
    public bool richTextSupport = true; // 是否支援富文本標籤

    private Coroutine typingCoroutine;
    private bool isTyping; // 是否正在進行打字機效果
    private string fullMessage; //目前全文
    private Action cbk;

    // 開始打字機效果
    public void StartTyping(string message, float typingSpeed = -1f, Action onComplete = null)
    {
        if (onComplete != null) cbk = onComplete;
        fullMessage = message;
        if (Director.Inst != null) DefaultTypingInterval = Director.Inst.DefaultTypingInterval;
        if (typingSpeed == -1f) typingSpeed = DefaultTypingInterval;
        //if (typingSpeed == -1f) typingSpeed = 1f;
        //Debug.Log($"typingSpeed: {typingSpeed}");
        // 停止之前的協程（如果有）
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // 開始新的協程
        typingCoroutine = StartCoroutine(TypeText(message, typingSpeed));
    }

    // 跳過打字機效果並直接顯示全文
    public void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        uiText.text = fullMessage; // 直接顯示完整文本
        isTyping = false;
        cbk?.Invoke();
        cbk = null;
    }

    // 協程：逐字顯示文本
    private IEnumerator TypeText(string message, float typingSpeed = 0.05f)
    {
        isTyping = true;
        uiText.text = ""; // 清空文本
        int tagOpen = 0; // 富文本標籤計數（僅當 richTextSupport 為 true 時使用）

        for (int i = 0; i < message.Length; i++)
        {
            char currentChar = message[i];

            // 處理富文本標籤
            if (richTextSupport)
            {
                if (currentChar == '<') tagOpen++;
                if (currentChar == '>') tagOpen--;

                // 當富文本標籤未結束時，直接添加字符並跳過延遲
                if (tagOpen > 0 || currentChar == '>')
                {
                    uiText.text += currentChar;
                    continue;
                }
            }

            // 添加當前字元到文本
            uiText.text += currentChar;

            // 播放短音效
            if (typingSound != null)
            {
                typingSound.Play();
            }

            // 檢查是否加速打字
            float currentSpeed = Input.GetKey(KeyCode.Space) ? typingSpeed / 5 : typingSpeed;

            // 等待當前字符的打字間隔
            yield return new WaitForSeconds(currentSpeed);
        }

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
}
