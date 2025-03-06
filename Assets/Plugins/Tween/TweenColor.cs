using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TweenColor : TweenBase
{
    [Header("Target Values")]
    public float targetR = -1f;
    public float targetG = -1f;
    public float targetB = -1f;
    public float targetA = -1f;
    public float initialR = -1f;
    public float initialG = -1f;
    public float initialB = -1f;
    public float initialA = -1f;

    private Component colorComponent;

    private void Awake()
    {
        InitColor();
    }

    private void InitColor() // 將目前顏色設定為非-1的initialRGBA
    {
        Color currentColor = Color.white; // 預設為白色

        if (TryGetComponent(out Image img))
        {
            colorComponent = img;
            currentColor = img.color;
            ApplyInitColor(ref currentColor);
            img.color = currentColor;
        }
        else if (TryGetComponent(out SpriteRenderer sr))
        {
            colorComponent = sr;
            currentColor = sr.color;
            ApplyInitColor(ref currentColor);
            sr.color = currentColor;
        }
        else if (TryGetComponent(out MeshRenderer mr))
        {
            colorComponent = mr;
            currentColor = mr.material.color;
            ApplyInitColor(ref currentColor);
            mr.material.color = currentColor;
        }
    }

    private void TargetColor() // 將目前顏色設定為非-1的targetRGBA
    {
        Color currentColor = Color.white; // 預設為白色

        if (TryGetComponent(out Image img))
        {
            colorComponent = img;
            currentColor = img.color;
            ApplyTargetColor(ref currentColor);
            img.color = currentColor;
        }
        else if (TryGetComponent(out SpriteRenderer sr))
        {
            colorComponent = sr;
            currentColor = sr.color;
            ApplyTargetColor(ref currentColor);
            sr.color = currentColor;
        }
        else if (TryGetComponent(out MeshRenderer mr))
        {
            colorComponent = mr;
            currentColor = mr.material.color;
            ApplyTargetColor(ref currentColor);
            mr.material.color = currentColor;
        }
    }

    /// <summary>
    /// 依據 initialR/G/B/A 來修改顏色
    /// </summary>
    private void ApplyInitColor(ref Color color)
    {
        if (initialR >= 0) { color.r = initialR; } else { initialR = color.r; }
        if (initialG >= 0) { color.g = initialG; } else { initialG = color.g; }
        if (initialB >= 0) { color.b = initialB; } else { initialB = color.b; }
        if (initialA >= 0) { color.a = initialA; } else { initialA = color.a; }
    }

    /// <summary>
    /// 依據 targetR/G/B/A 來修改顏色
    /// </summary>
    private void ApplyTargetColor(ref Color color)
    {
        if (targetR >= 0) { color.r = targetR; } else { targetR = color.r; }
        if (targetG >= 0) { color.g = targetG; } else { targetG = color.g; }
        if (targetB >= 0) { color.b = targetB; } else { targetB = color.b; }
        if (targetA >= 0) { color.a = targetA; } else { targetA = color.a; }
    }

    /// <summary>
    /// 取得當前顏色 (適用於 Image, SpriteRenderer, MeshRenderer)
    /// </summary>
    private Color GetCurrentColor()
    {
        if (colorComponent is Image img)
        {
            return img.color;
        }
        else if (colorComponent is SpriteRenderer sr)
        {
            return sr.color;
        }
        else if (colorComponent is MeshRenderer mr && mr.material != null)
        {
            return mr.material.color;
        }
        return Color.white; // 預設值
    }

    /// <summary>
    /// 套用顏色 (適用於 Image, SpriteRenderer, MeshRenderer)
    /// </summary>
    private void ApplyTweenColor(Color color)
    {
        if (colorComponent is Image img)
        {
            img.color = color;
        }
        else if (colorComponent is SpriteRenderer sr)
        {
            sr.color = color;
        }
        else if (colorComponent is MeshRenderer mr && mr.material != null)
        {
            mr.material.color = color;
        }
    }

    protected override Tween TweenForward()
    {
        if (colorComponent == null)
        {
            Debug.LogWarning("colorComponent 未設置");
            return null;
        }

        Color currentColor = GetCurrentColor();
        Sequence seq = DOTween.Sequence();

        if (targetR >= 0)
        {
            seq.Join(DOTween.To(() => currentColor.r, x =>
            {
                currentColor.r = x;
                ApplyTweenColor(currentColor);
            }, targetR, durForward).SetEase(easeForward));
        }
        if (targetG >= 0)
        {
            seq.Join(DOTween.To(() => currentColor.g, x =>
            {
                currentColor.g = x;
                ApplyTweenColor(currentColor);
            }, targetG, durForward).SetEase(easeForward));
        }
        if (targetB >= 0)
        {
            seq.Join(DOTween.To(() => currentColor.b, x =>
            {
                currentColor.b = x;
                ApplyTweenColor(currentColor);
            }, targetB, durForward).SetEase(easeForward));
        }
        if (targetA >= 0)
        {
            seq.Join(DOTween.To(() => currentColor.a, x =>
            {
                currentColor.a = x;
                ApplyTweenColor(currentColor);
            }, targetA, durForward).SetEase(easeForward));
        }

        return seq;
    }

    protected override Tween TweenBackward()
    {
        if (colorComponent == null)
        {
            Debug.LogWarning("colorComponent 未設置");
            return null;
        }

        Color currentColor = GetCurrentColor();
        Sequence seq = DOTween.Sequence();

        if (initialR >= 0)
        {
            seq.Join(DOTween.To(() => currentColor.r, x =>
            {
                currentColor.r = x;
                ApplyTweenColor(currentColor);
            }, initialR, durBackward).SetEase(easeBackward));
        }
        if (initialG >= 0)
        {
            seq.Join(DOTween.To(() => currentColor.g, x =>
            {
                currentColor.g = x;
                ApplyTweenColor(currentColor);
            }, initialG, durBackward).SetEase(easeBackward));
        }
        if (initialB >= 0)
        {
            seq.Join(DOTween.To(() => currentColor.b, x =>
            {
                currentColor.b = x;
                ApplyTweenColor(currentColor);
            }, initialB, durBackward).SetEase(easeBackward));
        }
        if (initialA >= 0)
        {
            seq.Join(DOTween.To(() => currentColor.a, x =>
            {
                currentColor.a = x;
                ApplyTweenColor(currentColor);
            }, initialA, durBackward).SetEase(easeBackward));
        }

        return seq;
    }

    protected override void ResetTween(int caseIdx = 0)
    {
        switch (caseIdx)
        {
            case 1: // to start
                InitColor();
                break;
            case 2: // to end
                TargetColor();
                break;
            default:
                break;
        }
    }
}
