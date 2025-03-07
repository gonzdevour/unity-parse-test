using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class TweenRotate : TweenBase
{
    [Header("Target Values")]
    public bool enableRotateX = false;
    public float addRotateX = -1;
    public bool enableRotateY = false;
    public float addRotateY = -1;
    public bool enableRotateZ = false;
    public float addRotateZ = -1;
    public bool enableInitial = false;
    public Vector3 initialAngle = new(0f, 0f, 0f);

    private Vector3 startAngle = new(0f, 0f, 0f);
    private Vector3 targetAngle = new(0f, 0f, 0f);
    private float startRotateX;
    private float startRotateY;
    private float startRotateZ;
    private float targetRotateX;
    private float targetRotateY;
    private float targetRotateZ;


    private void Awake()
    {
        InitTween();
    }

    public override void InitTween()
    {
        if (enableInitial)
        {
            // �_�I�G�p���w�]�ȫh�N�_�I�]���w�]��
            startRotateX = initialAngle.x;
            startRotateY = initialAngle.y;
            startRotateZ = initialAngle.z;
            transform.eulerAngles = new Vector3(startRotateX, startRotateY, startRotateZ);
        }
        else
        {
            // �_�I�G�p�L�w�]�ȫh�_�I�����
            startRotateX = transform.eulerAngles.x;
            startRotateY = transform.eulerAngles.y;
            startRotateZ = transform.eulerAngles.z;
        }

        // ���I�G���H�_�I�����I��l��
        targetRotateX = transform.eulerAngles.x;
        targetRotateY = transform.eulerAngles.y;
        targetRotateZ = transform.eulerAngles.z;

        SetStartToTarget();
        startAngle = new Vector3(startRotateX, startRotateY, startRotateZ);
        targetAngle = new Vector3(targetRotateX, targetRotateY, targetRotateZ);
    }

    private void SetStartToTarget()
    {
        // ���F�קK���t���׳y�������׸��D�A���s�p��_�I�P���I
        if (enableRotateX)
        {
            Dictionary<string, float> anglesPair = RecountStartAndTarget(startRotateX, addRotateX);
            startRotateX = anglesPair["start"];
            targetRotateX = anglesPair["target"];
        }
        if (enableRotateY)
        {
            Dictionary<string, float> anglesPair = RecountStartAndTarget(startRotateY, addRotateY);
            startRotateY = anglesPair["start"];
            targetRotateY = anglesPair["target"];
        }
        if (enableRotateZ)
        {
            Dictionary<string, float> anglesPair = RecountStartAndTarget(startRotateZ, addRotateZ);
            startRotateZ = anglesPair["start"];
            targetRotateZ = anglesPair["target"];
            Debug.Log($"{startRotateZ}to{targetRotateZ}");
        }
    }

    private void SetTargetToStart()
    {
        // ���F�קK���t���׳y�������׸��D�A���s�p��_�I�P���I
        if (enableRotateX)
        {
            Dictionary<string, float> anglesPair = RecountStartAndTarget(targetRotateX, -addRotateX);
            targetRotateZ = anglesPair["start"];
            startRotateZ = anglesPair["target"];
        }
        if (enableRotateY)
        {
            Dictionary<string, float> anglesPair = RecountStartAndTarget(targetRotateY, -addRotateY);
            targetRotateZ = anglesPair["start"];
            startRotateZ = anglesPair["target"];
        }
        if (enableRotateZ)
        {
            Debug.Log($"originTargetZ:{targetRotateZ}");
            Dictionary<string, float> anglesPair = RecountStartAndTarget(targetRotateZ, -addRotateZ);
            targetRotateZ = anglesPair["start"];
            startRotateZ = anglesPair["target"];
            Debug.Log($"{targetRotateZ}to{startRotateZ}");
        }
    }

    private Dictionary<string, float> RecountStartAndTarget(float start, float toAdd)
    {
        start = PositiveAngle(start);
        float target = PositiveAngle(start + toAdd);
        Debug.Log($"�_�I��{start}���I��{target})");

        Dictionary<string, float> result = new();
        result["start"] = start;
        result["target"] = target;
        return result;
    }

    private float PositiveAngle(float angle)
    {
        return (angle + 36000) % 360;
    }

    private float NegativeAngle(float angle)
    {
        return (angle - 36000) % 360;
    }

    protected override Tween TweenForward()
    {
        SetStartToTarget();
        Sequence seq = DOTween.Sequence();
        if (enableRotateX)
        {
            seq.Join(DOTween.To(() => transform.eulerAngles.x, x =>
            {
                Vector3 rotation = transform.eulerAngles;
                rotation.x = x;
                transform.eulerAngles = rotation;
            }, targetRotateX, durForward).SetEase(easeForward));
        }
        if (enableRotateY)
        {
            seq.Join(DOTween.To(() => transform.eulerAngles.y, y =>
            {
                Vector3 rotation = transform.eulerAngles;
                rotation.y = y;
                transform.eulerAngles = rotation;
            }, targetRotateY, durForward).SetEase(easeForward));
        }
        if (enableRotateZ)
        {
            seq.Join(DOTween.To(() => transform.eulerAngles.z, z =>
            {
                Vector3 rotation = transform.eulerAngles;
                rotation.z = z;
                transform.eulerAngles = rotation;
            }, targetRotateZ, durForward).SetEase(easeForward));
        }
        return seq;
    }

    protected override Tween TweenBackward()
    {
        SetTargetToStart();
        Sequence seq = DOTween.Sequence();
        if (enableRotateX)
        {
            seq.Join(DOTween.To(() => transform.eulerAngles.x, x =>
            {
                Vector3 rotation = transform.eulerAngles;
                rotation.x = x;
                transform.eulerAngles = rotation;
            }, startRotateX, durBackward).SetEase(easeBackward));
        }
        if (enableRotateY)
        {
            seq.Join(DOTween.To(() => transform.eulerAngles.y, y =>
            {
                Vector3 rotation = transform.eulerAngles;
                rotation.y = y;
                transform.eulerAngles = rotation;
            }, startRotateY, durBackward).SetEase(easeBackward));
        }
        if (enableRotateZ)
        {
            seq.Join(DOTween.To(() => transform.eulerAngles.z, z =>
            {
                Vector3 rotation = transform.eulerAngles;
                rotation.z = z;
                transform.eulerAngles = rotation;
            }, startRotateZ, durBackward).SetEase(easeBackward));
        }
        return seq;
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
