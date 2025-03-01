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
            Debug.Log($"��l�ơG{item["AssetID"]} = {item["�ɦW"]}");
            idFilenameDict[item["AssetID"]] = item["�ɦW"];
        }
        return idFilenameDict;
    }

    public Dictionary<string, string> InitImagePathsPortrait(List<Dictionary<string, string>> charDataList)
    {
        Dictionary<string, string> idFilenameDict = new();
        // �w�q���ݩ���ȦC��
        string emoTypes = PPM.Inst.Get("�������C��"); // "�L,��,��,��,��,��,�w"
        string[] emos = emoTypes.Split(",");
        foreach (var charData in charDataList)
        {
            // �զX�C�Ӫ�����ȹ�
            foreach (string emo in emos)
            {
                if (charData.ContainsKey(emo) && !string.IsNullOrEmpty(charData[emo]))
                {
                    idFilenameDict[charData["UID"] + emo] = charData["AssetID"] + "-" + charData[emo] + ".png";
                    // ex: imagePathsPortrait["���w�g��"] = A-anger
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
        var assetRoot = PPM.Inst.Get("�����ӷ�");
        var assetPath = PPM.Inst.Get("�Ÿ��������|");
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
        var assetRoot = PPM.Inst.Get("�����ӷ�");
        var assetPath = PPM.Inst.Get("�I���������|");
        var imagePath = assetRoot + "://" + assetPath + fileName;
        return imagePath;
    }

    public string GetPortraitImgUrl(string key) //key = charUID + charEmo
    {
        string imagePath = DefaultPortraitImgUrl;
        if (imagePathsPortrait.ContainsKey(key))
        {
            var fileName = imagePathsPortrait[key];
            var assetRoot = PPM.Inst.Get("�����ӷ�");
            var assetPath = PPM.Inst.Get("�Y�ϯ������|");
            imagePath = assetRoot + "://" + assetPath + fileName;
        }
        return imagePath;
    }

    public Dictionary<string, string> GetPaths_CharExpressions(Dictionary<string, string> charData)
    {
        Dictionary<string, string> imagePathsExpression = new(); // �Ϥ����|��

        var assetID = charData.GetValueOrDefault("AssetID", string.Empty);
        var assetRoot = PPM.Inst.Get("�����ӷ�");
        var assetPath = PPM.Inst.Get("����������|");
        var resPath = assetRoot + "://" + assetPath;
        string emoTypes = PPM.Inst.Get("�������C��"); // "�L,��,��,��,��,��,�w"
        string[] emos = emoTypes.Split(",");
        // �զX�C�Ӫ�����ȹ�
        foreach (string emo in emos)
        {
            string fileName = charData.GetValueOrDefault(emo, string.Empty);
            if (!string.IsNullOrEmpty(fileName))
            {
                // ex: imagePathsExpression["��"] = StreamingAssets://Image/AVG/Char/A-anger.png
                imagePathsExpression[emo] = resPath + assetID + "-" + fileName + ".png";
            }
        }
        return imagePathsExpression;
    }
}
