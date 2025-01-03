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
        // 停止任何進行中的 Tween，初始化
        Stop(true);
        ScreenImg.gameObject.SetActive(true);

        // 產生Unmask
        GameObject prefab = Resources.Load<GameObject>($"prefabs/{prefabName}");
        var inst = Instantiate(prefab, Vector3.zero, Quaternion.identity, UnmaskCtnr.transform);
        var circleRectTransform = inst.GetComponent<RectTransform>();

        // 將 circle 的 Local Position 歸 0
        circleRectTransform.localPosition = Vector3.zero;

        // 設置 circle 的初始寬高為 0
        circleRectTransform.sizeDelta = Vector2.zero;

        // 取得屏幕長邊
        float screenMaxDimension = Mathf.Max(Screen.width, Screen.height);

        // Tween 到 (長邊, 長邊)
        EaseCurrent = EaseIn;
        activeTween = circleRectTransform.DOSizeDelta(new Vector2(screenMaxDimension, screenMaxDimension), DurationIn)
            .SetEase(EaseCurrent)
            .OnComplete(() =>
            {
                activeTween = null;
                ScreenImg.gameObject.SetActive(false);
                foreach (Transform child in UnmaskCtnr.transform)
                {
                    Destroy(child.gameObject); // 刪除所有Unmask
                }
                onComplete?.Invoke();
            });
    }

    public void FadeOut(System.Action onComplete)
    {
        // 停止任何進行中的 Tween，初始化
        Stop(true);
        ScreenImg.gameObject.SetActive(true);

        // 產生Unmask
        GameObject prefab = Resources.Load<GameObject>($"prefabs/{prefabName}");
        var inst = Instantiate(prefab, Vector3.zero, Quaternion.identity, UnmaskCtnr.transform);
        var circleRectTransform = inst.GetComponent<RectTransform>();

        // 將 circle 的 Local Position 歸 0
        circleRectTransform.localPosition = Vector3.zero;

        // 設置 circle 的初始大小為屏幕長邊
        float screenMaxDimension = Mathf.Max(Screen.width, Screen.height);
        circleRectTransform.sizeDelta = new Vector2(screenMaxDimension, screenMaxDimension);

        // Tween 到 (0, 0)
        EaseCurrent = EaseOut;
        activeTween = circleRectTransform.DOSizeDelta(Vector2.zero, DurationOut)
            .SetEase(EaseCurrent)
            .OnComplete(() =>
            {
                activeTween = null;
                ScreenImg.gameObject.SetActive(true);
                foreach (Transform child in UnmaskCtnr.transform)
                {
                    Destroy(child.gameObject); // 刪除所有Unmask
                }
                onComplete?.Invoke();
            });
    }

    public void Stop(bool instant)
    {
        // 停止任何正在進行的 Tween
        if (activeTween != null && activeTween.IsActive())
        {
            activeTween.Kill(true);
        }

        if (instant)
        {
            if (activeTween != null && activeTween.IsPlaying())
            {
                // 根據 Tween 是 FadeIn 或 FadeOut 判斷
                if (EaseCurrent == EaseIn)
                {
                    // FadeIn 的最終狀態
                    ScreenImg.gameObject.SetActive(false);
                    foreach (Transform child in UnmaskCtnr.transform)
                    {
                        Destroy(child.gameObject); // 刪除所有Unmask
                    }
                }
                else if (EaseCurrent == EaseOut)
                {
                    // FadeOut 的最終狀態
                    ScreenImg.gameObject.SetActive(true);
                    foreach (Transform child in UnmaskCtnr.transform)
                    {
                        Destroy(child.gameObject); // 刪除所有Unmask
                    }
                }
            }
        }
        else
        {
            if (activeTween != null && activeTween.IsPlaying())
            {
                // 根據 Tween 是 FadeIn 或 FadeOut 判斷
                if (EaseCurrent == EaseIn)
                {
                    // FadeIn 的最終狀態
                    ScreenImg.gameObject.SetActive(true);
                }
                else if (EaseCurrent == EaseOut)
                {
                    // FadeOut 的最終狀態
                    ScreenImg.gameObject.SetActive(false);
                }
                foreach (Transform child in UnmaskCtnr.transform)
                {
                    Destroy(child.gameObject); // 刪除所有Unmask
                }
            }
        }

        activeTween = null; // 清理 Active Tween
    }
}
