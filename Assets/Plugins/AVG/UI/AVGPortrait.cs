using System.Collections.Generic;
using UnityEngine;

public class AVGPortrait : MonoBehaviour
{
    public TransitionImage Portrait;
    private Dictionary<string, string> imagePaths = new Dictionary<string, string>(); // �x�s�Ϥ��귽���|

    public void Init(List<Dictionary<string, string>> imageFileNamesDictList)
    {
        // �w�q���ݩ���ȦC��
        string emoTypes = PPM.Inst.Get("�������C��"); // "�L,��,��,��,��,��,�w"
        string[] emos = emoTypes.Split(",");
        foreach (var item in imageFileNamesDictList)
        {
            // �զX�C�Ӫ�����ȹ�
            foreach (string emo in emos)
            {
                if (item.ContainsKey(emo) && !string.IsNullOrEmpty(item[emo]))
                {
                    imagePaths[item["UID"] + emo] = item["AssetID"] + "-" + item[emo] + ".png";
                    // ex: imagePaths["���w�g��"] = A-anger
                }
            }
        }
    }
    public void GoTo(string key, string effectType = "fade", float duration = 0.5f)
    {
        if (!imagePaths.ContainsKey(key))
        {
            Debug.LogError($"Key '{key}' does not exist in imagePaths.");
            return;
        }
        var fileName = imagePaths[key];
        var assetRoot = PPM.Inst.Get("�����ӷ�");
        var assetPath = PPM.Inst.Get("�Y�ϯ������|");
        var imagePath = assetRoot + "://" + assetPath + fileName;

        Portrait.StartTransition(imagePath, effectType, duration);
    }

    public void Clear()
    {
        Portrait.Clear();
    }
}
