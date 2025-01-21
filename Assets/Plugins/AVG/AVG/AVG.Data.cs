using Newtonsoft.Json;
using Story;
using System;
using System.Collections.Generic;
using System.Linq;
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

    public void InitPPM(string pageName)
    {
        // 在 PPM 中設置測試字串數據，同時支援TxR
        Debug.Log("PPM初始化");
        List<Preset> allItems = dbManager.QueryTable<Preset>(pageName);
        foreach (var item in allItems)
        {
            //Debug.Log($"{item.Key}={item.Value}");
            PPM.Inst.Set(item.Key, item.Value);
        }
    }

    public void UpdatePPM(string pageName)
    {
        // 取得所有 `Preset` 設定
        List<Preset> allItems = dbManager.QueryTable<Preset>(pageName);

        foreach (var item in allItems)
        {
            // 取得現有的 PlayerPrefs 值
            string existingValue = PPM.Inst.Get(item.Key);

            // 只有當現有值為 null 或空字串時，才更新
            if (string.IsNullOrEmpty(existingValue))
            {
                Debug.Log($"PPM更新：{item.Key}={item.Value}");
                PPM.Inst.Set(item.Key, item.Value);
            }
        }
    }


    public void SavePPM(string pageName, string slotName)
    {
        // 取得所有 `Preset` 設定
        List<Preset> allItems = dbManager.QueryTable<Preset>(pageName);
        Dictionary<string, string> saveData = new();

        foreach (var item in allItems)
        {
            // 從 PPM 取得已儲存的值
            string value = PPM.Inst.Get(item.Key);
            saveData[item.Key] = value;
        }

        // 新增存檔日期
        saveData["存檔標題"] = CurrentStoryTitle;
        saveData["存檔日期"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // 將字典轉換為 JSON 字串並儲存
        Debug.Log("PPM全部儲存");
        string jsonString = JsonConvert.SerializeObject(saveData);
        PPM.Inst.Set(slotName, jsonString);
    }

    public void LoadPPM(string pageName, string slotName)
    {
        // 從 PPM 讀取 JSON 字串
        string jsonString = PPM.Inst.Get(slotName);
        if (string.IsNullOrEmpty(jsonString))
        {
            Debug.LogWarning($"LoadPPM: 找不到 slot '{slotName}' 的數據");
            InitPPM(pageName);
            return;
        }

        // 解析 JSON 為字典
        Dictionary<string, string> loadedData = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);

        Debug.Log("PPM全部讀取");
        foreach (var kvp in loadedData)
        {
            PPM.Inst.Set(kvp.Key, kvp.Value);
        }
    }

    public Dictionary<string, string> GetSlotData(string slotName)
    {
        // 從 PPM 讀取 JSON 字串
        string jsonString = PPM.Inst.Get(slotName);
        if (string.IsNullOrEmpty(jsonString))
        {
            Debug.Log($"GetSlotData: 找不到 slot '{slotName}' 的數據");
            return null;
        }
        // 解析 JSON 為字典
        Dictionary<string, string> loadedData = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
        return loadedData;
    }

    public void FilterStories(string pageName)
    {
        List<StoryList> allItems = dbManager.QueryTable<StoryList>(pageName);

        foreach (var item in allItems)
        {
            Debug.Log($"title:{item.Title}, once:{item.Once}, cond:{item.Condition}, desc:{item.Description}");

            if (item.Once == "Y" && IsTitleInReadList(item.Title)) continue;

            if (Judge.EvaluateCondition(item.Condition))
            {
                Debug.Log($"Condition met: {item.Title}");

                // 將符合條件的 Title 添加到 PendingStories
                AVG.Inst.PendingStories.Add(item);
            }
        }
    }

    public void AddTitleToReadList(string newTitle)
    {
        if (string.IsNullOrEmpty(newTitle))
        {
            Debug.LogWarning("標題為空，無法新增！");
            return;
        }
        // 讀取現有的章節清單
        string savedList = PlayerPrefs.GetString(ReadlistKey, "");
        string[] titles = savedList.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

        // 檢查是否已存在，避免重複加入
        if (titles.Contains(newTitle))
        {
            Debug.Log($"標題 '{newTitle}' 已存在，不重複加入。");
            return;
        }

        // 新增標題並儲存回 PlayerPrefs
        string updatedList = savedList == "" ? newTitle : savedList + "," + newTitle;
        PlayerPrefs.SetString(ReadlistKey, updatedList);
        PlayerPrefs.Save();

        Debug.Log($"新增已讀標題：{newTitle}");
    }

    public bool IsTitleInReadList(string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            Debug.LogWarning("標題為空，無法檢查！");
            return false;
        }

        string savedList = PlayerPrefs.GetString(ReadlistKey, "");

        if (string.IsNullOrEmpty(savedList))
        {
            return false; // 如果沒有已讀清單，直接回傳 false
        }

        string[] titles = savedList.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

        return titles.Contains(title);
    }

}
