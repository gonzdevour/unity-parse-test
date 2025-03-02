using UnityEngine;
using System.Collections.Generic;
using Story;

public partial class Director
{
    public void InitAssets()
    {
        string resourceFrom = PPM.Inst.Get("�����ӷ�");

        string backgroundFrom = PPM.Inst.Get("�I���������|");
        List<Dictionary<string, string>> dictListBackgrouds = AVG.Inst.GetDictListFromDB<AssetData>("Bgs");
        imagePathsBackground = RebuildFilePaths(dictListBackgrouds, resourceFrom, backgroundFrom);

        string simbolFrom = PPM.Inst.Get("�Ÿ��������|");
        List<Dictionary<string, string>> dictListSimbols = AVG.Inst.GetDictListFromDB<AssetData>("Simbols");
        imagePathsSimbols = RebuildFilePaths(dictListSimbols, resourceFrom, simbolFrom);

        string portraitFrom = PPM.Inst.Get("�Y�ϯ������|");
        List<Dictionary<string, string>> dictListPortraits = AVG.Inst.GetDictListFromDB<CharData>("Chars");
        imagePathsPortrait = RebuildFilePathsPortrait(dictListPortraits, resourceFrom, portraitFrom);
    }

    public Dictionary<string, string> RebuildFilePaths(List<Dictionary<string, string>> idFilenameDict, string assetRoot, string assetPath)
    {
        string fileName, filePath;
        Dictionary<string, string> pathsDictList = new();
        foreach (var item in idFilenameDict)
        {
            fileName = item["�ɦW"].ToString();
            filePath = assetRoot + "://" + assetPath + fileName;
            pathsDictList[item["AssetID"]] = filePath;
            //Debug.Log($"��l�ơG{item["AssetID"]} = {filePath}");
        }
        return pathsDictList;
    }

    public Dictionary<string, string> RebuildFilePathsPortrait(List<Dictionary<string, string>> charDataList, string assetRoot, string assetPath)
    {
        Dictionary<string, string> pathsDictList = new();
        string fileName, filePath;
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
                    fileName = charData["AssetID"] + "-" + charData[emo] + ".png";
                    filePath = assetRoot + "://" + assetPath + fileName;
                    pathsDictList[charData["UID"] + emo] = filePath;
                    //Debug.Log($"[Portrait]{charData["UID"] + emo}�G{filePath}");
                    // ex: imagePathsPortrait["���w�g��"] = A-anger�����|
                }
            }
        }
        return pathsDictList;
    }

    public string GetSimbolImgUrl(string key)
    {
        if (!imagePathsSimbols.ContainsKey(key))
        {
            Debug.LogError($"Key '{key}' does not exist in imagePaths.");
            return string.Empty;
        }
        return imagePathsSimbols[key]; ;
    }

    public string GetBackgroundImgUrl(string key)
    {
        if (!imagePathsBackground.ContainsKey(key))
        {
            Debug.LogError($"Key '{key}' does not exist in imagePaths.");
            return string.Empty;
        }
        return imagePathsBackground[key];
    }

    public string GetPortraitImgUrl(string key) //key = charUID + charEmo
    {
        if (!imagePathsPortrait.ContainsKey(key))
        {
            Debug.LogError($"Key '{key}' does not exist in imagePaths.");
            return DefaultPortraitImgUrl;
        }
        return imagePathsPortrait[key];
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
