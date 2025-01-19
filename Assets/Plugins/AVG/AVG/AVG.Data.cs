using Newtonsoft.Json;
using Story;
using System.Collections.Generic;
using UnityEngine;

public partial class AVG
{
    public Dictionary<string, string> GetCharDataByUID(string pageName, string UID)
    {
        // 初始化條件
        string condition = $"UID = '{UID}'";

        // 呼叫 QueryTable 函數
        List<CharData> results = dbManager.QueryTable<CharData>(pageName, condition);

        CharData charData = null;
        // 獲取第一筆查詢結果
        if (results.Count > 0)
        {
            charData = results[0];
            //Debug.Log($"找到資料：{bgData.名} ({bgData.UID})");
        }
        else
        {
            //Debug.Log("查詢無結果");
        }

        var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(charData));

        return dict;
    }

    public List<Dictionary<string, string>> GetCharDataAll(string pageName)
    {
        // 呼叫 QueryTable 函數
        List<CharData> results = dbManager.QueryTable<CharData>(pageName);
        var dictList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(
                JsonConvert.SerializeObject(results)
            );

        return dictList;
    }

    public List<Dictionary<string, string>> GetBgData(string pageName)
    {
        // 呼叫 QueryTable 函數
        List<BgData> results = dbManager.QueryTable<BgData>(pageName);
        var dictList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(
                JsonConvert.SerializeObject(results)
            );

        return dictList;
    }

    public string GetPortraitImgUrl(string charUID, string charEmo)
    {
        var key = charUID + charEmo;
        var imgPathsPortrait = Director.Inst.imagePathsPortrait;
        string imagePath = Director.Inst.DefaultPortraitImgUrl;
        if (imgPathsPortrait.ContainsKey(key))
        {
            var fileName = imgPathsPortrait[key];
            var assetRoot = PPM.Inst.Get("素材來源");
            var assetPath = PPM.Inst.Get("頭圖素材路徑");
            imagePath = assetRoot + "://" + assetPath + fileName;
        }
        return imagePath;
    }

    public void UpdatePPM(string pageName)
    {
        // 在 PPM 中設置測試字串數據，同時支援TxR
        List<Preset> allItems = dbManager.QueryTable<Preset>(pageName);
        foreach (var item in allItems)
        {
            //Debug.Log($"{item.Key}={item.Value}");
            PPM.Inst.Set(item.Key, item.Value);
        }
    }

    public void FilterStories(string pageName)
    {
        List<StoryList> allItems = dbManager.QueryTable<StoryList>(pageName);

        foreach (var item in allItems)
        {
            Debug.Log($"title:{item.Title}, cond:{item.Condition}, desc:{item.Description}");

            if (Judge.EvaluateCondition(item.Condition))
            {
                Debug.Log($"Condition met: {item.Title}");

                // 將符合條件的 Title 添加到 PendingStoryTitles
                AVG.Inst.PendingStoryTitles.Add(item.Title);
            }
        }
    }
}
