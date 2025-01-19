using Newtonsoft.Json;
using Story;
using System.Collections.Generic;
using UnityEngine;

public partial class AVG
{
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

    public void UpdatePPM(string pageName)
    {
        // �b PPM ���]�m���զr��ƾڡA�P�ɤ䴩TxR
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

                // �N�ŦX���� Title �K�[�� PendingStoryTitles
                AVG.Inst.PendingStoryTitles.Add(item.Title);
            }
        }
    }
}
