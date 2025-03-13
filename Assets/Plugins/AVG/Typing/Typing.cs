using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public partial class Typing : MonoBehaviour
{
    public Text uiText; // UI Text (Legacy) ����
    public float DefaultTypingInterval = 0.05f; // �C�Ӧr����ܶ��j
    public GameObject MarkNext;
    public AudioSource typingSound; // �u����
                                    
    private Dictionary<string, Action<object[]>> typingActions; // �w�qaction�r��
    private string[] typingTriggers; // �O���n�b�ĴX�ӦrĲ�o����action
    private Coroutine typingCoroutine;
    private bool isTyping; // �O�_���b�i�楴�r���ĪG
    private string fullMessage; //�ثe����
    private Action cbk;

    private float action_waitSec; //action����r�����ݮɶ�

    // ��l�Ʀr���ĪG
    private void Start()
    {
        // ��l�Ʀr��øj�w���
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

    // �}�l���r���ĪG
    public void StartTyping(string message, float typingSpeed = -1f, Action onComplete = null)
    {
        typingTriggers = GDCodeReindex(message); // �r�������l�ơA�O���b�ĴX�Ӧr���ɭ԰���ĪG
        message = Regex.Replace(message, @"\[[^\]]+\]", ""); // �����Ҧ�GDCode�� [tag]
        //Debug.Log($"[GDCodedMsg]{message}");

        if (onComplete != null) cbk = onComplete;
        fullMessage = message;
        if (Director.Inst != null) DefaultTypingInterval = Director.Inst.DefaultTypingInterval;
        if (typingSpeed == -1f) typingSpeed = DefaultTypingInterval;
        if (AVG.Inst.isSkipping) typingSpeed = 0f;
        //if (typingSpeed == -1f) typingSpeed = 1f;
        //Debug.Log($"typingSpeed: {typingSpeed}");
        // ����e����{�]�p�G���^
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // �}�l�s����{
        MarkNext.SetActive(false); // �����~��b�Y
        typingCoroutine = StartCoroutine(TypeText(message, typingSpeed));
    }

    // ���L���r���ĪG�ê�����ܥ���
    public void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        MarkNext.SetActive(true); // ����~��b�Y
        uiText.text = fullMessage; // ������ܧ���奻
        Debug.Log($"[SkipTyping]{fullMessage}");
        isTyping = false;
        cbk?.Invoke();
        cbk = null;
    }

    // ��{�G�v�r��ܤ奻
    private IEnumerator TypeText(string message, float typingSpeed = 0.05f)
    {
        List<string> richCharList = ParseRichTextCharacters(message);

        isTyping = true;
        uiText.text = ""; // �M�Ť奻
        int charIdx = -1;
        float typingSpeedWithWait = 0;
        foreach (string currentChar in richCharList)
        {
            charIdx++;
            //��l��action�t�C�ܼ�
            action_waitSec = 0f;
            //����action
            TypingExecuteAction(charIdx);
            //�W�[�r�����ݮɶ�(�p�G��)
            typingSpeedWithWait = Math.Max(0, typingSpeed + action_waitSec);

            //Debug.Log(currentChar);

            // �K�[��e�r����奻
            uiText.text += currentChar;

            // ����u����
            if (typingSound != null)
            {
                typingSound.Play();
            }

            // �ˬd�O�_�[�t���r
            float currentSpeed = Input.GetKey(KeyCode.Space) ? typingSpeedWithWait / 5 : typingSpeedWithWait;

            // ���ݷ�e�r�Ū����r���j
            yield return new WaitForSeconds(currentSpeed);
        }
        // ����~��b�Y
        MarkNext.SetActive(true);
        // �T�O�̲���ܧ���奻
        uiText.text = message;
        // ���r������
        isTyping = false;
        typingCoroutine = null;
        cbk?.Invoke();
        cbk = null;
    }

    // �ˬd���r���ĪG�O�_���b�B��
    public bool IsTyping()
    {
        return isTyping;
    }

    // ����r���S��
    public void TypingExecuteAction(int idx)
    {
        string[] parts = GDCodeParse(typingTriggers[idx]);
        //Debug.Log($"[TypingExecuteAction]{string.Concat(parts)}");
        if (parts.Length >= 1 && typingActions.ContainsKey(parts[0]))
        {
            // �����ʧ@�W�٩M�Ѽ�
            string actionName = parts[0];
            object[] parameters = parts[1..]; // �����ѤU���Ѽ�

            // ������������
            typingActions[actionName](parameters);
        }
    }
}
