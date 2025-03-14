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
        if (boundRect == null) boundRect = gameObject.GetComponent<RectTransform>(); // 預設為 Canvas

        // 獲取真實大小 (避免使用 sizeDelta)
        float width = rect.rect.width;
        float height = rect.rect.height;
        float boundWidth = boundRect.rect.width;
        float boundHeight = boundRect.rect.height;

        // 修正：考慮 Pivot 影響 (Y 軸)
        Vector2 minBounds = new Vector2(
            -boundWidth / 2 + width * rect.pivot.x + padding,  // X 最小值
            -boundHeight / 2 + height * (rect.pivot.y) + padding // Y 最小值修正
        );

        Vector2 maxBounds = new Vector2(
            boundWidth / 2 - width * (1 - rect.pivot.x) - padding,  // X 最大值
            boundHeight / 2 - height * (1 - rect.pivot.y) - padding // Y 最大值修正
        );

        // 限制 anchoredPosition，確保 UI 在留白範圍內
        Vector2 clampedPosition = rect.anchoredPosition;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minBounds.x, maxBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minBounds.y, maxBounds.y);

        // 更新 UI 位置
        rect.anchoredPosition = clampedPosition;

        // Debug 訊息
        Debug.Log($"[ClampToBounds] 原始位置: {rect.anchoredPosition}");
        Debug.Log($"[ClampToBounds] 修正後位置: {clampedPosition}");
    }

}
