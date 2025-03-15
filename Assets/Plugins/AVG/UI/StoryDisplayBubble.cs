using System.Runtime.InteropServices;
using UnityEngine;

public class StoryDisplayBubble : StoryDisplay
{
    public RectTransform BubbleBody;
    public RectTransform BubbleTail;

    private Vector2 BubbleTailOriginPos;

    private void Awake()
    {
        BubbleTailOriginPos = BubbleTail.anchoredPosition;
    }

    public void AimX(float xDiff) //傳入charRect和bbRect的anchoredPosition.x的差值
    {
        Vector2 Pos = BubbleTailOriginPos;
        Pos.x += xDiff;
        BubbleTail.anchoredPosition = Pos;
    }
}
