using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Collections;

public class TransitionImage : MonoBehaviour
{
    public Image activeImage; // 當前顯示的圖片
    public Image readyImage;  // 下一步將顯示的圖片

    private Dictionary<string, Action<float>> transitionEffects; // 儲存特效對應方法
    private Tween currentTween; // 當前執行的過渡動畫

    private void Awake()
    {
        // 初始化特效字典
        transitionEffects = new Dictionary<string, Action<float>>
        {
            { "fade", FadeTransition },
            { "slide", SlideTransition },
            { "slideright", SlideRightTransition },
            { "slidedown", SlideDownTransition },
            { "slideup", SlideUpTransition },
            { "scale", ScaleTransition },
            { "scale2", Scale2Transition },
        };
    }

    /// <summary>
    /// 開始圖片轉換
    /// </summary>
    /// <param name="imagePath">新圖片的路徑</param>
    /// <param name="transitionType">過渡特效類型</param>
    /// <param name="dur">過渡特效時間</param>
    /// <param name="onComplete">過渡完成時的回調</param>
    public void StartTransition(string imagePath, string transitionType, float dur = 2f, Action onComplete = null)
    {
        // 若有正在執行的過渡效果，先完成它
        CompleteCurrentTransition();

        // 更換 ReadyImage 圖片
        Sprite newSprite = LoadSpriteFromPath(imagePath);
        if (newSprite == null)
        {
            Debug.LogError($"圖片路徑無效: {imagePath}");
            return;
        }
        readyImage.sprite = newSprite;
        readyImage.gameObject.SetActive(true); // 顯示 ReadyImage

        // 檢查並執行特效
        if (transitionEffects.ContainsKey(transitionType.ToLower()))
        {
            transitionEffects[transitionType.ToLower()](dur);
        }
        else
        {
            Debug.LogError($"無效的過渡特效類型: {transitionType}");
        }

        // 設置完成時的回調
        if (onComplete != null)
        {
            currentTween?.OnComplete(() => onComplete());
        }
    }

    /// <summary>
    /// 淡入淡出過渡 (DOTween 實現)
    /// </summary>
    private void FadeTransition(float duration)
    {
        readyImage.color = new Color(1, 1, 1, 0); // 初始化 ReadyImage
        currentTween = DOTween.Sequence()
            .Append(activeImage.DOFade(0, duration).OnComplete(() =>
            {
                activeImage.gameObject.SetActive(false);
                activeImage.color = new Color(1, 1, 1, 1); // 恢復透明度
            }))
            .Join(readyImage.DOFade(1, duration).OnComplete(SwapImages));
    }

    /// <summary>
    /// 從左到右滑動過渡 (DOTween 實現)
    /// </summary>
    private void SlideRightTransition(float duration)
    {
        float width = activeImage.rectTransform.rect.width;
        readyImage.rectTransform.anchoredPosition = new Vector2(-width, 0); // 從左側滑入

        currentTween = DOTween.Sequence()
            .Append(activeImage.rectTransform.DOAnchorPos(new Vector2(width, 0), duration).OnComplete(() =>
            {
                activeImage.gameObject.SetActive(false);
                activeImage.rectTransform.anchoredPosition = Vector2.zero; // 重置位置
            }))
            .Join(readyImage.rectTransform.DOAnchorPos(Vector2.zero, duration).OnComplete(SwapImages));
    }

    /// <summary>
    /// 從上到下滑動過渡 (DOTween 實現)
    /// </summary>
    private void SlideDownTransition(float duration)
    {
        float height = activeImage.rectTransform.rect.height;
        readyImage.rectTransform.anchoredPosition = new Vector2(0, height); // 從上方滑入

        currentTween = DOTween.Sequence()
            .Append(activeImage.rectTransform.DOAnchorPos(new Vector2(0, -height), duration).OnComplete(() =>
            {
                activeImage.gameObject.SetActive(false);
                activeImage.rectTransform.anchoredPosition = Vector2.zero; // 重置位置
            }))
            .Join(readyImage.rectTransform.DOAnchorPos(Vector2.zero, duration).OnComplete(SwapImages));
    }

    /// <summary>
    /// 從下到上滑動過渡 (DOTween 實現)
    /// </summary>
    private void SlideUpTransition(float duration)
    {
        float height = activeImage.rectTransform.rect.height;
        readyImage.rectTransform.anchoredPosition = new Vector2(0, -height); // 從下方滑入

        currentTween = DOTween.Sequence()
            .Append(activeImage.rectTransform.DOAnchorPos(new Vector2(0, height), duration).OnComplete(() =>
            {
                activeImage.gameObject.SetActive(false);
                activeImage.rectTransform.anchoredPosition = Vector2.zero; // 重置位置
            }))
            .Join(readyImage.rectTransform.DOAnchorPos(Vector2.zero, duration).OnComplete(SwapImages));
    }

    /// <summary>
    /// 滑動過渡 (DOTween 實現)
    /// </summary>
    private void SlideTransition(float duration)
    {
        float width = activeImage.rectTransform.rect.width;
        readyImage.rectTransform.anchoredPosition = new Vector2(width, 0); // 從右側滑入

        currentTween = DOTween.Sequence()
            .Append(activeImage.rectTransform.DOAnchorPos(new Vector2(-width, 0), duration).OnComplete(() =>
            {
                activeImage.gameObject.SetActive(false);
                activeImage.rectTransform.anchoredPosition = Vector2.zero; // 重置位置
            }))
            .Join(readyImage.rectTransform.DOAnchorPos(Vector2.zero, duration).OnComplete(SwapImages));
    }

    /// <summary>
    /// 縮放過渡 (DOTween 實現)
    /// </summary>
    private void ScaleTransition(float duration)
    {
        // 初始化 ReadyImage
        readyImage.rectTransform.localScale = Vector3.zero;
        // 只scale readyImage
        currentTween = readyImage.rectTransform.DOScale(Vector3.one, duration).OnComplete(SwapImages);
    }

    private void Scale2Transition(float duration)
    {
        // 初始化 ReadyImage
        readyImage.rectTransform.localScale = Vector3.zero;
        // scale readyImage & activeImage
        currentTween = DOTween.Sequence()
            .Append(activeImage.rectTransform.DOScale(Vector3.zero, duration).OnComplete(() =>
            {
                activeImage.gameObject.SetActive(false);
                activeImage.rectTransform.localScale = Vector3.one; // 恢復縮放
            }))
            .Join(readyImage.rectTransform.DOScale(Vector3.one, duration).OnComplete(SwapImages));
    }

    /// <summary>
    /// 從路徑加載圖片為 Sprite
    /// </summary>
    private Sprite LoadSpriteFromPath(string path)
    {
        if (!System.IO.File.Exists(path)) return null;

        byte[] imageData = System.IO.File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(imageData))
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        return null;
    }

    /// <summary>
    /// 交換 ActiveImage 和 ReadyImage 的角色
    /// </summary>
    private void SwapImages()
    {
        // 確保 activeImage 在最上層
        activeImage.transform.SetAsLastSibling();

        Image temp = activeImage;
        activeImage = readyImage;
        readyImage = temp;

        readyImage.gameObject.SetActive(false); // 隱藏 ReadyImage
    }

    /// <summary>
    /// 完成當前過渡效果
    /// </summary>
    public void CompleteCurrentTransition()
    {
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Complete();
            currentTween = null;
        }
    }
}
