using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

[System.Serializable]
public class UserData
{
    //UserInfo
    public string objectId; // SignIn後取得，即PlayerPrefs的"userID"
    public string name;
    public string token;
    public int avatar;
    public int login;
    //Score
    public int score;

    //functions
    // 更新 PlayerPrefs
    public void Update(UserData newUserData)
    {        
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(newUserData);// 將 newUserData 序列化為 JSON        
        JObject data = JObject.Parse(json);// 解析 JSON 為 JObject

        // 定義鍵映射表
        Dictionary<string, string> keyMapping = new Dictionary<string, string>
        {
            { "objectId", "userID" } // 將 objectId 映射為 userID
        };

        foreach (var kvp in data) // 遍歷 JObject 並更新 PlayerPrefs
        {
            if (kvp.Value.Type == JTokenType.Null)
                continue;

            // 根據映射表獲取對應的鍵名，如果 JSON 的鍵不在映射表中，則使用原始鍵作為默認值。
            string targetKey = keyMapping.ContainsKey(kvp.Key) ? keyMapping[kvp.Key] : kvp.Key;

            switch (kvp.Value.Type)
            {
                case JTokenType.String:
                    PlayerPrefs.SetString(targetKey, kvp.Value.ToString());
                    break;

                case JTokenType.Integer:
                    PlayerPrefs.SetInt(targetKey, kvp.Value.ToObject<int>());
                    break;

                case JTokenType.Boolean:
                    PlayerPrefs.SetInt(targetKey, kvp.Value.ToObject<bool>() ? 1 : 0);
                    break;

                default:
                    Debug.LogWarning($"Unsupported type for key: {kvp.Key}");
                    break;
            }
        }
        // 將整體存入 "UserData" 鍵中，作為備份
        PlayerPrefs.SetString("UserData", json);
        // 保存更改
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs 已動態更新！");
        UserData.List();
    }

    public static void List()
    {
        // 定義鍵映射表
        Dictionary<string, string> keyMapping = new Dictionary<string, string>
        {
            { "objectId", "userID" } // 將 objectId 映射為 userID
        };

        Debug.Log("=== PlayerPrefs 鍵值列表 ===");

        // 獲取存儲在 "UserData" 鍵中的 JSON
        string json = PlayerPrefs.GetString("UserData", "{}");

        try
        {
            // 解析 JSON 為 JObject
            JObject data = JObject.Parse(json);

            // 遍歷 JObject 並列出鍵值
            foreach (var kvp in data)
            {
                // 根據映射表獲取對應的鍵名，如果 JSON 的鍵不在映射表中，則使用原始鍵作為默認值。
                string targetKey = keyMapping.ContainsKey(kvp.Key) ? keyMapping[kvp.Key] : kvp.Key;
                string value = kvp.Value.Type switch
                {
                    JTokenType.String => kvp.Value.ToString(),
                    JTokenType.Integer => kvp.Value.ToString(),
                    JTokenType.Float => kvp.Value.ToString(),
                    JTokenType.Boolean => kvp.Value.ToObject<bool>() ? "true" : "false",
                    JTokenType.Null => "null",
                    _ => "Unsupported Type"
                };

                Debug.Log($"{targetKey}: {value}");
            }
        }
        catch (JsonException e)
        {
            Debug.LogError($"解析 UserData JSON 時出錯: {e.Message}");
        }

        Debug.Log("=== 列出結束 ===");
    }
}
