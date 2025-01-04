using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TransitionEffectFade : MonoBehaviour, ITransitionEffect
{
    private GameObject UnmaskCtnr;
    private Image ScreenImg;
    private float DurOut;
    private float DurIn;
    private Ease EaseOut;
    private Ease EaseIn;
    private Tween activeTween;

    public void Init(TransitionEffectConfig config)
    {
        UnmaskCtnr = config.UnmaskCtnr;
        ScreenImg = config.ScreenImg;
        DurOut = config.DurOut;
        DurIn = config.DurIn;
        EaseOut = config.EaseOut;
        EaseIn = config.EaseIn;

        activeTween = null;
    }

    public void FadeIn(System.Action onComplete)
    {
        if (ScreenImg == null)
        {
            Debug.LogError("FadeIn 無法執行，因為 ScreenImg 未正確設置！");
            return;
        }

        Stop(true); // 停止任何進行中的 Tween，保證乾淨狀態

        // 開始淡入動畫
        activeTween = ScreenImg.DOFade(0, DurIn)
            .SetEase(EaseIn)
            .OnComplete(() =>
            {
                activeTween = null;
                onComplete?.Invoke();
            });
    }

    public void FadeOut(System.Action onComplete)
    {
        if (ScreenImg == null)
        {
            Debug.LogError("FadeOut 無法執行，因為 ScreenImg 未正確設置！");
            return;
        }

        Stop(true); // 停止任何進行中的 Tween，保證乾淨狀態

        // 開始淡出動畫
        activeTween = ScreenImg.DOFade(1, DurOut)
            .SetEase(EaseOut)
            .OnComplete(() =>
            {
                activeTween = null;
                onComplete?.Invoke();
            });
    }

    public void Stop(bool instant)
    {
        if (ScreenImg == null)
        {
            Debug.LogError("Stop 無法執行，因為 ScreenImg 未正確設置！");
            return;
        }

        // 停止任何進行中的 Tween
        if (activeTween != null && activeTween.IsActive())
        {
            activeTween.Kill(instant);
        }

        if (instant)
        {
            // 根據 Tween 狀態設定完成值
            if (activeTween != null && activeTween.IsPlaying())
            {
                if (activeTween.Duration(false) == DurIn)
                {
                    // 如果是 FadeIn，設定最終透明度為 0
                    ScreenImg.color = new Color(ScreenImg.color.r, ScreenImg.color.g, ScreenImg.color.b, 0);
                }
                else if (activeTween.Duration(false) == DurOut)
                {
                    // 如果是 FadeOut，設定最終透明度為 1
                    ScreenImg.color = new Color(ScreenImg.color.r, ScreenImg.color.g, ScreenImg.color.b, 1);
                }
            }
        }
        else
        {
            // 將透明度設置為 Tween 開始前的狀態
            ScreenImg.color = new Color(ScreenImg.color.r, ScreenImg.color.g, ScreenImg.color.b, 1);
        }

        activeTween = null; // 清理 Active Tween
    }
}
