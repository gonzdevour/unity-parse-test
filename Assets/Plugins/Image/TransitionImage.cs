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
    public Image activeImage; // ��e��ܪ��Ϥ�
    public Image readyImage;  // �U�@�B�N��ܪ��Ϥ�

    private Dictionary<string, Action<float, Ease, Ease>> transitionEffects; // �x�s�S�Ĺ�����k
    private Tween currentTween; // ��e���檺�L��ʵe

    private void Awake()
    {
        // ��l�ƯS�Ħr��
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
                Debug.Log("�z���������s�b�A��Nsprite�]�w����");
            }
        }
    }

    public void ResetAlpha()
    {
        // �j��ReadyImage���z���׬�1�A�O�d��e RGB
        var readyImageColor = readyImage.color;
        readyImage.color = new Color(readyImageColor.r, readyImageColor.g, readyImageColor.b, 1);
        // �j��ActiveImage���z���׬�1�A�O�d��e RGB
        var activeImageColor = activeImage.color;
        activeImage.color = new Color(readyImageColor.r, readyImageColor.g, readyImageColor.b, 1);
    }

    /// <summary>
    /// �}�l�Ϥ��ഫ
    /// </summary>
    /// <param name="imgUrl">�s�Ϥ������|</param>
    /// <param name="transitionType">�L��S������</param>
    /// <param name="dur">�L��S�Įɶ�</param>
    /// <param name="onComplete">�L�秹���ɪ��^��</param>
    public void StartTransition(string imgUrl, string transitionType = "fade", float dur = 1f, Ease easeOut = Ease.Linear, Ease easeIn = Ease.Linear, Action onComplete = null)
    {
        //Debug.Log($"[TImg�}�l�ഫ]����:{transitionType}, dur:{dur}f, img:{imgUrl}");
        // �Y�����b���檺�L��ĪG�A��������
        CompleteCurrentTransition();

        // �� ReadyImage �Ϥ�
        SpriteCacher.Inst.GetSprite(imgUrl, (sprite) =>
        {
            if (readyImage != null) //�קK�^�Ǯɪ���w�R��
            {
                readyImage.sprite = sprite;
                // �]�w����l�j�p
                if (SetNativeSize) readyImage.SetNativeSize();
                // ��� ReadyImage
                readyImage.gameObject.SetActive(true);
                // �ˬd�ð���S��
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
                        effect(dur, easeOut, easeIn); // ����S��
                    }
                    else
                    {
                        Debug.LogError($"�S�� '{transitionType}' ���w�q�欰�C");
                    }
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
            };
        });
    }

    /// <summary>
    /// �H�J�H�X�L�� (DOTween ��{)
    /// </summary>
    private void FadeTransition(float duration, Ease easeOut = Ease.Linear, Ease easeIn = Ease.Linear)
    {
        // �j��ReadyImage���z���׬�0�A�O�d��e RGB
        var readyImageColor = readyImage.color;
        readyImage.color = new Color(readyImageColor.r, readyImageColor.g, readyImageColor.b, 0);

        currentTween = DOTween.Sequence()
            // �H�X activeImage
            .Join(activeImage.DOFade(0, duration).SetEase(easeOut))
            // �H�J readyImage
            .Join(readyImage.DOFade(1, duration).SetEase(easeIn))
            // ���H�J�B�H�X������A���� SwapImages
            .OnComplete(() =>
            {
                activeImage.gameObject.SetActive(false);
                SwapImages();
            });
    }

    /// <summary>
    /// �q����k�ưʹL�� (DOTween ��{)
    /// </summary>
    private void SlideRightTransition(float duration, Ease easeOut = Ease.Linear, Ease easeIn = Ease.Linear)
    {
        float width = activeImage.rectTransform.rect.width;
        readyImage.rectTransform.anchoredPosition = new Vector2(-width, 0); // �q�����ƤJ

        currentTween = DOTween.Sequence()
            .Append(activeImage.rectTransform.DOAnchorPos(new Vector2(width, 0), duration).SetEase(easeOut).OnComplete(() =>
            {
                activeImage.gameObject.SetActive(false);
                activeImage.rectTransform.anchoredPosition = Vector2.zero; // ���m��m
            }))
            .Join(readyImage.rectTransform.DOAnchorPos(Vector2.zero, duration).SetEase(easeIn))
            .OnComplete(SwapImages);
    }

    /// <summary>
    /// �q�W��U�ưʹL�� (DOTween ��{)
    /// </summary>
    private void SlideDownTransition(float duration, Ease easeOut = Ease.Linear, Ease easeIn = Ease.Linear)
    {
        float height = activeImage.rectTransform.rect.height;
        readyImage.rectTransform.anchoredPosition = new Vector2(0, height); // �q�W��ƤJ

        currentTween = DOTween.Sequence()
            .Append(activeImage.rectTransform.DOAnchorPos(new Vector2(0, -height), duration).SetEase(easeOut).OnComplete(() =>
            {
                activeImage.gameObject.SetActive(false);
                activeImage.rectTransform.anchoredPosition = Vector2.zero; // ���m��m
            }))
            .Join(readyImage.rectTransform.DOAnchorPos(Vector2.zero, duration).SetEase(easeIn).OnComplete(SwapImages));
    }

    /// <summary>
    /// �q�U��W�ưʹL�� (DOTween ��{)
    /// </summary>
    private void SlideUpTransition(float duration, Ease easeOut = Ease.Linear, Ease easeIn = Ease.Linear)
    {
        float height = activeImage.rectTransform.rect.height;
        readyImage.rectTransform.anchoredPosition = new Vector2(0, -height); // �q�U��ƤJ

        currentTween = DOTween.Sequence()
            .Append(activeImage.rectTransform.DOAnchorPos(new Vector2(0, height), duration).SetEase(easeOut).OnComplete(() =>
            {
                activeImage.gameObject.SetActive(false);
                activeImage.rectTransform.anchoredPosition = Vector2.zero; // ���m��m
            }))
            .Join(readyImage.rectTransform.DOAnchorPos(Vector2.zero, duration).SetEase(easeIn).OnComplete(SwapImages));
    }

    /// <summary>
    /// �ưʹL�� (DOTween ��{)
    /// </summary>
    private void SlideTransition(float duration, Ease easeOut = Ease.Linear, Ease easeIn = Ease.Linear)
    {
        float width = activeImage.rectTransform.rect.width;
        readyImage.rectTransform.anchoredPosition = new Vector2(width, 0); // �q�k���ƤJ

        currentTween = DOTween.Sequence()
            .Append(activeImage.rectTransform.DOAnchorPos(new Vector2(-width, 0), duration).SetEase(easeOut).OnComplete(() =>
            {
                activeImage.gameObject.SetActive(false);
                activeImage.rectTransform.anchoredPosition = Vector2.zero; // ���m��m
            }))
            .Join(readyImage.rectTransform.DOAnchorPos(Vector2.zero, duration).SetEase(easeIn).OnComplete(SwapImages));
    }

    /// <summary>
    /// �Y��L�� (DOTween ��{)
    /// </summary>
    private void ScaleTransition(float duration, Ease easeOut = Ease.Linear, Ease easeIn = Ease.Linear)
    {
        // ��l�� ReadyImage
        readyImage.rectTransform.localScale = Vector3.zero;
        // �uscale readyImage
        currentTween = readyImage.rectTransform.DOScale(Vector3.one, duration).SetEase(easeOut).OnComplete(SwapImages);
    }

    private void Scale2Transition(float duration, Ease easeOut = Ease.Linear, Ease easeIn = Ease.Linear)
    {
        // ��l�� ReadyImage
        readyImage.rectTransform.localScale = Vector3.zero;
        // scale readyImage & activeImage
        currentTween = DOTween.Sequence()
            .Append(activeImage.rectTransform.DOScale(Vector3.zero, duration).SetEase(easeOut).OnComplete(() =>
            {
                activeImage.gameObject.SetActive(false);
                activeImage.rectTransform.localScale = Vector3.one; // ��_�Y��
            }))
            .Join(readyImage.rectTransform.DOScale(Vector3.one, duration).SetEase(easeIn).OnComplete(SwapImages));
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
            Debug.Log("�����çR���ثeTween");
        }
    }
}
