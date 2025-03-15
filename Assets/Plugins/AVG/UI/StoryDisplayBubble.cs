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

    public void AimX(float xDiff) //�ǤJcharRect�MbbRect��anchoredPosition.x���t��
    {
        Vector2 Pos = BubbleTailOriginPos;
        Pos.x += xDiff;
        BubbleTail.anchoredPosition = Pos;
    }
}
