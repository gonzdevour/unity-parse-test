using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TransitionEffectCircle : MonoBehaviour, ITransitionEffect
{
    private GameObject UnmaskCtnr;
    private Image ScreenImg;
    private float DurationOut;
    private float DurationIn;
    private Ease EaseOut;
    private Ease EaseIn;
    private Ease EaseCurrent;
    private Tween activeTween;

    public TransitionEffectCircle(GameObject UnmaskCtnr, Image ScreenImg, float DurOut, float DurIn, Ease EaseOut, Ease EaseIn)
    {
        this.UnmaskCtnr = UnmaskCtnr;
        this.ScreenImg = ScreenImg;
        this.DurationOut = DurOut;
        this.DurationIn = DurIn;
        this.EaseOut = EaseOut;
        this.EaseIn = EaseIn;
        activeTween = null;
    }

    private string prefabName = "Unmask";

    public void FadeIn(System.Action onComplete)
    {
        // �������i�椤�� Tween�A��l��
        Stop(true);
        ScreenImg.gameObject.SetActive(true);

        // ����Unmask
        GameObject prefab = Resources.Load<GameObject>($"prefabs/{prefabName}");
        var inst = Instantiate(prefab, Vector3.zero, Quaternion.identity, UnmaskCtnr.transform);
        var circleRectTransform = inst.GetComponent<RectTransform>();

        // �N circle �� Local Position �k 0
        circleRectTransform.localPosition = Vector3.zero;

        // �]�m circle ����l�e���� 0
        circleRectTransform.sizeDelta = Vector2.zero;

        // ���o�̹�����
        float screenMaxDimension = Mathf.Max(Screen.width, Screen.height);

        // Tween �� (����, ����)
        EaseCurrent = EaseIn;
        activeTween = circleRectTransform.DOSizeDelta(new Vector2(screenMaxDimension, screenMaxDimension), DurationIn)
            .SetEase(EaseCurrent)
            .OnComplete(() =>
            {
                activeTween = null;
                ScreenImg.gameObject.SetActive(false);
                foreach (Transform child in UnmaskCtnr.transform)
                {
                    Destroy(child.gameObject); // �R���Ҧ�Unmask
                }
                onComplete?.Invoke();
            });
    }

    public void FadeOut(System.Action onComplete)
    {
        // �������i�椤�� Tween�A��l��
        Stop(true);
        ScreenImg.gameObject.SetActive(true);

        // ����Unmask
        GameObject prefab = Resources.Load<GameObject>($"prefabs/{prefabName}");
        var inst = Instantiate(prefab, Vector3.zero, Quaternion.identity, UnmaskCtnr.transform);
        var circleRectTransform = inst.GetComponent<RectTransform>();

        // �N circle �� Local Position �k 0
        circleRectTransform.localPosition = Vector3.zero;

        // �]�m circle ����l�j�p���̹�����
        float screenMaxDimension = Mathf.Max(Screen.width, Screen.height);
        circleRectTransform.sizeDelta = new Vector2(screenMaxDimension, screenMaxDimension);

        // Tween �� (0, 0)
        EaseCurrent = EaseOut;
        activeTween = circleRectTransform.DOSizeDelta(Vector2.zero, DurationOut)
            .SetEase(EaseCurrent)
            .OnComplete(() =>
            {
                activeTween = null;
                ScreenImg.gameObject.SetActive(true);
                foreach (Transform child in UnmaskCtnr.transform)
                {
                    Destroy(child.gameObject); // �R���Ҧ�Unmask
                }
                onComplete?.Invoke();
            });
    }

    public void Stop(bool instant)
    {
        // ������󥿦b�i�檺 Tween
        if (activeTween != null && activeTween.IsActive())
        {
            activeTween.Kill(true);
        }

        if (instant)
        {
            if (activeTween != null && activeTween.IsPlaying())
            {
                // �ھ� Tween �O FadeIn �� FadeOut �P�_
                if (EaseCurrent == EaseIn)
                {
                    // FadeIn ���̲ת��A
                    ScreenImg.gameObject.SetActive(false);
                    foreach (Transform child in UnmaskCtnr.transform)
                    {
                        Destroy(child.gameObject); // �R���Ҧ�Unmask
                    }
                }
                else if (EaseCurrent == EaseOut)
                {
                    // FadeOut ���̲ת��A
                    ScreenImg.gameObject.SetActive(true);
                    foreach (Transform child in UnmaskCtnr.transform)
                    {
                        Destroy(child.gameObject); // �R���Ҧ�Unmask
                    }
                }
            }
        }
        else
        {
            if (activeTween != null && activeTween.IsPlaying())
            {
                // �ھ� Tween �O FadeIn �� FadeOut �P�_
                if (EaseCurrent == EaseIn)
                {
                    // FadeIn ���̲ת��A
                    ScreenImg.gameObject.SetActive(true);
                }
                else if (EaseCurrent == EaseOut)
                {
                    // FadeOut ���̲ת��A
                    ScreenImg.gameObject.SetActive(false);
                }
                foreach (Transform child in UnmaskCtnr.transform)
                {
                    Destroy(child.gameObject); // �R���Ҧ�Unmask
                }
            }
        }

        activeTween = null; // �M�z Active Tween
    }
}
