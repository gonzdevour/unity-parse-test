using System.Collections.Generic;
using UnityEngine;

public class AVGBackground : MonoBehaviour
{
    public TransitionImage Background;
    private Dictionary<string, string> imagePaths = new Dictionary<string, string>(); // �x�s�Ϥ��귽���|

    public void Init(List<Dictionary<string, string>> imageFileNamesDictList)
    {
        foreach (var item in imageFileNamesDictList)
        {
            imagePaths[item["UID"]] = item["���ɦW��"];
            Debug.Log($"��l�ơG{item["UID"]}={item["���ɦW��"]}" );
        }
    }

    public void GoTo(string key, string effectType = "fade", float duration = 2f)
    {
        if (!imagePaths.ContainsKey(key))
        {
            Debug.LogError($"Key '{key}' does not exist in imagePaths.");
            return;
        }
        var fileName = imagePaths[key];
        var assetRoot = PPM.Inst.Get("�����ӷ�");
        var assetPath = PPM.Inst.Get("�I���������|");
        var imagePath = assetRoot + "://" + assetPath + fileName;

        Background.StartTransition(imagePath, effectType, duration);
    }

    public void Clear()
    {
        Background.Clear();
    }
}
