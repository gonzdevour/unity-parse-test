using System.Collections.Generic;
using UnityEngine;

public class AVGPortrait : MonoBehaviour
{
    public TransitionImage Portrait;
    private string LastPortraitKey = string.Empty;

    public void GoTo(string key, string effectType = "fade", float duration = 0.5f)
    {
        if (key != LastPortraitKey) //不同頭圖時才執行轉換
        {
            string imagePath = Director.Inst.GetPortraitImgUrl(key);
            Portrait.StartTransition(imagePath, effectType, duration);
        }
        LastPortraitKey = key;
    }

    public void Clear()
    {
        Portrait.Clear();
    }
}
