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
        // 取得當前的 localScale
        Vector3 scale = transform.localScale;
        // 只有當 initialScale.x >= 0 才設定 X 軸
        if (initialScale.x >= 0) scale.x = initialScale.x;
        // 只有當 initialScale.y >= 0 才設定 Y 軸
        if (initialScale.y >= 0) scale.y = initialScale.y;
        // 更新 Transform 的 localScale
        transform.localScale = scale;
        // 設定 startScale
        startScale = transform.localScale;
    }

    private void SetTargetScale()
    {
        // 取得當前的 localScale
        targetScale = transform.localScale;
        // 只有當 targetScaleX 或 targetScaleY >= 0 時才覆蓋對應軸向
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
