using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TransitionEffectAnim : MonoBehaviour, ITransitionEffect
{
    private GameObject UnmaskCtnr;
    private Image ScreenImg;
    private float DurOut;
    private float DurIn;
    private Ease EaseOut;
    private Ease EaseIn;
    private Ease EaseCurrent;
    private Tween activeTween;
    private System.Action onComplete; // 用於保存回調參數
    private AnimEvents animEvents;
    private float animDirection;

    public void Init(TransitionEffectConfig config)
    {
        UnmaskCtnr = config.UnmaskCtnr;
        ScreenImg = config.ScreenImg;
        DurOut = config.DurOut;
        DurIn = config.DurIn;
        EaseOut = config.EaseOut;
        EaseIn = config.EaseIn;

        activeTween = null;
    }

    private string prefabName = "Unmask_Water01";

    public void FadeIn(System.Action onComplete)
    {
        this.onComplete = onComplete;
        //Debug.Log("circle fade in");
        // 停止任何進行中的 Tween，初始化
        Stop(true);
        ScreenImg.gameObject.SetActive(true);

        // 產生Unmask
        GameObject prefab = Resources.Load<GameObject>($"prefabs/{prefabName}");
        var inst = Instantiate(prefab, Vector3.zero, Quaternion.identity, UnmaskCtnr.transform);
        var RTrans = inst.GetComponent<RectTransform>();

        // 訂閱回調事件
        animEvents = inst.GetComponent<AnimEvents>();
        animEvents.OnAnimationStartCallback += OnAnimationStart;
        animEvents.OnAnimationEndCallback += OnAnimationEnd;

        // 取得animator
        var animator = inst.GetComponent<Animator>();

        // 將Local Position 歸 0
        RTrans.localPosition = Vector3.zero;

        // 設定size為Screen長邊
        float screenMaxDimension = Mathf.Max(ScreenImg.rectTransform.rect.width, ScreenImg.rectTransform.rect.height);
        RTrans.sizeDelta = new Vector2(screenMaxDimension, screenMaxDimension);

        //播放動畫
        animDirection = -1;
        animator.SetFloat("Direction", -1);
        animator.Play("TEffect_Water01", -1, float.NegativeInfinity);
    }

    public void FadeOut(System.Action onComplete)
    {
        this.onComplete = onComplete;
        //Debug.Log("circle fade out");
        // 停止任何進行中的 Tween，初始化
        Stop(true);
        ScreenImg.gameObject.SetActive(true);

        // 產生Unmask
        GameObject prefab = Resources.Load<GameObject>($"prefabs/{prefabName}");
        var inst = Instantiate(prefab, Vector3.zero, Quaternion.identity, UnmaskCtnr.transform);
        var RTrans = inst.GetComponent<RectTransform>();

        // 訂閱回調事件
        animEvents = inst.GetComponent<AnimEvents>();
        animEvents.OnAnimationStartCallback += OnAnimationStart;
        animEvents.OnAnimationEndCallback += OnAnimationEnd;

        // 取得animator
        var animator = inst.GetComponent<Animator>();

        // 將Local Position 歸 0
        RTrans.localPosition = Vector3.zero;

        // 設定size為Screen長邊
        float screenMaxDimension = Mathf.Max(ScreenImg.rectTransform.rect.width, ScreenImg.rectTransform.rect.height);
        RTrans.sizeDelta = new Vector2(screenMaxDimension, screenMaxDimension);

        //播放動畫
        animDirection = 1;
        animator.speed = 1; // 正向播放
        animator.Play("TEffect_Water01", -1, 0f); // 從動畫起點開始
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

    private void OnAnimationEnd()
    {
        if (animDirection == 1) //fadeOut
        {
            Debug.Log("OnAnimationEnd");
            // 取消訂閱，防止回調再次觸發
            animEvents.OnAnimationEndCallback -= OnAnimationEnd;

            activeTween = null;
            ScreenImg.gameObject.SetActive(true);
            foreach (Transform child in UnmaskCtnr.transform)
            {
                Destroy(child.gameObject); // 刪除所有Unmask
            }
            onComplete?.Invoke();
            onComplete = null;
        }
        else
        {
            ScreenImg.gameObject.SetActive(true); //fadeIn
        }

    }

    private void OnAnimationStart()
    {
        if (animDirection == -1) //fadeIn
        {
            Debug.Log("OnAnimationStart");
            // 取消訂閱，防止回調再次觸發
            animEvents.OnAnimationStartCallback -= OnAnimationStart;

            activeTween = null;
            ScreenImg.gameObject.SetActive(false);
            foreach (Transform child in UnmaskCtnr.transform)
            {
                Destroy(child.gameObject); // 刪除所有Unmask
            }
            onComplete?.Invoke();
            onComplete = null;
        }
        else
        {
            ScreenImg.gameObject.SetActive(true); //fadeOut
        }

    }
}
