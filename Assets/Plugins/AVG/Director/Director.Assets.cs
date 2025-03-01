using UnityEngine;
using System.Collections.Generic;
using Story;

public partial class Director
{
    public void InitAssets()
    {
        imagePathsBackground = InitImagePaths(AVG.Inst.GetDictListFromDB<AssetData>("Bgs"));
        imagePathsSimbols = InitImagePaths(AVG.Inst.GetDictListFromDB<AssetData>("Simbols"));

        imagePathsPortrait = InitImagePathsPortrait(AVG.Inst.GetDictListFromDB<CharData>("Chars"));
    }

    public Dictionary<string, string> InitImagePaths(List<Dictionary<string, string>> pathsDictList)
    {
        Dictionary<string, string> idFilenameDict = new();
        foreach (var item in pathsDictList)
        {
            Debug.Log($"初始化：{item["AssetID"]} = {item["檔名"]}");
            idFilenameDict[item["AssetID"]] = item["檔名"];
        }
        return idFilenameDict;
    }

    public Dictionary<string, string> InitImagePathsPortrait(List<Dictionary<string, string>> charDataList)
    {
        Dictionary<string, string> idFilenameDict = new();
        // 定義表情屬性鍵值列表
        string emoTypes = PPM.Inst.Get("表情類型列表"); // "無,喜,怒,樂,驚,疑,暈"
        string[] emos = emoTypes.Split(",");
        foreach (var charData in charDataList)
        {
            // 組合每個表情的鍵值對
            foreach (string emo in emos)
            {
                if (charData.ContainsKey(emo) && !string.IsNullOrEmpty(charData[emo]))
                {
                    idFilenameDict[charData["UID"] + emo] = charData["AssetID"] + "-" + charData[emo] + ".png";
                    // ex: imagePathsPortrait["高德君怒"] = A-anger
                }
            }
        }
        return idFilenameDict;
    }

    public string GetSimbolImgUrl(string key)
    {
        if (!imagePathsSimbols.ContainsKey(key))
        {
            Debug.LogError($"Key '{key}' does not exist in imagePaths.");
            return string.Empty;
        }
        var fileName = imagePathsSimbols[key];
        var assetRoot = PPM.Inst.Get("素材來源");
        var assetPath = PPM.Inst.Get("符號素材路徑");
        var imagePath = assetRoot + "://" + assetPath + fileName;
        return imagePath;
    }

    public string GetBackgroundImgUrl(string key)
    {
        if (!imagePathsBackground.ContainsKey(key))
        {
            Debug.LogError($"Key '{key}' does not exist in imagePaths.");
            return string.Empty;
        }
        var fileName = imagePathsBackground[key];
        var assetRoot = PPM.Inst.Get("素材來源");
        var assetPath = PPM.Inst.Get("背景素材路徑");
        var imagePath = assetRoot + "://" + assetPath + fileName;
        return imagePath;
    }

    public string GetPortraitImgUrl(string key) //key = charUID + charEmo
    {
        string imagePath = DefaultPortraitImgUrl;
        if (imagePathsPortrait.ContainsKey(key))
        {
            var fileName = imagePathsPortrait[key];
            var assetRoot = PPM.Inst.Get("素材來源");
            var assetPath = PPM.Inst.Get("頭圖素材路徑");
            imagePath = assetRoot + "://" + assetPath + fileName;
        }
        return imagePath;
    }

    public Dictionary<string, string> GetPaths_CharExpressions(Dictionary<string, string> charData)
    {
        Dictionary<string, string> imagePathsExpression = new(); // 圖片路徑表

        var assetID = charData.GetValueOrDefault("AssetID", string.Empty);
        var assetRoot = PPM.Inst.Get("素材來源");
        var assetPath = PPM.Inst.Get("角色素材路徑");
        var resPath = assetRoot + "://" + assetPath;
        string emoTypes = PPM.Inst.Get("表情類型列表"); // "無,喜,怒,樂,驚,疑,暈"
        string[] emos = emoTypes.Split(",");
        // 組合每個表情的鍵值對
        foreach (string emo in emos)
        {
            string fileName = charData.GetValueOrDefault(emo, string.Empty);
            if (!string.IsNullOrEmpty(fileName))
            {
                // ex: imagePathsExpression["怒"] = StreamingAssets://Image/AVG/Char/A-anger.png
                imagePathsExpression[emo] = resPath + assetID + "-" + fileName + ".png";
            }
        }
        return imagePathsExpression;
    }
}
