using Newtonsoft.Json;
using Story;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class AVG
{
    // �G��Ū����|autosave�A���save�u��Nautosave���ѼưO������wslot�A�קK�G��Ū���e���Ѽ��ܰʳQ�x�s��slot
    public void Save(string pageName, string slotName)
    {
        if (slotName == "AVGSaveSlotAuto")
        {
            SavePPM(pageName, slotName);
        }
        else
        {
            // �q PPM Ū�� JSON �r��
            string jsonString = PPM.Inst.Get("AVGSaveSlotAuto");
            PPM.Inst.Set(slotName, jsonString);
        }
    }

    public void Load(string pageName, string slotName)
    {
        LoadPPM(pageName, slotName);
        // �NPendingStories�]���s�ɤ�����Ū�@��(�p�G��)
        Dictionary<string, string> saveData = GetSlotData(slotName);
        var stories = JsonConvert.DeserializeObject<List<StoryMeta>>(saveData["��Ū�@��"]) ?? new List<StoryMeta>();
        Debug.Log($"�Ĥ@�ӫ�Ū�@�����D�G{stories[0].Title}");
        // ����AVG
        StartCoroutine(AVGRestart(stories));
    }

    public void InitPPM(string pageName)
    {
        // �b PPM ���]�m���զr��ƾڡA�P�ɤ䴩TxR
        Debug.Log("PPM��l��");
        List<Preset> allItems = dbManager.QueryTable<Preset>(pageName);
        foreach (var item in allItems)
        {
            //Debug.Log($"{item.Key}={item.Value}");
            PPM.Inst.Set(item.Key, item.Value);
        }
    }

    public void UpdatePPM(string pageName)
    {
        // ���o�Ҧ� `Preset` �]�w
        List<Preset> allItems = dbManager.QueryTable<Preset>(pageName);

        foreach (var item in allItems)
        {
            // ���o�{���� PlayerPrefs ��
            string existingValue = PPM.Inst.Get(item.Key);

            // �u����{���Ȭ� null �ΪŦr��A�θ�item�ݩʬ�AlwaysUpdate�ɡA�~��s
            if (string.IsNullOrEmpty(existingValue) || item.AlwaysUpdate == "Y")
            {
                Debug.Log($"PPM��s�G{item.Key}={item.Value}");
                PPM.Inst.Set(item.Key, item.Value);
            }
        }
    }

    public void SavePPM(string pageName, string slotName)
    {
        // ���o�Ҧ� `Preset` �]�w
        List<Preset> allItems = dbManager.QueryTable<Preset>(pageName);
        Dictionary<string, string> saveData = new();

        foreach (var item in allItems)
        {
            // �q PPM ���o�w�x�s����
            string value = PPM.Inst.Get(item.Key);
            saveData[item.Key] = value;
        }

        // �s�W�s�ɤ��
        saveData["�s�ɼ��D"] = CurrentStoryTitle;
        saveData["�s�ɤ��"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        saveData["��Ū�@��"] = JsonConvert.SerializeObject(PendingStoriesForSave);

        // �N�r���ഫ�� JSON �r����x�s
        Debug.Log("PPM�����x�s");
        string jsonString = JsonConvert.SerializeObject(saveData);
        PPM.Inst.Set(slotName, jsonString);
    }

    public void LoadPPM(string pageName, string slotName)
    {
        // �q PPM Ū�� JSON �r��
        string jsonString = PPM.Inst.Get(slotName);
        if (string.IsNullOrEmpty(jsonString))
        {
            Debug.LogWarning($"LoadPPM: �䤣�� slot '{slotName}' ���ƾ�");
            InitPPM(pageName);
            return;
        }

        // �ѪR JSON ���r��
        Dictionary<string, string> loadedData = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);

        Debug.Log("PPM����Ū��");
        foreach (var kvp in loadedData)
        {
            PPM.Inst.Set(kvp.Key, kvp.Value);
        }
    }

    public Dictionary<string, string> GetSlotData(string slotName)
    {
        // �q PPM Ū�� JSON �r��
        string jsonString = PPM.Inst.Get(slotName);
        if (string.IsNullOrEmpty(jsonString))
        {
            Debug.Log($"GetSlotData: �䤣�� slot '{slotName}' ���ƾ�");
            return null;
        }
        // �ѪR JSON ���r��
        Dictionary<string, string> loadedData = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
        return loadedData;
    }

    public Dictionary<string, string> GetCharDataByUID(string pageName, string UID)
    {
        // ��l�Ʊ���
        string condition = $"UID = '{UID}'";

        // �I�s QueryTable ���
        List<CharData> results = dbManager.QueryTable<CharData>(pageName, condition);

        CharData charData = null;
        // ����Ĥ@���d�ߵ��G
        if (results.Count > 0)
        {
            charData = results[0];
            //Debug.Log($"����ơG{bgData.�W} ({bgData.UID})");
        }
        else
        {
            //Debug.Log("�d�ߵL���G");
        }

        var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(charData));

        return dict;
    }

    public List<Dictionary<string, string>> GetCharDataAll(string pageName)
    {
        // �I�s QueryTable ���
        List<CharData> results = dbManager.QueryTable<CharData>(pageName);
        var dictList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(
                JsonConvert.SerializeObject(results)
            );

        return dictList;
    }

    public List<Dictionary<string, string>> GetBgData(string pageName)
    {
        // �I�s QueryTable ���
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
            var assetRoot = PPM.Inst.Get("�����ӷ�");
            var assetPath = PPM.Inst.Get("�Y�ϯ������|");
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

                // �N�ŦX���� Title �K�[�� PendingStories
                resultStories.Add(item);
            }
        }
        return resultStories;
    }

    public void PrependStoryByTitle(string title)
    {
        StoryMeta item = new(){ Title = title };
        Debug.Log($"[�[�J�G�Ʀ�(�Y)]{item.Title}");
        PendingStories.Insert(0, item);
    }

    public void AppendStoryByTitle(string title)
    {
        StoryMeta item = new() { Title = title };
        Debug.Log($"[�[�J�G�Ʀ�(��)]{item.Title}");
        PendingStories.Add(item);
    }

    public StoryMeta GetStoryByTitle(string pageName, string title)
    {
        // �T�O title �Q���T�a�]�b��޸���
        string condition = $"Title = '{title}'";

        // �d�߸�Ʈw
        List<StoryMeta> results = dbManager.QueryTable<StoryMeta>(pageName, condition);

        // �p�G���G����ơA�^�ǲĤ@���F�_�h�^�� null
        return results.Count > 0 ? results[0] : null;
    }

    public void AddTitleToReadList(string newTitle)
    {
        if (string.IsNullOrEmpty(newTitle))
        {
            Debug.LogWarning("���D���šA�L�k�s�W�I");
            return;
        }
        // Ū���{�������`�M��
        string savedList = PlayerPrefs.GetString(ReadlistKey, "");
        string[] titles = savedList.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

        // �ˬd�O�_�w�s�b�A�קK���ƥ[�J
        if (titles.Contains(newTitle))
        {
            Debug.Log($"���D '{newTitle}' �w�s�b�A�����ƥ[�J�C");
            return;
        }

        // �s�W���D���x�s�^ PlayerPrefs
        string updatedList = savedList == "" ? newTitle : savedList + "," + newTitle;
        PlayerPrefs.SetString(ReadlistKey, updatedList);
        PlayerPrefs.Save();

        Debug.Log($"�s�W�wŪ���D�G{newTitle}");
    }

    public bool IsTitleInReadList(string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            Debug.LogWarning("���D���šA�L�k�ˬd�I");
            return false;
        }

        string savedList = PlayerPrefs.GetString(ReadlistKey, "");

        if (string.IsNullOrEmpty(savedList))
        {
            return false; // �p�G�S���wŪ�M��A�����^�� false
        }

        string[] titles = savedList.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

        return titles.Contains(title);
    }
}
