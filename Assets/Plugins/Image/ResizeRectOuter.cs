using System.Collections.Generic;
using UnityEngine;

public class ResizeRectOuter : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 originalSize;

    private void Awake()
    {
        // 獲取 RectTransform
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("ResizeByScreenWidth 需要附加到擁有 RectTransform 的物件上。");
            return;
        }

        // 記錄原始寬高
        originalSize = rectTransform.sizeDelta;
    }

    private void OnEnable()
    {
        // 訂閱螢幕尺寸變更事件
        ScreenResizeDetector.OnScreenResized += Resize;

        // 初始化強制更新
        var screenResizeDetector = GetComponentInParent<ScreenResizeDetector>();
        screenResizeDetector.RefreshScreenState();
    }

    private void OnDisable()
    {
        // 取消訂閱螢幕尺寸變更事件
        ScreenResizeDetector.OnScreenResized -= Resize;
    }

    private void Resize(Dictionary<string, Vector2> ScreenStateDict)
    {
        Vector2 ResRef = ScreenStateDict["ResRef"];
        float newWidth;
        float newHeight;
        if (Screen.width > Screen.height) //landscape
        {
            newWidth = originalSize.x * (ResRef.y / ResRef.x);
            newHeight = originalSize.y * (ResRef.y / ResRef.x);
            Debug.Log($"Window Resized to LandScape {newWidth},{newHeight}");
        }
        else //portrait
        {
            newWidth = originalSize.x * (ResRef.y / originalSize.y);
            newHeight = ResRef.y;
            Debug.Log($"Window Resized to Portrait {newWidth},{newHeight}");
        }
        // 設置 RectTransform 的大小
        rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
        Debug.Log($"Screen size: {Screen.width},{Screen.height}, ratio:{ScreenStateDict["Ratio"].x},{ScreenStateDict["Ratio"].y}");
        Debug.Log($"rect resized to {newWidth},{newHeight}");
    }
}