using UnityEngine;
using System;
using DG.Tweening;

/// <summary>
/// Tween �Ѽƪ��e���A���\�i���ܼ�
/// </summary>
public class TweenParams
{
    // tweenBase
    public bool? startOnEnable;
    public int? loopTimes;
    public bool? yoyo;
    public bool? resetAfterTween;
    public bool? destroyAfterTween;
    public float? durForward;
    public float? durBackward;
    public Ease? easeForward;
    public Ease? easeBackward;
    public float? delay;
    public float? loopDelayForward;
    public float? loopDelayBackward;
    public bool? loopEndAtFoward;
    public Action cbkForward;
    public Action cbkLoopDelayForward;
    public Action cbkBackward;
    public Action cbkLoopDelayBackward;
    public Action cbkComplete;

    // tweenPosition
    public float? targetX;
    public float? targetY;
    public Vector2? initialPosition;

    // tweenColor
    public float? targetR;
    public float? targetG;
    public float? targetB;
    public float? targetA;
    public float? initialR;
    public float? initialG;
    public float? initialB;
    public float? initialA;
}