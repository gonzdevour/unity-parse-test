using System.Collections.Generic;
using UnityEngine;

using story;

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
        if (Avg.LayerChar != null)
        {
            foreach (Transform child in Avg.LayerChar)
            {
                GameObject.Destroy(child.gameObject);// 清除角色
            }
        }
    }

    public void CharIn(Dictionary<string, string> charData, string charUID, string charPos, string charEmo)
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
            if (!string.IsNullOrEmpty(charEmo))
            {
                Char.SetExpression(charEmo, "fade", DefaultCharTransDur); // 設定表情
                Debug.Log($"{charUID}的表情轉變為：{charEmo}");
            }
        }
        else
        {
            Debug.Log($"畫面上不存在{charUID}，生成角色");
            CharGen(charData, charUID, charPos, charEmo);
        }
    }

    public void CharGen(Dictionary<string, string> charData, string charUID, string charPos, string charEmo)
    {
        Vector2 SpawnPoint = PositionParser.ParsePoint(charPos, Avg.MainPanel);

        // 實例化 Prefab
        GameObject newChar = Instantiate(Avg.CharPrefab, Avg.LayerChar);
        newChar.name = charUID;
        var newCharTransform = newChar.GetComponent<RectTransform>();
        newCharTransform.anchoredPosition = SpawnPoint;

        IChar Char = newChar.GetComponent<IChar>();
        Char.Init(charData, charEmo);
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
        Char.MoveX(fromTo, duration);

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
        Char.MoveY(fromTo, duration);

        Debug.Log($"Move {charUID}'s Y from {fromTo[0].y} to {fromTo[1].y}");
    }
}
