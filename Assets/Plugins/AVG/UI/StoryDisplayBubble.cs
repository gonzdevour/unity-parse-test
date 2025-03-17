using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryDisplayBubble : StoryDisplay
{
    public RectTransform BubbleBg;
    public RectTransform BubbleTail;

    private Vector2 BubbleTailOriginPos;
    private Image BubbleBgImg;
    private Image BubbleTailImg;
    private Dictionary<string, string> tonePaths = new Dictionary<string, string>()
    {
        { "���q", "Sprites/UI/Bubble" },
        { "���", "Sprites/UI/Bubble_Cloud" },
        { "�z��", "Sprites/UI/Bubble_Spike" },
        { "���q��", "Sprites/UI/BubbleTail" },
        { "��ҧ�", "Sprites/UI/BubbleTail_Cloud" }
    };

    private void Awake()
    {
        BubbleTailOriginPos = BubbleTail.anchoredPosition;
        BubbleBgImg = BubbleBg.GetComponent<Image>();
        BubbleTailImg = BubbleTail.GetComponent<Image>();
    }

    public void AimX(float xDiff) //�ǤJcharRect�MbbRect��anchoredPosition.x���t��
    {
        Vector2 Pos = BubbleTailOriginPos;
        Pos.x += xDiff;
        BubbleTail.anchoredPosition = Pos;
    }

    public void SetTone(string tone)
    {
        switch (tone)
        {
            case "���":
                BubbleBgImg.sprite = GetSprite("���");
                BubbleTailImg.sprite = GetSprite("��ҧ�");
                break;
            case "�z��":
                BubbleBgImg.sprite = GetSprite("�z��");
                BubbleTailImg.sprite = GetSprite("���q��");
                break;
            default:
                BubbleBgImg.sprite = GetSprite("���q");
                BubbleTailImg.sprite = GetSprite("���q��");
                break;
        }
    }

    // ���o������ Sprite
    public Sprite GetSprite(string tone)
    {
        if (tonePaths.TryGetValue(tone, out string path))
        {
            return Resources.Load<Sprite>(path);
        }
        else
        {
            Debug.LogWarning($"�䤣�������tone�Ϲ�: {tone}");
            return null;
        }
    }
}
