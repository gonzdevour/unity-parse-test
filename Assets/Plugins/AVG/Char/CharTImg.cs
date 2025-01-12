using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharTImg : MonoBehaviour, IChar
{
    public Image TImg0;
    public Image TImg1;

    public string UID { get; set; } // �ߤ@�ѧO�X
    public string �m { get; set; } // �m
    public string �W { get; set; } // �W
    public string �q�� { get; set; } // �q�� (�p�G����/�p�j)
    public string ¾�� { get; set; } // ¾��
    public string �ʺ�1 { get; set; } // �ʺ�1
    public string �ʺ�2 { get; set; } // �ʺ�2
    public float Scale { get; set; } // �Y����
    public int YAdd { get; set; } // Y �b�첾
    public string AssetID { get; set; } // �겣 ID

    // �������ݩ�
    public string �L { get; set; } // �L��
    public string �� { get; set; } // ��
    public string �� { get; set; } // ��
    public string �� { get; set; } // ��
    public string �� { get; set; } // ��
    public string �� { get; set; } // ��
    public string �w { get; set; } // �w

    public void Init(Dictionary<string, string> CharData)
    {
        Debug.Log($"��l�ƨ����ơG{CharData["UID"]}");
        // �]�m�ݩ�
        UID = CharData.GetValueOrDefault("UID", string.Empty);
        �m = CharData.GetValueOrDefault("�m", string.Empty);
        �W = CharData.GetValueOrDefault("�W", string.Empty);
        �q�� = CharData.GetValueOrDefault("�q��", string.Empty);
        ¾�� = CharData.GetValueOrDefault("¾��", string.Empty);
        �ʺ�1 = CharData.GetValueOrDefault("�ʺ�1", string.Empty);
        �ʺ�2 = CharData.GetValueOrDefault("�ʺ�2", string.Empty);
        Debug.Log($"��l�ƨ����ơG{UID}");

        // �]�m Scale �M YAdd
        if (CharData.TryGetValue("Scale", out string scale) && float.TryParse(scale, out float parsedScale))
        {
            Scale = parsedScale;
            //transform.localScale = Vector3.one * parsedScale;
        }

        if (CharData.TryGetValue("YAdd", out string yAdd) && int.TryParse(yAdd, out int parsedYAdd))
        {
            YAdd = parsedYAdd;
            transform.localPosition += new Vector3(0, parsedYAdd, 0);
        }

        // �겣 ID
        AssetID = CharData.GetValueOrDefault("AssetID", string.Empty);
        // �������ݩ�
        �L = CharData.GetValueOrDefault("�L", string.Empty);
        �� = CharData.GetValueOrDefault("��", string.Empty);
        �� = CharData.GetValueOrDefault("��", string.Empty);
        �� = CharData.GetValueOrDefault("��", string.Empty);
        �� = CharData.GetValueOrDefault("��", string.Empty);
        �� = CharData.GetValueOrDefault("��", string.Empty);
        �w = CharData.GetValueOrDefault("�w", string.Empty);

        Debug.Log($"Character {UID} initialized with name {�m}{�W}.");
    }

    public void Focus()
    {
        // �N���󲾨�Ҧ��P�h���󪺳̫e��
        transform.SetAsLastSibling();
        // �ϥ� DOTween �N TImg0 �M TImg1 ���C���ܬ� RGB = 1
        if (TImg0 != null)
        {
            TImg0.DOColor(new Color(1f, 1f, 1f), 0.5f);
        }

        if (TImg1 != null)
        {
            TImg1.DOColor(new Color(1f, 1f, 1f), 0.5f);
        }
    }

    public void Unfocus()
    {
        // �ϥ� DOTween �N TImg0 �M TImg1 ���C���ܬ� RGB = 0.3
        if (TImg0 != null)
        {
            TImg0.DOColor(new Color(0.3f, 0.3f, 0.3f), 0.5f);
        }

        if (TImg1 != null)
        {
            TImg1.DOColor(new Color(0.3f, 0.3f, 0.3f), 0.5f);
        }
    }
    public void Move(Vector2[] fromTo, float dur = 0f)
    {
        if (fromTo == null || fromTo.Length < 2)
        {
            Debug.LogError("fromTo array must contain at least 2 points.");
            return;
        }

        Vector2 fromPoint = fromTo[0] + new Vector2(0, YAdd);
        Vector2 toPoint = fromTo[1] + new Vector2(0, YAdd);

        // �]�m��l��m
        var rect = GetComponent<RectTransform>();
        rect.anchoredPosition = fromPoint;

        // �_���I���P�ɡA�ϥ� DOTween �i�沾��
        if (fromPoint != toPoint)
        {
            rect.DOAnchorPos(toPoint, dur).SetEase(Ease.Linear).OnComplete(() =>
            {
                Debug.Log($"Character {UID} moved from {fromPoint} to {toPoint} in {dur} seconds.");
            });
        }
    }

    public void MoveX(Vector2[] fromTo, float dur = 0f)
    {
        Vector2 fromPoint = fromTo[0] + new Vector2(0, YAdd);
        Vector2 toPoint = fromTo[1] + new Vector2(0, YAdd);

        // �]�m��l��m
        var rect = GetComponent<RectTransform>();
        Vector2 currentPos = rect.anchoredPosition;
        rect.anchoredPosition = new Vector2(fromPoint.x, currentPos.y); // �u��s X �b

        // �_���I���P�ɡA�ϥ� DOTween �i�沾��
        if (fromPoint.x != toPoint.x)
        {
            rect.DOAnchorPosX(toPoint.x, dur).SetEase(Ease.Linear).OnComplete(() =>
            {
                Debug.Log($"Character {UID} moved on X-axis from {fromPoint.x} to {toPoint.x} in {dur} seconds.");
            });
        }
    }
    public void MoveY(Vector2[] fromTo, float dur = 0f)
    {
        Vector2 fromPoint = fromTo[0] + new Vector2(0, YAdd);
        Vector2 toPoint = fromTo[1] + new Vector2(0, YAdd);

        // �]�m��l��m
        var rect = GetComponent<RectTransform>();
        Vector2 currentPos = rect.anchoredPosition;
        rect.anchoredPosition = new Vector2(currentPos.x, fromPoint.y); // �u��s Y �b

        // �_���I���P�ɡA�ϥ� DOTween �i�沾��
        if (fromPoint.y != toPoint.y)
        {
            rect.DOAnchorPosY(toPoint.y, dur).SetEase(Ease.Linear).OnComplete(() =>
            {
                Debug.Log($"Character {UID} moved on Y-axis from {fromPoint.y} to {toPoint.y} in {dur} seconds.");
            });
        }
    }
}
