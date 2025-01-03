using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TransitionEffectFade : ITransitionEffect
{
    private GameObject UnmaskCtnr;
    private Image ScreenImg;
    private float DurationOut;
    private float DurationIn;
    private Ease EaseOut;
    private Ease EaseIn;
    private Tween activeTween;

    public TransitionEffectFade(GameObject UnmaskCtnr, Image ScreenImg, float DurOut, float DurIn, Ease EaseOut, Ease EaseIn)
    {
        this.UnmaskCtnr = UnmaskCtnr;
        this.ScreenImg = ScreenImg;
        this.DurationOut = DurOut;
        this.DurationIn = DurIn;
        this.EaseOut = EaseOut;
        this.EaseIn = EaseIn;
        activeTween = null;
    }

    public void FadeIn(System.Action onComplete)
    {
        if (ScreenImg == null)
        {
            Debug.LogError("FadeIn �L�k����A�]�� ScreenImg �����T�]�m�I");
            return;
        }

        Stop(true); // �������i�椤�� Tween�A�O�Ұ��b���A

        // �}�l�H�J�ʵe
        activeTween = ScreenImg.DOFade(0, DurationIn)
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
            Debug.LogError("FadeOut �L�k����A�]�� ScreenImg �����T�]�m�I");
            return;
        }

        Stop(true); // �������i�椤�� Tween�A�O�Ұ��b���A

        // �}�l�H�X�ʵe
        activeTween = ScreenImg.DOFade(1, DurationOut)
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
            Debug.LogError("Stop �L�k����A�]�� ScreenImg �����T�]�m�I");
            return;
        }

        // �������i�椤�� Tween
        if (activeTween != null && activeTween.IsActive())
        {
            activeTween.Kill(instant);
        }

        if (instant)
        {
            // �ھ� Tween ���A�]�w������
            if (activeTween != null && activeTween.IsPlaying())
            {
                if (activeTween.Duration(false) == DurationIn)
                {
                    // �p�G�O FadeIn�A�]�w�̲׳z���׬� 0
                    ScreenImg.color = new Color(ScreenImg.color.r, ScreenImg.color.g, ScreenImg.color.b, 0);
                }
                else if (activeTween.Duration(false) == DurationOut)
                {
                    // �p�G�O FadeOut�A�]�w�̲׳z���׬� 1
                    ScreenImg.color = new Color(ScreenImg.color.r, ScreenImg.color.g, ScreenImg.color.b, 1);
                }
            }
        }
        else
        {
            // �N�z���׳]�m�� Tween �}�l�e�����A
            ScreenImg.color = new Color(ScreenImg.color.r, ScreenImg.color.g, ScreenImg.color.b, 1);
        }

        activeTween = null; // �M�z Active Tween
    }
}
