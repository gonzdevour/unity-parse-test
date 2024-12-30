using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Collections;

public class TransitionImage : MonoBehaviour
{
    public Image activeImage; // ��e��ܪ��Ϥ�
    public Image readyImage;  // �U�@�B�N��ܪ��Ϥ�
    public float transitionDuration = 10.0f; // �L��ɶ�

    private Dictionary<string, Action> transitionEffects; // �x�s�S�Ĺ�����k

    private void Awake()
    {
        // ��l�ƯS�Ħr��
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
    /// �}�l�Ϥ��ഫ
    /// </summary>
    /// <param name="imagePath">�s�Ϥ������|</param>
    /// <param name="transitionType">�L��S������</param>
    public void StartTransition(string imagePath, string transitionType)
    {
        // �� ReadyImage �Ϥ�
        Sprite newSprite = LoadSpriteFromPath(imagePath);
        if (newSprite == null)
        {
            Debug.LogError($"�Ϥ����|�L��: {imagePath}");
            return;
        }
        readyImage.sprite = newSprite;
        readyImage.gameObject.SetActive(true); // ��� ReadyImage

        // �ˬd�ð���S��
        if (transitionEffects.ContainsKey(transitionType.ToLower()))
        {
            transitionEffects[transitionType.ToLower()]();
        }
        else
        {
            Debug.LogError($"�L�Ī��L��S������: {transitionType}");
        }
    }

    /// <summary>
    /// �H�J�H�X�L�� (DOTween ��{)
    /// </summary>
    private void FadeTransition()
    {
        readyImage.color = new Color(1, 1, 1, 0); // ��l�� ReadyImage
        activeImage.DOFade(0, transitionDuration).OnComplete(() =>
        {
            activeImage.gameObject.SetActive(false);
            activeImage.color = new Color(1, 1, 1, 1); // ��_�z����
        });
        readyImage.DOFade(1, transitionDuration).OnComplete(SwapImages);
    }

    /// <summary>
    /// �ưʹL�� (DOTween ��{)
    /// </summary>
    private void SlideTransition()
    {
        float width = activeImage.rectTransform.rect.width;
        readyImage.rectTransform.anchoredPosition = new Vector2(width, 0); // �q�k���ƤJ

        activeImage.rectTransform.DOAnchorPos(new Vector2(-width, 0), transitionDuration)
            .OnComplete(() => activeImage.gameObject.SetActive(false));
        readyImage.rectTransform.DOAnchorPos(Vector2.zero, transitionDuration).OnComplete(SwapImages);
    }

    /// <summary>
    /// �Y��L�� (DOTween ��{)
    /// </summary>
    private void ScaleTransition()
    {
        readyImage.rectTransform.localScale = Vector3.zero; // ��l�� ReadyImage
        activeImage.rectTransform.DOScale(Vector3.zero, transitionDuration).OnComplete(() =>
        {
            activeImage.gameObject.SetActive(false);
            activeImage.rectTransform.localScale = Vector3.one; // ��_�Y��
        });
        readyImage.rectTransform.DOScale(Vector3.one, transitionDuration).OnComplete(SwapImages);
    }

    /// <summary>
    /// �q���|�[���Ϥ��� Sprite
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
    /// �洫 ActiveImage �M ReadyImage ������
    /// </summary>
    private void SwapImages()
    {
        Image temp = activeImage;
        activeImage = readyImage;
        readyImage = temp;

        readyImage.gameObject.SetActive(false); // ���� ReadyImage
    }
}
