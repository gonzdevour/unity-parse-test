using UnityEngine;
using DG.Tweening; // 確保已經安裝 DOTween 並使用此命名空間

public class ImageTweenPosition : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector3 initialPosition;
    private Tween moveTween;

    [Header("Tween Settings")]
    public float targetX = 0f; // 目標位移量 X
    public float targetY = -20f; // 目標位移量 Y
    public float durForward = 0.5f; // 目標位移量 X
    public float durBackward = 0.5f; // 目標位移量 Y
    public Ease easeForward = Ease.InOutSine; // 前往目標的緩動效果
    public Ease easeBackward = Ease.InOutSine; // 返回初始位置的緩動效果

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
        initialPosition = rectTransform.localPosition;

        // 開始 Tween 動畫
        StartBounceTween();
    }

    private void OnDisable()
    {
        // 停止動畫並重置位置
        StopBounceTween();
    }

    private void StartBounceTween()
    {
        // 停止已有的 Tween
        moveTween?.Kill();

        // 設定目標位置
        Vector3 targetPosition = initialPosition + new Vector3(targetX, targetY, 0);

        // 創建來回循環 Tween 動畫
        moveTween = rectTransform.DOLocalMove(targetPosition, 0.5f)
            .SetEase(easeForward) // 前往目標的緩動效果
            .SetLoops(-1, LoopType.Yoyo) // 無限來回循環
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
            });

    }

    private void StopBounceTween()
    {
        // 停止動畫
        moveTween?.Kill();

        // 回到初始位置
        rectTransform.localPosition = initialPosition;
    }
}
