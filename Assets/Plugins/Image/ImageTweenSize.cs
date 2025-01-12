using UnityEngine;
using DG.Tweening;
using System.Collections; // 確保已經安裝 DOTween 並使用此命名空間

public class ImageTweenSize : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 initialSize;
    private Tween sizeTween;

    [Header("Tween Settings")]
    public int loopTimes = -1;
    public bool StartOnEnable = false;
    public bool ResetAfterTween = false;
    public bool DestroyAfterTween = false;
    public float relativeWidth = 0.1f; // 相對寬度（比例）
    public float relativeHeight = 0.1f; // 相對高度（比例）
    public float durIncrease = 0.5f; // 增大尺寸持續時間
    public float durDecrease = 0.5f; // 減小尺寸持續時間
    public Ease easeIncrease = Ease.Linear; // 增大尺寸的緩動效果
    public Ease easeDecrease = Ease.Linear; // 減小尺寸的緩動效果
    public float delay = 0f; // 動畫延遲
    public Vector2 initial = Vector2.zero;

    /// <summary>
    /// 初始化並更新變數。
    /// </summary>
    public void Init(
        int? loopTimes = null,
        bool? startOnEnable = null,
        bool? resetAfterTween = null,
        bool? destroyAfterTween = null,
        float? relativeWidth = null,
        float? relativeHeight = null,
        float? durIncrease = null,
        float? durDecrease = null,
        Ease? easeIncrease = null,
        Ease? easeDecrease = null,
        float? delay = null,
        Vector2? initial = null
    )
    {
        if (loopTimes.HasValue) this.loopTimes = loopTimes.Value;
        if (startOnEnable.HasValue) this.StartOnEnable = startOnEnable.Value;
        if (resetAfterTween.HasValue) this.ResetAfterTween = resetAfterTween.Value;
        if (destroyAfterTween.HasValue) this.DestroyAfterTween = destroyAfterTween.Value;
        if (relativeWidth.HasValue) this.relativeWidth = Mathf.Clamp01(relativeWidth.Value);
        if (relativeHeight.HasValue) this.relativeHeight = Mathf.Clamp01(relativeHeight.Value);
        if (durIncrease.HasValue) this.durIncrease = Mathf.Max(0, durIncrease.Value);
        if (durDecrease.HasValue) this.durDecrease = Mathf.Max(0, durDecrease.Value);
        if (easeIncrease.HasValue) this.easeIncrease = easeIncrease.Value;
        if (easeDecrease.HasValue) this.easeDecrease = easeDecrease.Value;
        if (delay.HasValue) this.delay = Mathf.Max(0, delay.Value);
        if (initial.HasValue) this.initial = initial.Value;
    }

    private void Awake()
    {
        // 獲取 RectTransform
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("此腳本必須掛載在擁有 RectTransform 的物件上！");
            enabled = false;
        }
    }

    private void OnEnable()
    {
        // 記錄初始尺寸
        if (initial != Vector2.zero)
        {
            rectTransform.sizeDelta = initial;
        }
        initialSize = rectTransform.sizeDelta;

        // 開始 Tween 動畫
        if (StartOnEnable) StartCoroutine(Tween(delay));
    }

    private void OnDisable()
    {
        // 停止動畫並重置尺寸
        StopTween();
    }

    private IEnumerator Tween(float Delay = 0f)
    {
        yield return new WaitForSeconds(Delay);

        // 停止已有的 Tween
        sizeTween?.Kill();

        // 設定目標尺寸為相對於初始尺寸的比例
        Vector2 targetSize = new Vector2(
            initialSize.x * (1 + relativeWidth),
            initialSize.y * (1 + relativeHeight)
        );

        // 創建來回循環 Tween 動畫
        sizeTween = rectTransform.DOSizeDelta(targetSize, durIncrease)
            .SetEase(easeIncrease) // 增大尺寸的緩動效果
            .SetLoops(loopTimes, LoopType.Yoyo) // 無限來回循環
            .OnStepComplete(() =>
            {
                // 動畫方向改變時切換 Ease
                if (sizeTween.IsPlaying())
                {
                    sizeTween.SetEase(sizeTween.CompletedLoops() % 2 == 0 ? easeDecrease : easeIncrease);
                }
            })
            .OnComplete(() => 
            { 
                if (ResetAfterTween) rectTransform.sizeDelta = initialSize;
                if (DestroyAfterTween) Destroy(gameObject); 
            });
    }

    private void StopTween()
    {
        // 停止動畫
        sizeTween?.Kill();

        // 回到初始尺寸
        rectTransform.sizeDelta = initialSize;
    }
}
