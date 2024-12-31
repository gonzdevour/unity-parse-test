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

    private Dictionary<string, Action<float>> transitionEffects; // �x�s�S�Ĺ�����k
    private Tween currentTween; // ��e���檺�L��ʵe

    private void Awake()
    {
        // ��l�ƯS�Ħr��
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
    /// �}�l�Ϥ��ഫ
    /// </summary>
    /// <param name="imagePath">�s�Ϥ������|</param>
    /// <param name="transitionType">�L��S������</param>
    /// <param name="dur">�L��S�Įɶ�</param>
    /// <param name="onComplete">�L�秹���ɪ��^��</param>
    public void StartTransition(string imagePath, string transitionType, float dur = 2f, Action onComplete = null)
    {
        // �Y�����b���檺�L��ĪG�A��������
        CompleteCurrentTransition();

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
            transitionEffects[transitionType.ToLower()](dur);
        }
        else
        {
            Debug.LogError($"�L�Ī��L��S������: {transitionType}");
        }

        // �]�m�����ɪ��^��
        if (onComplete != null)
        {
            currentTween?.OnComplete(() => onComplete());
        }
    }

    /// <summary>
    /// �H�J�H�X�L�� (DOTween ��{)
    /// </summary>
    private void FadeTransition(float duration)
    {
        readyImage.color = new Color(1, 1, 1, 0); // ��l�� ReadyImage
        currentTween = DOTween.Sequence()
            .Append(activeImage.DOFade(0, duration).OnComplete(() =>
            {
                activeImage.gameObject.SetActive(false);
                activeImage.color = new Color(1, 1, 1, 1); // ��_�z����
            }))
            .Join(readyImage.DOFade(1, duration).OnComplete(SwapImages));
    }

    /// <summary>
    /// �q����k�ưʹL�� (DOTween ��{)
    /// </summary>
    private void SlideRightTransition(float duration)
    {
        float width = activeImage.rectTransform.rect.width;
        readyImage.rectTransform.anchoredPosition = new Vector2(-width, 0); // �q�����ƤJ

        currentTween = DOTween.Sequence()
            .Append(activeImage.rectTransform.DOAnchorPos(new Vector2(width, 0), duration).OnComplete(() =>
            {
                activeImage.gameObject.SetActive(false);
                activeImage.rectTransform.anchoredPosition = Vector2.zero; // ���m��m
            }))
            .Join(readyImage.rectTransform.DOAnchorPos(Vector2.zero, duration).OnComplete(SwapImages));
    }

    /// <summary>
    /// �q�W��U�ưʹL�� (DOTween ��{)
    /// </summary>
    private void SlideDownTransition(float duration)
    {
        float height = activeImage.rectTransform.rect.height;
        readyImage.rectTransform.anchoredPosition = new Vector2(0, height); // �q�W��ƤJ

        currentTween = DOTween.Sequence()
            .Append(activeImage.rectTransform.DOAnchorPos(new Vector2(0, -height), duration).OnComplete(() =>
            {
                activeImage.gameObject.SetActive(false);
                activeImage.rectTransform.anchoredPosition = Vector2.zero; // ���m��m
            }))
            .Join(readyImage.rectTransform.DOAnchorPos(Vector2.zero, duration).OnComplete(SwapImages));
    }

    /// <summary>
    /// �q�U��W�ưʹL�� (DOTween ��{)
    /// </summary>
    private void SlideUpTransition(float duration)
    {
        float height = activeImage.rectTransform.rect.height;
        readyImage.rectTransform.anchoredPosition = new Vector2(0, -height); // �q�U��ƤJ

        currentTween = DOTween.Sequence()
            .Append(activeImage.rectTransform.DOAnchorPos(new Vector2(0, height), duration).OnComplete(() =>
            {
                activeImage.gameObject.SetActive(false);
                activeImage.rectTransform.anchoredPosition = Vector2.zero; // ���m��m
            }))
            .Join(readyImage.rectTransform.DOAnchorPos(Vector2.zero, duration).OnComplete(SwapImages));
    }

    /// <summary>
    /// �ưʹL�� (DOTween ��{)
    /// </summary>
    private void SlideTransition(float duration)
    {
        float width = activeImage.rectTransform.rect.width;
        readyImage.rectTransform.anchoredPosition = new Vector2(width, 0); // �q�k���ƤJ

        currentTween = DOTween.Sequence()
            .Append(activeImage.rectTransform.DOAnchorPos(new Vector2(-width, 0), duration).OnComplete(() =>
            {
                activeImage.gameObject.SetActive(false);
                activeImage.rectTransform.anchoredPosition = Vector2.zero; // ���m��m
            }))
            .Join(readyImage.rectTransform.DOAnchorPos(Vector2.zero, duration).OnComplete(SwapImages));
    }

    /// <summary>
    /// �Y��L�� (DOTween ��{)
    /// </summary>
    private void ScaleTransition(float duration)
    {
        // ��l�� ReadyImage
        readyImage.rectTransform.localScale = Vector3.zero;
        // �uscale readyImage
        currentTween = readyImage.rectTransform.DOScale(Vector3.one, duration).OnComplete(SwapImages);
    }

    private void Scale2Transition(float duration)
    {
        // ��l�� ReadyImage
        readyImage.rectTransform.localScale = Vector3.zero;
        // scale readyImage & activeImage
        currentTween = DOTween.Sequence()
            .Append(activeImage.rectTransform.DOScale(Vector3.zero, duration).OnComplete(() =>
            {
                activeImage.gameObject.SetActive(false);
                activeImage.rectTransform.localScale = Vector3.one; // ��_�Y��
            }))
            .Join(readyImage.rectTransform.DOScale(Vector3.one, duration).OnComplete(SwapImages));
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
        // �T�O activeImage �b�̤W�h
        activeImage.transform.SetAsLastSibling();

        Image temp = activeImage;
        activeImage = readyImage;
        readyImage = temp;

        readyImage.gameObject.SetActive(false); // ���� ReadyImage
    }

    /// <summary>
    /// ������e�L��ĪG
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
