using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

[System.Serializable]
public class UserData
{
    //UserInfo
    public string objectId; // SignIn����o�A�YPlayerPrefs��"userID"
    public string name;
    public string token;
    public int avatar;
    public int login;
    //Score
    public int score;

    //functions
    // ��s PlayerPrefs
    public void Update(UserData newUserData)
    {        
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(newUserData);// �N newUserData �ǦC�Ƭ� JSON        
        JObject data = JObject.Parse(json);// �ѪR JSON �� JObject

        // �w�q��M�g��
        Dictionary<string, string> keyMapping = new Dictionary<string, string>
        {
            { "objectId", "userID" } // �N objectId �M�g�� userID
        };

        foreach (var kvp in data) // �M�� JObject �ç�s PlayerPrefs
        {
            if (kvp.Value.Type == JTokenType.Null)
                continue;

            // �ھڬM�g�������������W�A�p�G JSON ���䤣�b�M�g���A�h�ϥέ�l��@���q�{�ȡC
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
        // �N����s�J "UserData" �䤤�A�@���ƥ�
        PlayerPrefs.SetString("UserData", json);
        // �O�s���
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs �w�ʺA��s�I");
        UserData.List();
    }

    public static void List()
    {
        // �w�q��M�g��
        Dictionary<string, string> keyMapping = new Dictionary<string, string>
        {
            { "objectId", "userID" } // �N objectId �M�g�� userID
        };

        Debug.Log("=== PlayerPrefs ��ȦC�� ===");

        // ����s�x�b "UserData" �䤤�� JSON
        string json = PlayerPrefs.GetString("UserData", "{}");

        try
        {
            // �ѪR JSON �� JObject
            JObject data = JObject.Parse(json);

            // �M�� JObject �æC�X���
            foreach (var kvp in data)
            {
                // �ھڬM�g�������������W�A�p�G JSON ���䤣�b�M�g���A�h�ϥέ�l��@���q�{�ȡC
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
            Debug.LogError($"�ѪR UserData JSON �ɥX��: {e.Message}");
        }

        Debug.Log("=== �C�X���� ===");
    }
}
