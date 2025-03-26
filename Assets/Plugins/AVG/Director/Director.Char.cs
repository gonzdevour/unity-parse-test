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
            // �ˬd�l����O�_�� IChar �ե�
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
        Avg.StoryPlayer.ClearPortrait();//�M���Y��

        if (Avg.LayerChar != null)
        {
            foreach (Transform child in Avg.LayerChar)
            {
                GameObject.Destroy(child.gameObject);// �M������
            }
        }
    }

    public IChar CharIn(Dictionary<string, string> charData, string charUID, string charPos, string charEmo, string charSimbol)
    {
        // �b LayerChar ���M��l����
        IChar Char = GetCharByUID(charUID);
        if (Char != null)
        {
            Debug.Log($"�e���W�s�b{charUID}");
            Char.Focus(DefaultCharFocusDur); // ���ŦX�� UID�A���� CharFocus
            if (!string.IsNullOrEmpty(charPos))
            {
                Vector2[] fromTo = PositionParser.ParsePos(charPos, Avg.MainPanel);
                Char.Move(fromTo, float.Parse(PPM.Inst.Get("�첾���", "0.5"))); // ���ʨ���w��m
                Debug.Log($"���s�w��w�s�b������{charUID}��{fromTo[1]}");
            }
            if (!string.IsNullOrEmpty(charEmo) && charEmo != Char.Expression)
            {
                Char.SetExpression(charEmo, "fade", DefaultCharTransDur); // �]�w��
                Debug.Log($"{charUID}�������ܬ��G{charEmo}");
            }
            if (!string.IsNullOrEmpty(charSimbol))
            {
                Char.SetSimbol(charSimbol); // �]�w�Ÿ�
                Debug.Log($"{charUID}���ͲŸ��G{charSimbol}");
            }
        }
        else
        {
            Debug.Log($"�e���W���s�b{charUID}�A�ͦ�����");
            Char = CharGen(charData, charUID, charPos, charEmo, charSimbol);
        }
        return Char;
    }

    public IChar CharGen(Dictionary<string, string> charData, string charUID, string charPos, string charEmo, string charSimbol)
    {
        Vector2 SpawnPoint = PositionParser.ParsePoint(charPos, Avg.MainPanel);

        // ��Ҥ� Prefab
        GameObject newChar;
        string resType = charData["��������"];
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
        // ���� ModelImage Prefab
        GameObject MImg = Instantiate(Avg.CharPrefab_MImg, Avg.LayerChar);
        // ���X ModelImage ���� RawImage
        RawImage rawImg = MImg.GetComponentInChildren<RawImage>();
        // �ͦ� Model �� RawImage �z�L renderTexture �M�g
        GameObject modelGO = CanvasUI.Inst.CreateModelToRawImage(GetModelUrl(resType, assetID), rawImg);
        // �N�ͦ��� Model �P ModelImage �j�w
        CharModel charModel = MImg.GetComponent<CharModel>();
        charModel.Model = modelGO;

        return MImg;
    }

    public void CharsUnfocusAll()
    {
        foreach (RectTransform child in Avg.LayerChar) //��X�Ҧ�Char�l����
        {
            IChar Char = child.GetComponent<IChar>(); // �ˬd�l����O�_�� IChar �ե�
            Char?.Unfocus(DefaultCharUnfocusDur); // �p�G���ŦX�� UID�A���� CharFocus
        }
    }

    private void MoveCharX(object[] args)
    {
        string charUID = args[0]?.ToString();
        string charPos = args[1].ToString();
        string dur = args[2]?.ToString();

        Vector2[] fromTo = PositionParser.ParsePos(charPos, Avg.MainPanel, "x"); //��x�b�첾
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

        Vector2[] fromTo = PositionParser.ParsePos(charPos, Avg.MainPanel, "y"); //��y�b�첾
        IChar Char = GetCharByUID(charUID);
        float duration = string.IsNullOrEmpty(dur) ? DefaultCharMoveDur : float.Parse(dur);
        Char?.MoveY(fromTo, duration);

        Debug.Log($"Move {charUID}'s Y from {fromTo[0].y} to {fromTo[1].y}");
    }

    private void ChangeExpression(object[] args)
    {
        string charUID = args[0]?.ToString();
        string charEmo = args[1]?.ToString();
        Debug.Log($"[Director]���ձN{charUID}�������ܬ�{charEmo}");
        // �b LayerChar ���M��l����
        IChar Char = GetCharByUID(charUID);
        if (Char != null)
        {
            Debug.Log($"�e���W�s�b{charUID}");
            if (!string.IsNullOrEmpty(charEmo) && charEmo != Char.Expression)
            {
                Char.SetExpression(charEmo, "fade", DefaultCharTransDur); // �]�w��
                Debug.Log($"{charUID}�������ܬ��G{charEmo}");
            }
        }
        else
        {
            Debug.Log($"�e���W���s�b{charUID}");
        }
    }
}
