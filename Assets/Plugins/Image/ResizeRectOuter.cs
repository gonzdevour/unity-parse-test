using System.Collections.Generic;
using UnityEngine;

public class ResizeRectOuter : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 originalSize;

    private void Awake()
    {
        // ��� RectTransform
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("ResizeByScreenWidth �ݭn���[��֦� RectTransform ������W�C");
            return;
        }

        // �O����l�e��
        originalSize = rectTransform.sizeDelta;
    }

    private void OnEnable()
    {
        // �q�\�ù��ؤo�ܧ�ƥ�
        ScreenResizeDetector.OnScreenResized += Resize;

        // ��l�Ʊj���s
        var screenResizeDetector = GetComponentInParent<ScreenResizeDetector>();
        screenResizeDetector.RefreshScreenState();
    }

    private void OnDisable()
    {
        // �����q�\�ù��ؤo�ܧ�ƥ�
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
        // �]�m RectTransform ���j�p
        rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
        Debug.Log($"Screen size: {Screen.width},{Screen.height}, ratio:{ScreenStateDict["Ratio"].x},{ScreenStateDict["Ratio"].y}");
        Debug.Log($"rect resized to {newWidth},{newHeight}");
    }
}