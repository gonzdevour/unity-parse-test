using Newtonsoft.Json;
using Story;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class AVG
{
    // 故事讀完後會autosave，手動save只能將autosave的參數記錄到指定slot，避免故事讀完前的參數變動被儲存到slot
    public void Save(string pageName, string slotName)
    {
        if (slotName == "AVGSaveSlotAuto")
        {
            SavePPM(pageName, slotName);
        }
        else
        {
            // 從 PPM 讀取 JSON 字串
            string jsonString = PPM.Inst.Get("AVGSaveSlotAuto");
            PPM.Inst.Set(slotName, jsonString);
        }
    }

    public void Load(string pageName, string slotName)
    {
        LoadPPM(pageName, slotName);
        // 將PendingStories設為存檔中的待讀劇本(如果有)
        Dictionary<string, string> saveData = GetSlotData(slotName);
        var stories = JsonConvert.DeserializeObject<List<StoryMeta>>(saveData["待讀劇本"]) ?? new List<StoryMeta>();
        Debug.Log($"第一個待讀劇本標題：{stories[0].Title}");
        // 重啟AVG
        StartCoroutine(AVGRestart(stories));
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

            // 只有當現有值為 null 或空字串，或該item屬性為AlwaysUpdate時，才更新
            if (string.IsNullOrEmpty(existingValue) || item.AlwaysUpdate == "Y")
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
        saveData["待讀劇本"] = JsonConvert.SerializeObject(PendingStoriesForSave);

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

    public List<StoryMeta> FilterStories(string pageName, string condition = "")
    {
        List<StoryMeta> resultStories = new();
        List<StoryMeta> allItems = dbManager.QueryTable<StoryMeta>(pageName, condition);

        foreach (var item in allItems)
        {
            Debug.Log($"title:{item.Title}, once:{item.Once}, cond:{item.Condition}, desc:{item.Description}, tag:{item.Tag}");

            if (item.Once == "Y" && IsTitleInReadList(item.Title)) continue;

            if (Judge.EvaluateCondition(item.Condition))
            {
                Debug.Log($"Condition met: {item.Title}");

                // 將符合條件的 Title 添加到 PendingStories
                resultStories.Add(item);
            }
        }
        return resultStories;
    }

    public void PrependStoryByTitle(string title)
    {
        StoryMeta item = new(){ Title = title };
        Debug.Log($"[加入故事串(頭)]{item.Title}");
        PendingStories.Insert(0, item);
    }

    public void AppendStoryByTitle(string title)
    {
        StoryMeta item = new() { Title = title };
        Debug.Log($"[加入故事串(尾)]{item.Title}");
        PendingStories.Add(item);
    }

    public StoryMeta GetStoryByTitle(string pageName, string title)
    {
        // 確保 title 被正確地包在單引號內
        string condition = $"Title = '{title}'";

        // 查詢資料庫
        List<StoryMeta> results = dbManager.QueryTable<StoryMeta>(pageName, condition);

        // 如果結果有資料，回傳第一筆；否則回傳 null
        return results.Count > 0 ? results[0] : null;
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
