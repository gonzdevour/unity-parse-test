using UnityEngine;
using System;
using System.Collections;
using DG.Tweening;

public class TweenPositionOld : MonoBehaviour
{
    private Vector3 initialPosition;
    private Tween curTween;
    private Coroutine CoroutineTween;

    [Header("Tween Settings")]
    public int loopTimes = -1;
    public bool StartOnEnable = false;
    public bool Yoyo = false;
    public bool ResetAfterTween = false;
    public bool DestroyAfterTween = false;
    public float targetX = 0f; // 目標位移量 X
    public float targetY = -20f; // 目標位移量 Y
    public float durForward = 0.5f; // 前往目標時間
    public float durBackward = 0.5f; // 返回初始位置的時間
    public Ease easeForward = Ease.Linear; // 前往目標的緩動效果
    public Ease easeBackward = Ease.Linear; // 返回初始位置的緩動效果
    public float delay = 0f;
    public float loopDelayForward = 0f;
    public float loopDelayBackward = 0f;
    public Vector2 initial = Vector2.zero;

    public Action cbkForward;
    public Action cbkLoopDelayForward;
    public Action cbkBackward;
    public Action cbkLoopDelayBackward;
    public Action cbkComplete;

    private void OnEnable()
    {
        // 記錄初始位置
        if (initial != Vector2.zero)
        {
            transform.localPosition = initial;
        }
        initialPosition = transform.localPosition;

        // 開始 Tween 動畫
        if (StartOnEnable) CoroutineTween = StartCoroutine(Tween());
    }

    private void OnDisable()
    {
        // 停止動畫並重置位置
        StopTween();
    }
    /// <summary>
    /// 呼叫 StartTween() 時，可以 只傳入想修改的參數，其他保留原值。
    /// 為了可以在inspector中直接修改數值，造成code內的初始化變得相當複雜
    /// </summary>
    public IEnumerator StartTween(
        int? loopTimes = null,
        bool? startOnEnable = null,
        bool? yoyo = null,
        bool? resetAfterTween = null,
        bool? destroyAfterTween = null,
        float? targetX = null,
        float? targetY = null,
        float? durForward = null,
        float? durBackward = null,
        Ease? easeForward = null,
        Ease? easeBackward = null,
        float? delay = null,
        float? loopDelayForward = null,
        float? loopDelayBackward = null,
        Vector2? initial = null,
        Action cbkForward = null,
        Action cbkLoopDelayForward = null,
        Action cbkBackward = null,
        Action cbkLoopDelayBackward = null,
        Action cbkComplete = null
    )
    {
        if (loopTimes.HasValue) this.loopTimes = loopTimes.Value;
        if (startOnEnable.HasValue) this.StartOnEnable = startOnEnable.Value;
        if (yoyo.HasValue) this.Yoyo = yoyo.Value;
        if (resetAfterTween.HasValue) this.ResetAfterTween = resetAfterTween.Value;
        if (destroyAfterTween.HasValue) this.DestroyAfterTween = destroyAfterTween.Value;
        if (targetX.HasValue) this.targetX = targetX.Value;
        if (targetY.HasValue) this.targetY = targetY.Value;
        if (durForward.HasValue) this.durForward = Mathf.Max(0, durForward.Value);
        if (durBackward.HasValue) this.durBackward = Mathf.Max(0, durBackward.Value);
        if (easeForward.HasValue) this.easeForward = easeForward.Value;
        if (easeBackward.HasValue) this.easeBackward = easeBackward.Value;
        if (delay.HasValue) this.delay = Mathf.Max(0, delay.Value);
        if (loopDelayForward.HasValue) this.loopDelayForward = Mathf.Max(0, loopDelayForward.Value);
        if (loopDelayBackward.HasValue) this.loopDelayBackward = Mathf.Max(0, loopDelayBackward.Value);
        if (initial.HasValue) this.initial = initial.Value;
        if (loopTimes.HasValue) this.loopTimes = loopTimes.Value;
        if (cbkForward != null) this.cbkForward = cbkForward;
        if (cbkLoopDelayForward != null) this.cbkLoopDelayForward = cbkLoopDelayForward;
        if (cbkBackward != null) this.cbkBackward = cbkBackward;
        if (cbkLoopDelayBackward != null) this.cbkLoopDelayBackward = cbkLoopDelayBackward;
        if (cbkComplete != null) this.cbkComplete = cbkComplete;

        yield return CoroutineTween = StartCoroutine(Tween());
    }

    private IEnumerator Tween()
    {
        Debug.Log("tween開始");
        // 起始Delay
        yield return new WaitForSeconds(delay);

        // 創建循環Tween動畫
        Vector3 targetPosition;
        for (int i = 0; loopTimes == -1 || i <= loopTimes; i++) //-1為無限迴圈
        {
            if (gameObject == null || CoroutineTween == null) { break; } // moveTween若移除則迴圈停止

            transform.localPosition = initialPosition;
            targetPosition = initialPosition + new Vector3(targetX, targetY, 0);
            curTween = transform.DOLocalMove(targetPosition, durForward).SetEase(easeForward);
            yield return curTween.WaitForCompletion();
            cbkForward?.Invoke();
            yield return new WaitForSeconds(loopDelayForward);
            cbkLoopDelayForward?.Invoke();

            if (Yoyo == false) { continue; } //是否來回

            targetPosition = initialPosition;
            curTween = transform.DOLocalMove(targetPosition, durBackward).SetEase(easeBackward);
            yield return curTween.WaitForCompletion();
            cbkBackward?.Invoke();
            yield return new WaitForSeconds(loopDelayBackward);
            cbkLoopDelayBackward?.Invoke();
        }
        cbkComplete?.Invoke();
        if (ResetAfterTween) transform.localPosition = initialPosition;
        if (DestroyAfterTween) Destroy(gameObject);
    }

    private void StopTween()
    {
        // 停止動畫
        curTween?.Kill();
        if (CoroutineTween != null) StopCoroutine(CoroutineTween);
        // 回到初始位置
        transform.localPosition = initialPosition;
    }
}
