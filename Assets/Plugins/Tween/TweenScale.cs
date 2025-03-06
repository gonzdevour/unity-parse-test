using UnityEngine;
using DG.Tweening;

public class TweenScale : TweenBase
{
    [Header("Target Values")]
    public float targetScaleX = -1f;
    public float targetScaleY = -1f;
    public Vector2 initialScale = new(-1f, -1f);

    private Vector3 startScale;
    private Vector3 targetScale;

    private void Awake()
    {
        SetStartScale();
    }

    private void SetStartScale()
    {
        // ���o��e�� localScale
        Vector3 scale = transform.localScale;
        // �u���� initialScale.x >= 0 �~�]�w X �b
        if (initialScale.x >= 0) scale.x = initialScale.x;
        // �u���� initialScale.y >= 0 �~�]�w Y �b
        if (initialScale.y >= 0) scale.y = initialScale.y;
        // ��s Transform �� localScale
        transform.localScale = scale;
        // �]�w startScale
        startScale = transform.localScale;
    }

    private void SetTargetScale()
    {
        // ���o��e�� localScale
        targetScale = transform.localScale;
        // �u���� targetScaleX �� targetScaleY >= 0 �ɤ~�л\�����b�V
        if (targetScaleX >= 0) targetScale.x = targetScaleX;
        if (targetScaleY >= 0) targetScale.y = targetScaleY;
    }

    protected override Tween TweenForward()
    {
        Sequence seq = DOTween.Sequence();
        if (targetScaleX >= 0) seq.Join(transform.DOScaleX(targetScaleX, durForward).SetEase(easeForward));
        if (targetScaleY >= 0) seq.Join(transform.DOScaleY(targetScaleY, durForward).SetEase(easeForward));
        return seq;
    }

    protected override Tween TweenBackward()
    {
        return transform.DOScale(startScale, durBackward).SetEase(easeBackward);
    }

    protected override void ResetTween(int caseIdx = 0)
    {
        switch (caseIdx)
        {
            case 1: // to start
                transform.localScale = startScale;
                break;
            case 2: // to end
                SetTargetScale();
                break;
            default:
                break;
        }
    }
}
