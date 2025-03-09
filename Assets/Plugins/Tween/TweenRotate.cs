using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class TweenRotate : TweenBase
{
    [Header("Target Values")]
    public Vector3 addAngle = new(0f, 0f, 0f);
    public bool enableInitAngle = false;
    public Vector3 initAngle = new(-1f, -1f, -1f);

    private Vector3 startAngle = new(0f, 0f, 0f);
    private Vector3 targetAngle = new(0f, 0f, 0f);

    private void Awake()
    {
        InitTween();
    }

    public override void InitTween()
    {
        if (enableInitAngle)
        {
            // 起點：如有預設值則將起點設為預設值
            startAngle = new Vector3(initAngle.x, initAngle.y, initAngle.z);
            transform.eulerAngles = startAngle;
        }
        else
        {
            // 起點：如無預設值則起點為原值
            startAngle = transform.eulerAngles;
        }
        // 終點：先以起點為終點初始化
        targetAngle = startAngle + addAngle;
    }

    protected override Tween TweenForward()
    {
        return transform.DORotate(targetAngle, durForward, RotateMode.FastBeyond360).SetEase(easeForward);
    }

    protected override Tween TweenBackward()
    {
        return transform.DORotate(transform.eulerAngles - addAngle, durBackward, RotateMode.FastBeyond360).SetEase(easeBackward);
    }

    protected override void ResetTween(int caseIdx = 0)
    {
        switch (caseIdx)
        {
            case 1: // to start
                transform.eulerAngles = startAngle;
                break;
            case 2: // to end
                transform.eulerAngles = targetAngle;
                break;
            default:
                break;
        }
    }
}
