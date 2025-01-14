using UnityEngine;
using System.Collections;

public partial class Director
{
    public void Off()
    {
        TEffect.Stop();// ����L���S��
        Avg.Background.Clear();// �M���I��sprites
        if (Avg.LayerChar != null)
        {
            foreach (Transform child in Avg.LayerChar)
            {
                GameObject.Destroy(child.gameObject);// �M������
            }
        }
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
