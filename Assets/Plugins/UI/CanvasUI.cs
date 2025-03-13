using UnityEngine;

public class CanvasUI : MonoBehaviour
{
    public GameObject cover;
    public PanelLoadingProgress panelLoadingProgress;
    public PanelSpinner panelSpinner;
    // 單例模式
    public static CanvasUI Inst { get; private set; }

    private void Awake()
    {
        // 確保單例唯一性
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject); // 場景切換時保持不銷毀

        cover.SetActive(true);
        panelSpinner.gameObject.SetActive(false);
        panelLoadingProgress.gameObject.SetActive(false);
    }

    public void ClampToBounds(RectTransform rect, float padding = 0, RectTransform boundRect = null)
    {
        if (boundRect == null) boundRect = gameObject.GetComponent<RectTransform>(); // boundRect 預設為 Canvas

        // 取得原始 anchoredPosition
        Vector2 originalPosition = rect.anchoredPosition;
        Debug.Log($"[ClampToBounds] 原始位置: {originalPosition}");

        // 計算最小邊界 (確保 UI 左下角不超出畫面)
        Vector2 minBounds = new Vector2(
            rect.sizeDelta.x / 2 + padding,
            rect.sizeDelta.y / 2 + padding
        );

        // 計算最大邊界 (確保 UI 右上角不超出畫面)
        Vector2 maxBounds = new Vector2(
            boundRect.sizeDelta.x - rect.sizeDelta.x / 2 - padding,
            boundRect.sizeDelta.y - rect.sizeDelta.y / 2 - padding
        );

        // 限制 anchoredPosition，確保 UI 在留白範圍內
        Vector2 clampedPosition = rect.anchoredPosition;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minBounds.x, maxBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minBounds.y, maxBounds.y);

        // 更新 UI 位置
        rect.anchoredPosition = clampedPosition;

        // 取得修正後的 anchoredPosition
        Debug.Log($"[ClampToBounds] 修正後位置: {clampedPosition}");
    }

}
