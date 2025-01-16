// 螢幕尺寸檢測器
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenResizeDetector : MonoBehaviour
{
    public static event System.Action<Dictionary<string, Vector2>> OnScreenResized;

    private Vector2 lastScreenSize;

    public void RefreshScreenState()
    {
        lastScreenSize = new Vector2(Screen.width, Screen.height);

        // 獲取 CanvasScaler
        CanvasScaler canvasScaler = transform.GetComponent<CanvasScaler>();
        // 取得預設比例
        var ResRef = canvasScaler.referenceResolution;
        // 取得縮放比例
        var ScreenSizeRatio = new Vector2(Screen.width / ResRef.x, Screen.height / ResRef.y);

        Dictionary<string, Vector2> ScreenStateDict = new()
            {
                {"ResRef", ResRef},
                {"Ratio", ScreenSizeRatio},
            };

        Debug.Log($"預設比：{ResRef}，實屏：{lastScreenSize}，縮放比{ScreenSizeRatio}");
        OnScreenResized?.Invoke(ScreenStateDict);
    }

    private void Update()
    {
        if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y)
        {
            RefreshScreenState();
        }
    }
}