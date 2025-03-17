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
        { "普通", "Sprites/UI/Bubble" },
        { "思考", "Sprites/UI/Bubble_Cloud" },
        { "爆炸", "Sprites/UI/Bubble_Spike" },
        { "普通尾", "Sprites/UI/BubbleTail" },
        { "思考尾", "Sprites/UI/BubbleTail_Cloud" }
    };

    private void Awake()
    {
        BubbleTailOriginPos = BubbleTail.anchoredPosition;
        BubbleBgImg = BubbleBg.GetComponent<Image>();
        BubbleTailImg = BubbleTail.GetComponent<Image>();
    }

    public void AimX(float xDiff) //傳入charRect和bbRect的anchoredPosition.x的差值
    {
        Vector2 Pos = BubbleTailOriginPos;
        Pos.x += xDiff;
        BubbleTail.anchoredPosition = Pos;
    }

    public void SetTone(string tone)
    {
        switch (tone)
        {
            case "思考":
                BubbleBgImg.sprite = GetSprite("思考");
                BubbleTailImg.sprite = GetSprite("思考尾");
                break;
            case "爆炸":
                BubbleBgImg.sprite = GetSprite("爆炸");
                BubbleTailImg.sprite = GetSprite("普通尾");
                break;
            default:
                BubbleBgImg.sprite = GetSprite("普通");
                BubbleTailImg.sprite = GetSprite("普通尾");
                break;
        }
    }

    // 取得對應的 Sprite
    public Sprite GetSprite(string tone)
    {
        if (tonePaths.TryGetValue(tone, out string path))
        {
            return Resources.Load<Sprite>(path);
        }
        else
        {
            Debug.LogWarning($"找不到對應的tone圖像: {tone}");
            return null;
        }
    }
}
