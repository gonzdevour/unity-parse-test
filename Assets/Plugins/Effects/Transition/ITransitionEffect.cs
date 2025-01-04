using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TransitionEffectConfig
{
    public GameObject UnmaskCtnr { get; set; }
    public Image ScreenImg { get; set; }
    public float DurOut { get; set; }
    public float DurIn { get; set; }
    public Ease EaseOut { get; set; }
    public Ease EaseIn { get; set; }
}

public interface ITransitionEffect
{
    void Init(TransitionEffectConfig config);
    void FadeIn(System.Action onComplete = null);
    void FadeOut(System.Action onComplete = null);
    void Stop(bool instant = true);
}