using UnityEngine;
using DG.Tweening;
using System.Collections; // 確保已經安裝 DOTween 並使用此命名空間

public class ImageTweenPosition : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector3 initialPosition;
    private Tween moveTween;

    [Header("Tween Settings")]
    public int loopTimes = -1;
    public bool StartOnEnable = false;
    public bool ResetAfterTween = false;
    public bool DestroyAfterTween = false;
    public float targetX = 0f; // 目標位移量 X
    public float targetY = -20f; // 目標位移量 Y
    public float durForward = 0.5f; // 前往目標時間
    public float durBackward = 0.5f; // 返回初始位置的時間
    public Ease easeForward = Ease.Linear; // 前往目標的緩動效果
    public Ease easeBackward = Ease.Linear; // 返回初始位置的緩動效果
    public float delay = 0f;
    public Vector2 initial = Vector2.zero;

    /// <summary>
    /// 初始化並更新變數。
    /// </summary>
    public void Init(
        int? loopTimes = null,
        bool? startOnEnable = null,
        bool? resetAfterTween = null,
        bool? destroyAfterTween = null,
        float? targetX = null,
        float? targetY = null,
        float? durForward = null,
        float? durBackward = null,
        Ease? easeForward = null,
        Ease? easeBackward = null,
        float? delay = null,
        Vector2? initial = null
    )
    {
        if (loopTimes.HasValue) this.loopTimes = loopTimes.Value;
        if (startOnEnable.HasValue) this.StartOnEnable = startOnEnable.Value;
        if (resetAfterTween.HasValue) this.ResetAfterTween = resetAfterTween.Value;
        if (destroyAfterTween.HasValue) this.DestroyAfterTween = destroyAfterTween.Value;
        if (targetX.HasValue) this.targetX = targetX.Value;
        if (targetY.HasValue) this.targetY = targetY.Value;
        if (durForward.HasValue) this.durForward = Mathf.Max(0, durForward.Value);
        if (durBackward.HasValue) this.durBackward = Mathf.Max(0, durBackward.Value);
        if (easeForward.HasValue) this.easeForward = easeForward.Value;
        if (easeBackward.HasValue) this.easeBackward = easeBackward.Value;
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
        // 記錄初始位置
        if (initial != Vector2.zero)
        {
            rectTransform.localPosition = initial;
        }
        initialPosition = rectTransform.localPosition;

        // 開始 Tween 動畫
        if (StartOnEnable) StartCoroutine(Tween(delay));
    }

    private void OnDisable()
    {
        // 停止動畫並重置位置
        StopTween();
    }

    private IEnumerator Tween(float Delay = 0f)
    {
        yield return new WaitForSeconds(Delay);

        // 停止已有的 Tween
        moveTween?.Kill();

        // 設定目標位置
        Vector3 targetPosition = initialPosition + new Vector3(targetX, targetY, 0);

        // 創建來回循環 Tween 動畫
        moveTween = rectTransform.DOLocalMove(targetPosition, durForward)
            .SetEase(easeForward) // 前往目標的緩動效果
            .SetLoops(loopTimes, LoopType.Yoyo) // 無限來回循環
            .OnStepComplete(() =>
            {
                // 動畫方向改變時切換 Ease
                if (moveTween.CompletedLoops() % 2 == 0)
                {
                    moveTween.SetEase(easeBackward);
                }
                else
                {
                    moveTween.SetEase(easeForward);
                }
            })
            .OnComplete(() => 
            { 
                if (ResetAfterTween) rectTransform.localPosition = initialPosition;
                if (DestroyAfterTween) Destroy(gameObject); 
            });

    }

    private void StopTween()
    {
        // 停止動畫
        moveTween?.Kill();

        // 回到初始位置
        rectTransform.localPosition = initialPosition;
    }
}
