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
    public float targetX = 0f; // �ؼЦ첾�q X
    public float targetY = -20f; // �ؼЦ첾�q Y
    public float durForward = 0.5f; // �e���ؼЮɶ�
    public float durBackward = 0.5f; // ��^��l��m���ɶ�
    public Ease easeForward = Ease.Linear; // �e���ؼЪ��w�ʮĪG
    public Ease easeBackward = Ease.Linear; // ��^��l��m���w�ʮĪG
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
        // �O����l��m
        if (initial != Vector2.zero)
        {
            transform.localPosition = initial;
        }
        initialPosition = transform.localPosition;

        // �}�l Tween �ʵe
        if (StartOnEnable) CoroutineTween = StartCoroutine(Tween());
    }

    private void OnDisable()
    {
        // ����ʵe�í��m��m
        StopTween();
    }
    /// <summary>
    /// �I�s StartTween() �ɡA�i�H �u�ǤJ�Q�ק諸�ѼơA��L�O�d��ȡC
    /// ���F�i�H�binspector�������ק�ƭȡA�y��code������l���ܱo�۷����
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
        Debug.Log("tween�}�l");
        // �_�lDelay
        yield return new WaitForSeconds(delay);

        // �Ыش`��Tween�ʵe
        Vector3 targetPosition;
        for (int i = 0; loopTimes == -1 || i <= loopTimes; i++) //-1���L���j��
        {
            if (gameObject == null || CoroutineTween == null) { break; } // moveTween�Y�����h�j�鰱��

            transform.localPosition = initialPosition;
            targetPosition = initialPosition + new Vector3(targetX, targetY, 0);
            curTween = transform.DOLocalMove(targetPosition, durForward).SetEase(easeForward);
            yield return curTween.WaitForCompletion();
            cbkForward?.Invoke();
            yield return new WaitForSeconds(loopDelayForward);
            cbkLoopDelayForward?.Invoke();

            if (Yoyo == false) { continue; } //�O�_�Ӧ^

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
        // ����ʵe
        curTween?.Kill();
        if (CoroutineTween != null) StopCoroutine(CoroutineTween);
        // �^���l��m
        transform.localPosition = initialPosition;
    }
}
