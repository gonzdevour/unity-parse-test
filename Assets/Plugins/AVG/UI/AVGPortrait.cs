using System.Collections.Generic;
using UnityEngine;

public class AVGPortrait : MonoBehaviour
{
    public TransitionImage Portrait;

    public void GoTo(string key, string effectType = "fade", float duration = 0.5f)
    {
        var imgPathsPortrait = Director.Inst.imagePathsPortrait;
        string imagePath = Director.Inst.DefaultPortraitImgUrl;
        if (imgPathsPortrait.ContainsKey(key))
        {
            var fileName = imgPathsPortrait[key];
            var assetRoot = PPM.Inst.Get("素材來源");
            var assetPath = PPM.Inst.Get("頭圖素材路徑");
            imagePath = assetRoot + "://" + assetPath + fileName;
        }
        else
        {
            Debug.LogError($"Key '{key}' does not exist in imagePaths.");
        }
        Portrait.StartTransition(imagePath, effectType, duration);
    }

    public void Clear()
    {
        Portrait.Clear();
    }
}
