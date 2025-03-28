using UnityEngine;
using System.Collections;

public partial class Director
{
    public void Off()
    {
        TEffect.Stop();// 停止過場特效
        Avg.Background.Clear();// 清除背景sprites
        Avg.StoryPlayer.ClearPortrait();//清除頭圖
        CharDestroyAll();// 清除所有角色
    }

    public void TEffectFadeIn()
    {
        StartCoroutine(TEffectFadeInWithDelay());
    }

    public void TEffectFadeOut()
    {
        StartCoroutine(TEffectFadeOutWithDelay());
    }

    public IEnumerator TEffectFadeInWithDelay(float Delay = 0f)
    {
        yield return new WaitForSeconds(Delay);
        TEffect.FadeIn();
    }

    public IEnumerator TEffectFadeOutWithDelay(float Delay = 0f)
    {
        yield return new WaitForSeconds(Delay);
        TEffect.FadeOut();
    }
}
