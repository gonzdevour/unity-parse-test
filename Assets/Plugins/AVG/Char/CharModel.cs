using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharModel : MonoBehaviour, IChar
{

    public GameObject Model;
    public Transform SimbolMarker;
    public GameObject SimbolPrefab;

    private RawImage rawImage;
    private Vector2 SimbolMarkerOriginPos;
    private Dictionary<string, string> imagePathsSimbols;

    public string UID { get; set; } // �ߤ@�ѧO�X
    public string �m { get; set; } // �m
    public string �W { get; set; } // �W
    public string �q�� { get; set; } // �q�� (�p�G����/�p�j)
    public string ¾�� { get; set; } // ¾��
    public string �ʺ�1 { get; set; } // �ʺ�1
    public string �ʺ�2 { get; set; } // �ʺ�2
    public string ��ø { get; set; } // �ʺ�2
    public string �Y�� { get; set; } // �ʺ�2
    public float Scale { get; set; } // �Y����
    public float YAdd { get; set; } // Y �b�첾
    public float SimbolX { get; set; } // simbol X����
    public float SimbolY { get; set; } // simbol Y����
    public string AssetID { get; set; } // �겣 ID
    public string Expression { get; set; } //�ثe����

    private void OnDestroy()
    {
        // ����P�Ӫ���������Ҧ� Tween�A�קK�s���w�R������
        DOTween.Kill(gameObject);
        // ����������ModelGO�A�s�P�l����camera�@�_�R��
        Destroy(Model);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void Init(Dictionary<string, string> CharData, string CharEmo = "�L", string CharSimbol = "")
    {
        // ���XrawImage
        rawImage = gameObject.GetComponentInChildren<RawImage>();
        // �]�w�Ÿ���
        imagePathsSimbols = Director.Inst.imagePathsSimbols;

        Debug.Log($"��l�ƨ����ơG{CharData["UID"]}");
        // �]�m�ݩ�
        UID = CharData.GetValueOrDefault("UID", string.Empty);
        �m = CharData.GetValueOrDefault("�m", string.Empty);
        �W = CharData.GetValueOrDefault("�W", string.Empty);
        �q�� = CharData.GetValueOrDefault("�q��", string.Empty);
        ¾�� = CharData.GetValueOrDefault("¾��", string.Empty);
        �ʺ�1 = CharData.GetValueOrDefault("�ʺ�1", string.Empty);
        �ʺ�2 = CharData.GetValueOrDefault("�ʺ�2", string.Empty);
        ��ø = CharData.GetValueOrDefault("��ø", string.Empty);
        �Y�� = CharData.GetValueOrDefault("�Y��", string.Empty);

        // �]�m Scale, YAdd, SimbolX, SimbolY
        if (CharData.TryGetValue("Scale", out string scale) && float.TryParse(scale, out float parsedScale))
        {
            Scale = parsedScale;
            //transform.localScale = Vector3.one * parsedScale;
            Model.transform.localScale = Vector3.one * parsedScale;
        }

        if (CharData.TryGetValue("YAdd", out string yAdd) && float.TryParse(yAdd, out float parsedYAdd))
        {
            YAdd = parsedYAdd;
            transform.localPosition += new Vector3(0, YAdd, 0);
        }

        if (CharData.TryGetValue("SimbolX", out string simbolX) && float.TryParse(simbolX, out float parsedSimbolX))
        {
            SimbolX = parsedSimbolX;
        }

        if (CharData.TryGetValue("SimbolY", out string simbolY) && float.TryParse(simbolY, out float parsedSimbolY))
        {
            SimbolY = parsedSimbolY;
        }

        // �]�w��l��

        // �O���Ÿ��w�]��m(�H�K�Q�����ȼv�T)
        SimbolMarkerOriginPos = new Vector2(SimbolMarker.localPosition.x, SimbolMarker.localPosition.y);
        // ���ͲŸ�
        if (!string.IsNullOrEmpty(CharSimbol))
        {
            SetSimbol(CharSimbol); // �]�w�Ÿ�
            Debug.Log($"{UID}���ͲŸ��G{CharSimbol}");
        }

        Debug.Log($"Character {UID} initialized with name {�m}{�W}.");
    }

    public void Focus(float dur = 0f)
    {
        // �N���󲾨�Ҧ��P�h���󪺳̫e��
        transform.SetAsLastSibling();
        rawImage.DOColor(new Color(1f, 1f, 1f), dur);
    }

    public void Unfocus(float dur = 0f)
    {
        rawImage.DOColor(new Color(0.3f, 0.3f, 0.3f), dur);
    }

    public void SetExpression(string expression = "�L", string transitionType = "slideup", float dur = 1f ) 
    {
    }

    public void SetSimbol(string simbolName)
    {
        if (imagePathsSimbols.ContainsKey(simbolName))
        {
            SimbolMarker.localPosition = SimbolMarkerOriginPos + new Vector2(SimbolX, SimbolY);
            Image simbolImage = Instantiate(SimbolPrefab, SimbolMarker).GetComponent<Image>();
            string simbolAddress = Director.Inst.GetSimbolImgUrl(simbolName);
            SpriteCacher.Inst.GetSprite(simbolAddress, (sprite) =>
            {
                simbolImage.sprite = sprite;
            });
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
