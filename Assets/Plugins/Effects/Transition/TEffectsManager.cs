using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TEffectsManager : MonoBehaviour
{
    public GameObject UnmaskCtnr;
    public Image ScreenImg;
    public Transform TEffectsPack;
    public float DurOut = 0.5f;
    public float DurIn = 0.5f;
    public Ease EaseOut = Ease.OutQuad;
    public Ease EaseIn = Ease.InQuad;

    private ITransitionEffect TEffect;

    /// <summary>
    /// ��l�ƹL���ʵe
    /// </summary>
    /// <param name="effectName">�S�ĥN�٦r��A�p"Fade"�B"Circle"</param>
    public ITransitionEffect Init(string effectName)
    {
        TEffect = TEffectsPack.GetComponent($"TransitionEffect{effectName}") as ITransitionEffect;
        TransitionEffectConfig config = new()
        {
            UnmaskCtnr = UnmaskCtnr,
            ScreenImg = ScreenImg,
            DurOut = DurOut,
            DurIn = DurIn,
            EaseOut = EaseOut,
            EaseIn = EaseIn
        };
        TEffect.Init(config);

        return TEffect;
    }
}
