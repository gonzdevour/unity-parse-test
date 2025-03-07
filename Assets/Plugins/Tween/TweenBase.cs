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
    public bool startOnEnable = false;     // 是否在enable時自動執行
    public int loopTimes = -1;             // -1 為無限迴圈，其他數值表示迴圈次數，若為 0 則執行 1 次
    public bool yoyo = false;              // 是否來回 Tween
    public bool resetAfterTween = false;   // Tween 完成後是否重置物件
    public bool destroyAfterTween = false; // Tween 完成後是否銷毀物件
    public float durForward = 0.5f;          // 前進 Tween 時間
    public float durBackward = 0.5f;         // 返回 Tween 時間
    public Ease easeForward = Ease.Linear;   // 前進緩動效果
    public Ease easeBackward = Ease.Linear;  // 返回緩動效果
    public float delay = 0f;                 // 起始延遲
    public float loopDelayForward = 0f;      // 前進後延遲
    public float loopDelayBackward = 0f;     // 返回後延遲
    public bool loopEndAtFoward = false;   // 最後一次迴圈是否停止於前進階段

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
        // 透過反射自動匹配變數名稱與參數名稱，並設定值。(變數名稱與參數名稱相同)
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

        // 注意：當 loopTimes 為 0 時，我們希望執行 1 次
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

            // 如果不啟用 yoyo，就跳過 backward tween
            if (!yoyo) continue;
            // 如果希望最後一圈停止在foward階段，就跳過 backward tween
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
        // 停止動畫
        curTween?.Kill();
        if (coroutineTween != null) StopCoroutine(coroutineTween);
        // 決定停止位置
        ResetTween(caseIdx);
    }

    public abstract void InitTween();
    protected abstract Tween TweenForward();
    protected abstract Tween TweenBackward();
    protected abstract void ResetTween(int caseIdx = 0);
}
