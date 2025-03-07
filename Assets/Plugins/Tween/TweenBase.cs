using UnityEngine;
using System;
using System.Collections;
using DG.Tweening;
using System.Reflection;

public abstract class TweenBase : MonoBehaviour
{
    protected Tween curTween;
    protected Coroutine coroutineTween;

    [Header("Progress Settings")]
    public bool startOnEnable = false;     // �O�_�benable�ɦ۰ʰ���
    public int loopTimes = -1;             // -1 ���L���j��A��L�ƭȪ�ܰj�馸�ơA�Y�� 0 �h���� 1 ��
    public bool yoyo = false;              // �O�_�Ӧ^ Tween
    public bool resetAfterTween = false;   // Tween ������O�_���m����
    public bool destroyAfterTween = false; // Tween ������O�_�P������
    public float durForward = 0.5f;          // �e�i Tween �ɶ�
    public float durBackward = 0.5f;         // ��^ Tween �ɶ�
    public Ease easeForward = Ease.Linear;   // �e�i�w�ʮĪG
    public Ease easeBackward = Ease.Linear;  // ��^�w�ʮĪG
    public float delay = 0f;                 // �_�l����
    public float loopDelayForward = 0f;      // �e�i�᩵��
    public float loopDelayBackward = 0f;     // ��^�᩵��
    public bool loopEndAtFoward = false;   // �̫�@���j��O�_�����e�i���q

    public Action cbkForward;
    public Action cbkLoopDelayForward;
    public Action cbkBackward;
    public Action cbkLoopDelayBackward;
    public Action cbkComplete;

    protected virtual void OnEnable()
    {
        if (startOnEnable) coroutineTween = StartCoroutine(TweenCoroutine());
    }

    protected virtual void OnDisable()
    {
        StopTween();
    }

    public IEnumerator StartTween(TweenParams parameters)
    {
        // �z�L�Ϯg�۰ʤǰt�ܼƦW�ٻP�ѼƦW�١A�ó]�w�ȡC(�ܼƦW�ٻP�ѼƦW�٬ۦP)
        Type paramType = parameters.GetType();
        Type thisType = this.GetType();
        foreach (FieldInfo paramField in paramType.GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            FieldInfo targetField = thisType.GetField(paramField.Name, BindingFlags.Public | BindingFlags.Instance);
            if (targetField != null)
            {
                object value = paramField.GetValue(parameters);
                if (value != null)
                {
                    targetField.SetValue(this, value);
                }
            }
        }
        InitTween();
        yield return coroutineTween = StartCoroutine(TweenCoroutine());
    }

    protected virtual IEnumerator TweenCoroutine()
    {
        yield return new WaitForSeconds(delay);

        // �`�N�G�� loopTimes �� 0 �ɡA�ڭ̧Ʊ���� 1 ��
        int iterations = loopTimes == -1 ? int.MaxValue : Mathf.Max(1, loopTimes);

        for (int i = 0; i < iterations; i++)
        {
            if (gameObject == null || coroutineTween == null) { break; }

            // Forward Tween
            curTween = TweenForward();
            yield return curTween.WaitForCompletion();
            cbkForward?.Invoke();
            yield return new WaitForSeconds(loopDelayForward);
            cbkLoopDelayForward?.Invoke();

            // �p�G���ҥ� yoyo�A�N���L backward tween
            if (!yoyo) continue;
            // �p�G�Ʊ�̫�@�鰱��bfoward���q�A�N���L backward tween
            if (i == iterations && loopEndAtFoward) continue;

            // Backward Tween
            curTween = TweenBackward();
            yield return curTween.WaitForCompletion();
            cbkBackward?.Invoke();
            yield return new WaitForSeconds(loopDelayBackward);
            cbkLoopDelayBackward?.Invoke();
        }

        cbkComplete?.Invoke();
        
        if (resetAfterTween) ResetTween(1);
        if (destroyAfterTween) Destroy(gameObject);
    }

    protected void StopTween(int caseIdx = 0)
    {
        // ����ʵe
        curTween?.Kill();
        if (coroutineTween != null) StopCoroutine(coroutineTween);
        // �M�w�����m
        ResetTween(caseIdx);
    }

    public abstract void InitTween();
    protected abstract Tween TweenForward();
    protected abstract Tween TweenBackward();
    protected abstract void ResetTween(int caseIdx = 0);
}
