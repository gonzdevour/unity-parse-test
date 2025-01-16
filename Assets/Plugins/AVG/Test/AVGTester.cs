using UnityEngine;
using UnityEngine.UI;

public class AVGTester : MonoBehaviour
{
    public Button Btn_TEffectFadeIn;
    public Button Btn_TEffectFadeOut;

    private void Awake()
    {
        Btn_TEffectFadeIn.onClick.AddListener(TEffectFadeIn);
        Btn_TEffectFadeOut.onClick.AddListener(TEffectFadeOut);
    }

    private void TEffectFadeIn()
    {
        Director.Inst.TEffectFadeIn();
    }
    private void TEffectFadeOut()
    {
        Director.Inst.TEffectFadeOut();
    }
}
