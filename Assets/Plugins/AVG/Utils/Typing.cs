using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Typing : MonoBehaviour
{
    public Text uiText; // UI Text (Legacy) ����
    public float DefaultTypingInterval = 0.05f; // �C�Ӧr����ܶ��j
    public AudioSource typingSound; // �u����
    public bool richTextSupport = true; // �O�_�䴩�I�奻����

    private Coroutine typingCoroutine;
    private bool isTyping; // �O�_���b�i�楴�r���ĪG
    private string fullMessage; //�ثe����
    private Action cbk;

    // �}�l���r���ĪG
    public void StartTyping(string message, float typingSpeed = -1f, Action onComplete = null)
    {
        if (onComplete != null) cbk = onComplete;
        fullMessage = message;
        if (Director.Inst != null) DefaultTypingInterval = Director.Inst.DefaultTypingInterval;
        if (typingSpeed == -1f) typingSpeed = DefaultTypingInterval;
        //if (typingSpeed == -1f) typingSpeed = 1f;
        //Debug.Log($"typingSpeed: {typingSpeed}");
        // ����e����{�]�p�G���^
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // �}�l�s����{
        typingCoroutine = StartCoroutine(TypeText(message, typingSpeed));
    }

    // ���L���r���ĪG�ê�����ܥ���
    public void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        uiText.text = fullMessage; // ������ܧ���奻
        isTyping = false;
        cbk?.Invoke();
        cbk = null;
    }

    // ��{�G�v�r��ܤ奻
    private IEnumerator TypeText(string message, float typingSpeed = 0.05f)
    {
        isTyping = true;
        uiText.text = ""; // �M�Ť奻
        int tagOpen = 0; // �I�奻���ҭp�ơ]�ȷ� richTextSupport �� true �ɨϥΡ^

        for (int i = 0; i < message.Length; i++)
        {
            char currentChar = message[i];

            // �B�z�I�奻����
            if (richTextSupport)
            {
                if (currentChar == '<') tagOpen++;
                if (currentChar == '>') tagOpen--;

                // ��I�奻���ҥ������ɡA�����K�[�r�Ũø��L����
                if (tagOpen > 0 || currentChar == '>')
                {
                    uiText.text += currentChar;
                    continue;
                }
            }

            // �K�[��e�r����奻
            uiText.text += currentChar;

            // ����u����
            if (typingSound != null)
            {
                typingSound.Play();
            }

            // �ˬd�O�_�[�t���r
            float currentSpeed = Input.GetKey(KeyCode.Space) ? typingSpeed / 5 : typingSpeed;

            // ���ݷ�e�r�Ū����r���j
            yield return new WaitForSeconds(currentSpeed);
        }

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
}
