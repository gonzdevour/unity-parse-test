using UnityEngine;
using System; // Type
using System.Collections;
using System.Reflection;
using DG.Tweening;

public class TweenPosition : TweenBase
{
    [Header("Target Values")]
    public float targetX = 0f;
    public float targetY = 0f;
    public Vector2 initialPosition = Vector2.zero;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    private void Awake()
    {
        // °O¿ýªì©l¦ì¸m
        if (initialPosition != Vector2.zero)
        {
            transform.localPosition = initialPosition;
        }
        startPosition = transform.localPosition;
    }

    protected override Tween TweenForward()
    {
        targetPosition = startPosition + new Vector3(targetX, targetY, 0);
        return transform.DOLocalMove(targetPosition, durForward).SetEase(easeForward);
    }

    protected override Tween TweenBackward()
    {
        return transform.DOLocalMove(startPosition, durBackward).SetEase(easeBackward);
    }

    protected override void ResetTween(int caseIdx = 0)
    {
        switch (caseIdx)
        {
            case 1: // to start
                transform.localPosition = startPosition;
                break;
            case 2: // to end
                transform.localPosition = targetPosition;
                break;
            default:
                break;
        }
    }
}
