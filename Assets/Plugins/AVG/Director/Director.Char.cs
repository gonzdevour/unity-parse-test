using System.Collections.Generic;
using UnityEngine;

using story;
using UnityEngine.UI;

public partial class Director
{
    public IChar GetCharByUID(string charUID)
    {
        IChar CharResult = null;
        foreach (RectTransform child in Avg.LayerChar)
        {
            // 檢查子物件是否有 IChar 組件
            IChar Char = child.GetComponent<IChar>();
            if (Char != null && Char.UID == charUID)
            {
                CharResult = Char;
            }
        }
        return CharResult;
    }

    public void CharDestroyAll()
    {
        Avg.StoryPlayer.ClearPortrait();//清除頭圖

        if (Avg.LayerChar != null)
        {
            foreach (Transform child in Avg.LayerChar)
            {
                GameObject.Destroy(child.gameObject);// 清除角色
            }
        }
    }

    public IChar CharIn(Dictionary<string, string> charData, string charUID, string charPos, string charEmo, string charSimbol)
    {
        // 在 LayerChar 中尋找子物件
        IChar Char = GetCharByUID(charUID);
        if (Char != null)
        {
            Debug.Log($"畫面上存在{charUID}");
            Char.Focus(DefaultCharFocusDur); // 找到符合的 UID，執行 CharFocus
            if (!string.IsNullOrEmpty(charPos))
            {
                Vector2[] fromTo = PositionParser.ParsePos(charPos, Avg.MainPanel);
                Char.Move(fromTo, float.Parse(PPM.Inst.Get("位移秒數", "0.5"))); // 移動到指定位置
                Debug.Log($"重新定位已存在的角色{charUID}至{fromTo[1]}");
            }
            if (!string.IsNullOrEmpty(charEmo) && charEmo != Char.Expression)
            {
                Char.SetExpression(charEmo, "fade", DefaultCharTransDur); // 設定表情
                Debug.Log($"{charUID}的表情轉變為：{charEmo}");
            }
            if (!string.IsNullOrEmpty(charSimbol))
            {
                Char.SetSimbol(charSimbol); // 設定符號
                Debug.Log($"{charUID}產生符號：{charSimbol}");
            }
        }
        else
        {
            Debug.Log($"畫面上不存在{charUID}，生成角色");
            Char = CharGen(charData, charUID, charPos, charEmo, charSimbol);
        }
        return Char;
    }

    public IChar CharGen(Dictionary<string, string> charData, string charUID, string charPos, string charEmo, string charSimbol)
    {
        Vector2 SpawnPoint = PositionParser.ParsePoint(charPos, Avg.MainPanel);

        // 實例化 Prefab
        GameObject newChar;
        string resType = charData["素材類型"];
        if (resType == "Pic") 
        {
            newChar = Instantiate(Avg.CharPrefab_TImg, Avg.LayerChar); 
        }
        else
        {
            newChar = CreateModelImage(resType, charData["AssetID"]);
        }

        newChar.name = charUID;
        var newCharTransform = newChar.GetComponent<RectTransform>();
        newCharTransform.anchoredPosition = SpawnPoint;

        IChar Char = newChar.GetComponent<IChar>();
        Char.Init(charData, charEmo, charSimbol);
        return Char;
    }

    public GameObject CreateModelImage(string resType, string assetID)
    {
        // 產生 ModelImage Prefab
        GameObject MImg = Instantiate(Avg.CharPrefab_MImg, Avg.LayerChar);
        // 取出 ModelImage 中的 RawImage
        RawImage rawImg = MImg.GetComponentInChildren<RawImage>();
        // 生成 Model 讓 RawImage 透過 renderTexture 映射
        GameObject modelGO = CanvasUI.Inst.CreateModelToRawImage(GetModelUrl(resType, assetID), rawImg);
        // 將生成的 Model 與 ModelImage 綁定
        CharModel charModel = MImg.GetComponent<CharModel>();
        charModel.Model = modelGO;

        return MImg;
    }

    public void CharsUnfocusAll()
    {
        foreach (RectTransform child in Avg.LayerChar) //找出所有Char子物件
        {
            IChar Char = child.GetComponent<IChar>(); // 檢查子物件是否有 IChar 組件
            Char?.Unfocus(DefaultCharUnfocusDur); // 如果找到符合的 UID，執行 CharFocus
        }
    }

    private void MoveCharX(object[] args)
    {
        string charUID = args[0]?.ToString();
        string charPos = args[1].ToString();
        string dur = args[2]?.ToString();

        Vector2[] fromTo = PositionParser.ParsePos(charPos, Avg.MainPanel, "x"); //取x軸位移
        IChar Char = GetCharByUID(charUID);
        float duration = string.IsNullOrEmpty(dur) ? DefaultCharMoveDur : float.Parse(dur);
        Char?.MoveX(fromTo, duration);

        Debug.Log($"Move {charUID}'s X from {fromTo[0].x} to {fromTo[1].x}");
    }

    private void MoveCharY(object[] args)
    {
        string charUID = args[0]?.ToString();
        string charPos = args[1].ToString();
        string dur = args[2]?.ToString();

        Vector2[] fromTo = PositionParser.ParsePos(charPos, Avg.MainPanel, "y"); //取y軸位移
        IChar Char = GetCharByUID(charUID);
        float duration = string.IsNullOrEmpty(dur) ? DefaultCharMoveDur : float.Parse(dur);
        Char?.MoveY(fromTo, duration);

        Debug.Log($"Move {charUID}'s Y from {fromTo[0].y} to {fromTo[1].y}");
    }

    private void ChangeExpression(object[] args)
    {
        string charUID = args[0]?.ToString();
        string charEmo = args[1]?.ToString();
        Debug.Log($"[Director]嘗試將{charUID}的表情轉變為{charEmo}");
        // 在 LayerChar 中尋找子物件
        IChar Char = GetCharByUID(charUID);
        if (Char != null)
        {
            Debug.Log($"畫面上存在{charUID}");
            if (!string.IsNullOrEmpty(charEmo) && charEmo != Char.Expression)
            {
                Char.SetExpression(charEmo, "fade", DefaultCharTransDur); // 設定表情
                Debug.Log($"{charUID}的表情轉變為：{charEmo}");
            }
        }
        else
        {
            Debug.Log($"畫面上不存在{charUID}");
        }
    }
}
