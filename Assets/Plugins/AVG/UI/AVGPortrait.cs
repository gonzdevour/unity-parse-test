using System.Collections.Generic;
using UnityEngine;

public class AVGPortrait : MonoBehaviour
{
    public TransitionImage Portrait;
    private Dictionary<string, string> imagePaths = new Dictionary<string, string>(); // 儲存圖片資源路徑

    public void Init(List<Dictionary<string, string>> imageFileNamesDictList)
    {
        // 定義表情屬性鍵值列表
        string emoTypes = PPM.Inst.Get("表情類型列表"); // "無,喜,怒,樂,驚,疑,暈"
        string[] emos = emoTypes.Split(",");
        foreach (var item in imageFileNamesDictList)
        {
            // 組合每個表情的鍵值對
            foreach (string emo in emos)
            {
                if (item.ContainsKey(emo) && !string.IsNullOrEmpty(item[emo]))
                {
                    imagePaths[item["UID"] + emo] = item["AssetID"] + "-" + item[emo] + ".png";
                    // ex: imagePaths["高德君怒"] = A-anger
                }
            }
        }
    }
    public void GoTo(string key, string effectType = "fade", float duration = 0.5f)
    {
        if (!imagePaths.ContainsKey(key))
        {
            Debug.LogError($"Key '{key}' does not exist in imagePaths.");
            return;
        }
        var fileName = imagePaths[key];
        var assetRoot = PPM.Inst.Get("素材來源");
        var assetPath = PPM.Inst.Get("頭圖素材路徑");
        var imagePath = assetRoot + "://" + assetPath + fileName;

        Portrait.StartTransition(imagePath, effectType, duration);
    }

    public void Clear()
    {
        Portrait.Clear();
    }
}
