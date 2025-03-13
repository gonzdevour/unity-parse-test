using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Collections;
using static System.TimeZoneInfo;

public class TransitionImage : MonoBehaviour
{
    public bool SetNativeSize = true;
    public Image activeImage; // 當前顯示的圖片
    public Image readyImage;  // 下一步將顯示的圖片

    private Dictionary<string, Action<float, Ease, Ease>> transitionEffects; // 儲存特效對應方法
    private Tween currentTween; // 當前執行的過渡動畫

    private void Awake()
    {
        // 初始化特效字典
        transitionEffects = new Dictionary<string, Action<float,Ease,Ease>>
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

    public void Clear()
    {
        ClearImage(activeImage);
        ClearImage(readyImage);
    }

    private void ClearImage(Image image)
    {
        if (image != null)
        {
            Sprite transparentSprite = Resources.Load<Sprite>("Sprites/transparent32x32");
            if (transparentSprite != null)
            {
                image.sprite = transparentSprite;
            }
            else
            {
                image.sprite = null;
                Debug.Log("透明素材不存在，改將sprite設定為空");
            }
        }
    }

    public void ResetAlpha()
    {
        // 強制ReadyImage的透明度為1，保留當前 RGB
        var readyImageColor = readyImage.color;
        readyImage.color = new Color(readyImageColor.r, readyImageColor.g, readyImageColor.b, 1);
        // 強制ActiveImage的透明度為1，保留當前 RGB
        var activeImageColor = activeImage.color;
        activeImage.color = new Color(readyImageColor.r, readyImageColor.g, readyImageColor.b, 1);
    }

    /// <summary>
    /// 開始圖片轉換
    /// </summary>
    /// <param name="imgUrl">新圖片的路徑</param>
    /// <param name="transitionType">過渡特效類型</param>
    /// <param name="dur">過渡特效時間</param>
    /// <param name="onComplete">過渡完成時的回調</param>
    public void StartTransition(string imgUrl, string transitionType = "fade", float dur = 1f, Ease easeOut = Ease.Linear, Ease easeIn = Ease.Linear, Action onComplete = null)
    {
        //Debug.Log($"[TImg開始轉換]類型:{transitionType}, dur:{dur}f, img:{imgUrl}");
        // 若有正在執行的過渡效果，先完成它
        CompleteCurrentTransition();

        // 更換 ReadyImage 圖片
        SpriteCacher.Inst.GetSprite(imgUrl, (sprite) =>
        {
            if (readyImage != null) //避免回傳時物件已刪除
            {
                readyImage.sprite = sprite;
                // 設定為原始大小
                if (SetNativeSize) readyImage.SetNativeSize();
                // 顯示 ReadyImage
                readyImage.gameObject.SetActive(true);
                // 檢查並執行特效
                if (transitionEffects == null)
                {
                    Debug.LogError("Transition effects dictionary is null.");
                    return;
                }
                if (transitionEffects.ContainsKey(transitionType.ToLower()))
                {
                    var effect = transitionEffects[transitionType.ToLower()];
                    if (effect != null)
                    {
                        effect(dur, easeOut, easeIn); // 執行特效
                    }
                    else
                    {
                        Debug.LogError($"特效 '{transitionType}' 未定義行為。");
                    }
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
            };
        });
    }

    /// <summary>
    /// 淡入淡出過渡 (DOTween 實現)
    /// </summary>
    private void FadeTransition(float duration, Ease easeOut = Ease.Linear, Ease easeIn = Ease.Linear)
    {
        // 強制ReadyImage的透明度為0，保留當前 RGB
        var readyImageColor = readyImage.color;
        readyImage.color = new Color(readyImageColor.r, readyImageColor.g, readyImageColor.b, 0);

        currentTween = DOTween.Sequence()
            // 淡出 activeImage
            .Join(activeImage.DOFade(0, duration).SetEase(easeOut))
            // 淡入 readyImage
            .Join(readyImage.DOFade(1, duration).SetEase(easeIn))
            // 等淡入、淡出完成後再執行 SwapImages
            .OnComplete(() =>
            {
                activeImage.gameObject.SetActive(false);
                SwapImages();
            });
    }

    /// <summary>
    /// 從左到右滑動過渡 (DOTween 實現)
    /// </summary>
    private void SlideRightTransition(float duration, Ease easeOut = Ease.Linear, Ease easeIn = Ease.Linear)
    {
        float width = activeImage.rectTransform.rect.width;
        readyImage.rectTransform.anchoredPosition = new Vector2(-width, 0); // 從左側滑入

        currentTween = DOTween.Sequence()
            .Append(activeImage.rectTransform.DOAnchorPos(new Vector2(width, 0), duration).SetEase(easeOut).OnComplete(() =>
            {
                activeImage.gameObject.SetActive(false);
                activeImage.rectTransform.anchoredPosition = Vector2.zero; // 重置位置
            }))
            .Join(readyImage.rectTransform.DOAnchorPos(Vector2.zero, duration).SetEase(easeIn))
            .OnComplete(SwapImages);
    }

    /// <summary>
    /// 從上到下滑動過渡 (DOTween 實現)
    /// </summary>
    private void SlideDownTransition(float duration, Ease easeOut = Ease.Linear, Ease easeIn = Ease.Linear)
    {
        float height = activeImage.rectTransform.rect.height;
        readyImage.rectTransform.anchoredPosition = new Vector2(0, height); // 從上方滑入

        currentTween = DOTween.Sequence()
            .Append(activeImage.rectTransform.DOAnchorPos(new Vector2(0, -height), duration).SetEase(easeOut).OnComplete(() =>
            {
                activeImage.gameObject.SetActive(false);
                activeImage.rectTransform.anchoredPosition = Vector2.zero; // 重置位置
            }))
            .Join(readyImage.rectTransform.DOAnchorPos(Vector2.zero, duration).SetEase(easeIn).OnComplete(SwapImages));
    }

    /// <summary>
    /// 從下到上滑動過渡 (DOTween 實現)
    /// </summary>
    private void SlideUpTransition(float duration, Ease easeOut = Ease.Linear, Ease easeIn = Ease.Linear)
    {
        float height = activeImage.rectTransform.rect.height;
        readyImage.rectTransform.anchoredPosition = new Vector2(0, -height); // 從下方滑入

        currentTween = DOTween.Sequence()
            .Append(activeImage.rectTransform.DOAnchorPos(new Vector2(0, height), duration).SetEase(easeOut).OnComplete(() =>
            {
                activeImage.gameObject.SetActive(false);
                activeImage.rectTransform.anchoredPosition = Vector2.zero; // 重置位置
            }))
            .Join(readyImage.rectTransform.DOAnchorPos(Vector2.zero, duration).SetEase(easeIn).OnComplete(SwapImages));
    }

    /// <summary>
    /// 滑動過渡 (DOTween 實現)
    /// </summary>
    private void SlideTransition(float duration, Ease easeOut = Ease.Linear, Ease easeIn = Ease.Linear)
    {
        float width = activeImage.rectTransform.rect.width;
        readyImage.rectTransform.anchoredPosition = new Vector2(width, 0); // 從右側滑入

        currentTween = DOTween.Sequence()
            .Append(activeImage.rectTransform.DOAnchorPos(new Vector2(-width, 0), duration).SetEase(easeOut).OnComplete(() =>
            {
                activeImage.gameObject.SetActive(false);
                activeImage.rectTransform.anchoredPosition = Vector2.zero; // 重置位置
            }))
            .Join(readyImage.rectTransform.DOAnchorPos(Vector2.zero, duration).SetEase(easeIn).OnComplete(SwapImages));
    }

    /// <summary>
    /// 縮放過渡 (DOTween 實現)
    /// </summary>
    private void ScaleTransition(float duration, Ease easeOut = Ease.Linear, Ease easeIn = Ease.Linear)
    {
        // 初始化 ReadyImage
        readyImage.rectTransform.localScale = Vector3.zero;
        // 只scale readyImage
        currentTween = readyImage.rectTransform.DOScale(Vector3.one, duration).SetEase(easeOut).OnComplete(SwapImages);
    }

    private void Scale2Transition(float duration, Ease easeOut = Ease.Linear, Ease easeIn = Ease.Linear)
    {
        // 初始化 ReadyImage
        readyImage.rectTransform.localScale = Vector3.zero;
        // scale readyImage & activeImage
        currentTween = DOTween.Sequence()
            .Append(activeImage.rectTransform.DOScale(Vector3.zero, duration).SetEase(easeOut).OnComplete(() =>
            {
                activeImage.gameObject.SetActive(false);
                activeImage.rectTransform.localScale = Vector3.one; // 恢復縮放
            }))
            .Join(readyImage.rectTransform.DOScale(Vector3.one, duration).SetEase(easeIn).OnComplete(SwapImages));
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
            Debug.Log("完成並刪除目前Tween");
        }
    }
}
