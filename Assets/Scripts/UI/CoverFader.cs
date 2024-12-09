using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class CoverFader : MonoBehaviour
{
    public Image Cover;
    private readonly float Duration = 0.5f;

    void Start()
    {
        FadeIn(); // 開始時淡入
    }

    public void FadeIn()
    {
        Debug.Log($"FadeIn Dur:{Duration}");
        //Cover.color = new Color(0, 0, 0, 1); // 確保初始是全黑
        Cover.DOFade(0, Duration).SetEase(Ease.InQuad); // 使用 DOTween 淡入，時間可自行調整
    }

    public void FadeOut(string targetScene)
    {
        Cover.DOFade(1, Duration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            Debug.Log($"FadeOut Dur:{Duration}");
            SceneManager.LoadScene(targetScene); // 淡出完成後切換場景
        });
    }
}
