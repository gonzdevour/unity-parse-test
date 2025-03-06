using UnityEngine;
using DG.Tweening;
using System.Collections; // 確保已經安裝 DOTween 並使用此命名空間

public class ImageTweenAngle : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector3 initialRotation;
    private Tween angleTween;

    [Header("Tween Settings")]
    public int loopTimes = -1;
    public bool StartOnEnable = false;
    public bool ResetAfterTween = false;
    public bool DestroyAfterTween = false;
    public float relativeAngle = 30f; // 相對旋轉角度（度數）
    public float durIncrease = 0.5f; // 增加旋轉持續時間
    public float durDecrease = 0.5f; // 減少旋轉持續時間
    public Ease easeIncrease = Ease.Linear; // 增加旋轉的緩動效果
    public Ease easeDecrease = Ease.Linear; // 減少旋轉的緩動效果
    public float delay = 0f; // 動畫延遲
    public float initial = -1f;

    /// <summary>
    /// 初始化並更新變數。
    /// </summary>
    public void Init(
        int? loopTimes = null,
        bool? startOnEnable = null,
        bool? resetAfterTween = null,
        bool? destroyAfterTween = null,
        float? relativeAngle = null,
        float? durIncrease = null,
        float? durDecrease = null,
        Ease? easeIncrease = null,
        Ease? easeDecrease = null,
        float? delay = null,
        float? initial = null
    )
    {
        if (loopTimes.HasValue) this.loopTimes = loopTimes.Value;
        if (startOnEnable.HasValue) this.StartOnEnable = startOnEnable.Value;
        if (resetAfterTween.HasValue) this.ResetAfterTween = resetAfterTween.Value;
        if (destroyAfterTween.HasValue) this.DestroyAfterTween = destroyAfterTween.Value;
        if (relativeAngle.HasValue) this.relativeAngle = relativeAngle.Value;
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
        // 初始角度
        if (initial != -1f)
        {
            rectTransform.localEulerAngles = new Vector3(0, 0, initial);
        }
        // 記錄初始旋轉角度
        initialRotation = rectTransform.localEulerAngles;

        // 開始 Tween 動畫
        if (StartOnEnable) StartCoroutine(Tween(delay));
    }

    private void OnDisable()
    {
        // 停止動畫並重置旋轉
        StopTween();
    }

    private IEnumerator Tween(float Delay = 0f)
    {
        yield return new WaitForSeconds(Delay);

        // 停止已有的 Tween
        angleTween?.Kill();

        // 設定目標旋轉角度為相對於初始旋轉的比例
        Vector3 targetRotation = initialRotation + new Vector3(0, 0, relativeAngle);

        // 創建來回循環 Tween 動畫
        angleTween = rectTransform.DOLocalRotate(targetRotation, durIncrease, RotateMode.FastBeyond360)
            .SetEase(easeIncrease) // 增加旋轉的緩動效果
            .SetLoops(loopTimes, LoopType.Yoyo) // 無限來回循環
            .OnStepComplete(() =>
            {
                // 動畫方向改變時切換 Ease
                if (angleTween.IsPlaying())
                {
                    angleTween.SetEase(angleTween.CompletedLoops() % 2 == 0 ? easeDecrease : easeIncrease);
                }
            })
            .OnComplete(() => 
            {
                if (ResetAfterTween) rectTransform.localEulerAngles = initialRotation;
                if (DestroyAfterTween) Destroy(gameObject); 
            });
    }

    private void StopTween()
    {
        // 停止動畫
        angleTween?.Kill();

        // 回到初始旋轉
        rectTransform.localEulerAngles = initialRotation;
    }
}
