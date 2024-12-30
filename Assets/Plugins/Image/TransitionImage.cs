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
    public float transitionDuration = 10.0f; // 過渡時間

    private Dictionary<string, Action> transitionEffects; // 儲存特效對應方法

    private void Awake()
    {
        // 初始化特效字典
        transitionEffects = new Dictionary<string, Action>
        {
            { "fade", FadeTransition },
            { "slide", SlideTransition },
            { "scale", ScaleTransition }
        };
    }

    private void Start()
    {
        StartCoroutine(Test());
    }

    private IEnumerator Test()
    {
        yield return new WaitForSeconds(2f);
        StartTransition("Assets/Resources/Sprites/AVG/BG/Landscape/Daily/ACstreet001_19201080.jpg", "fade");
    }

    /// <summary>
    /// 開始圖片轉換
    /// </summary>
    /// <param name="imagePath">新圖片的路徑</param>
    /// <param name="transitionType">過渡特效類型</param>
    public void StartTransition(string imagePath, string transitionType)
    {
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
            transitionEffects[transitionType.ToLower()]();
        }
        else
        {
            Debug.LogError($"無效的過渡特效類型: {transitionType}");
        }
    }

    /// <summary>
    /// 淡入淡出過渡 (DOTween 實現)
    /// </summary>
    private void FadeTransition()
    {
        readyImage.color = new Color(1, 1, 1, 0); // 初始化 ReadyImage
        activeImage.DOFade(0, transitionDuration).OnComplete(() =>
        {
            activeImage.gameObject.SetActive(false);
            activeImage.color = new Color(1, 1, 1, 1); // 恢復透明度
        });
        readyImage.DOFade(1, transitionDuration).OnComplete(SwapImages);
    }

    /// <summary>
    /// 滑動過渡 (DOTween 實現)
    /// </summary>
    private void SlideTransition()
    {
        float width = activeImage.rectTransform.rect.width;
        readyImage.rectTransform.anchoredPosition = new Vector2(width, 0); // 從右側滑入

        activeImage.rectTransform.DOAnchorPos(new Vector2(-width, 0), transitionDuration)
            .OnComplete(() => activeImage.gameObject.SetActive(false));
        readyImage.rectTransform.DOAnchorPos(Vector2.zero, transitionDuration).OnComplete(SwapImages);
    }

    /// <summary>
    /// 縮放過渡 (DOTween 實現)
    /// </summary>
    private void ScaleTransition()
    {
        readyImage.rectTransform.localScale = Vector3.zero; // 初始化 ReadyImage
        activeImage.rectTransform.DOScale(Vector3.zero, transitionDuration).OnComplete(() =>
        {
            activeImage.gameObject.SetActive(false);
            activeImage.rectTransform.localScale = Vector3.one; // 恢復縮放
        });
        readyImage.rectTransform.DOScale(Vector3.one, transitionDuration).OnComplete(SwapImages);
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
        Image temp = activeImage;
        activeImage = readyImage;
        readyImage = temp;

        readyImage.gameObject.SetActive(false); // 隱藏 ReadyImage
    }
}
