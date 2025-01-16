// �ù��ؤo�˴���
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

        // ��� CanvasScaler
        CanvasScaler canvasScaler = transform.GetComponent<CanvasScaler>();
        // ���o�w�]���
        var ResRef = canvasScaler.referenceResolution;
        // ���o�Y����
        var ScreenSizeRatio = new Vector2(Screen.width / ResRef.x, Screen.height / ResRef.y);

        Dictionary<string, Vector2> ScreenStateDict = new()
            {
                {"ResRef", ResRef},
                {"Ratio", ScreenSizeRatio},
            };

        Debug.Log($"�w�]��G{ResRef}�A��̡G{lastScreenSize}�A�Y���{ScreenSizeRatio}");
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