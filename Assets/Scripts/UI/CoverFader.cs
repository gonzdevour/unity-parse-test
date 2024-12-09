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
        FadeIn(); // �}�l�ɲH�J
    }

    public void FadeIn()
    {
        Debug.Log($"FadeIn Dur:{Duration}");
        //Cover.color = new Color(0, 0, 0, 1); // �T�O��l�O����
        Cover.DOFade(0, Duration).SetEase(Ease.InQuad); // �ϥ� DOTween �H�J�A�ɶ��i�ۦ�վ�
    }

    public void FadeOut(string targetScene)
    {
        Cover.DOFade(1, Duration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            Debug.Log($"FadeOut Dur:{Duration}");
            SceneManager.LoadScene(targetScene); // �H�X�������������
        });
    }
}
