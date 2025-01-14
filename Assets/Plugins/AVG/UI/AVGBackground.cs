using System.Collections.Generic;
using UnityEngine;

public class AVGBackground : MonoBehaviour
{
    public TransitionImage Background;
    private Dictionary<string, string> imagePaths = new Dictionary<string, string>(); // 儲存圖片資源路徑

    public void Init(List<Dictionary<string, string>> imageFileNamesDictList)
    {
        foreach (var item in imageFileNamesDictList)
        {
            imagePaths[item["UID"]] = item["圖檔名稱"];
            Debug.Log($"初始化：{item["UID"]}={item["圖檔名稱"]}" );
        }
    }

    public void GoTo(string key, string effectType = "fade", float duration = 2f)
    {
        if (!imagePaths.ContainsKey(key))
        {
            Debug.LogError($"Key '{key}' does not exist in imagePaths.");
            return;
        }
        var fileName = imagePaths[key];
        var assetRoot = PPM.Inst.Get("素材來源");
        var assetPath = PPM.Inst.Get("背景素材路徑");
        var imagePath = assetRoot + "://" + assetPath + fileName;

        Background.StartTransition(imagePath, effectType, duration);
    }

    public void Clear()
    {
        Background.Clear();
    }
}
