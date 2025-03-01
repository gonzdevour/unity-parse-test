using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class CharTImg : MonoBehaviour, IChar
{
    public TransitionImage TImg;
    public Image TImg0;
    public Image TImg1;
    public GameObject SimbolMarker;
    public GameObject SimbolPrefab;
    private Dictionary<string, string> imagePathsExpression = new(); // �o�Ө��⪺����ø���|��
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

    public void Init(Dictionary<string, string> CharData, string CharEmo = "�L", string CharSimbol = "")
    {
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

        // ���o�����⪺�Ҧ������|
        imagePathsExpression = Director.Inst.GetPaths_CharExpressions(CharData);

        // �]�w��l��
        Expression = CharEmo;
        if (imagePathsExpression != null)
        {
            // ���C������
            if (imagePathsExpression.ContainsKey(CharEmo))
            {
                // ���w������
                SpriteCacher.Inst.GetSprite(imagePathsExpression[CharEmo], (sprite) =>
                {
                    TImg0.sprite = sprite;
                    TImg0.SetNativeSize();
                });
            }
            else
            {
                // ���w���L�ȡA�M��w�]��
                string defaultEmoKey = GetDefaultExpression("�L");
                Expression = defaultEmoKey;
                SpriteCacher.Inst.GetSprite(imagePathsExpression[defaultEmoKey], (sprite) =>
                {
                    TImg0.sprite = sprite;
                    TImg0.SetNativeSize();
                });
            }
        }
        else
        {
            // ���C���L��
            Debug.Log($"{UID} has no expressions");
        }

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

        //// �����N TImg0 �M TImg1 ���C���ܬ� RGB = 1
        //TImg0.color = new Color(1f, 1f, 1f);
        //TImg1.color = new Color(1f, 1f, 1f);

        // �ϥ� DOTween �N TImg0 �M TImg1 ���C���ܬ� RGB = 1
        if (TImg0 != null)
        {
            DOTween.To(() => TImg0.color.r, x =>
            {
                Color newColor = TImg0.color;
                newColor.r = x;
                TImg0.color = newColor;
            }, 1f, dur).SetEase(Ease.Linear);

            DOTween.To(() => TImg0.color.g, x =>
            {
                Color newColor = TImg0.color;
                newColor.g = x;
                TImg0.color = newColor;
            }, 1f, dur).SetEase(Ease.Linear);

            DOTween.To(() => TImg0.color.b, x =>
            {
                Color newColor = TImg0.color;
                newColor.b = x;
                TImg0.color = newColor;
            }, 1f, dur).SetEase(Ease.Linear);
        }

        if (TImg1 != null)
        {
            DOTween.To(() => TImg1.color.r, x =>
            {
                Color newColor = TImg1.color;
                newColor.r = x;
                TImg1.color = newColor;
            }, 1f, dur).SetEase(Ease.Linear);

            DOTween.To(() => TImg1.color.g, x =>
            {
                Color newColor = TImg1.color;
                newColor.g = x;
                TImg1.color = newColor;
            }, 1f, dur).SetEase(Ease.Linear);

            DOTween.To(() => TImg1.color.b, x =>
            {
                Color newColor = TImg1.color;
                newColor.b = x;
                TImg1.color = newColor;
            }, 1f, dur).SetEase(Ease.Linear);
        }
    }

    public void Unfocus(float dur = 0f)
    {
        //// �����N TImg0 �M TImg1 ���C���ܬ� RGB = 0.3
        //TImg0.color = new Color(0.3f, 0.3f, 0.3f);
        //TImg1.color = new Color(0.3f, 0.3f, 0.3f);

        // �ϥ� DOTween �N TImg0 �M TImg1 ���C���ܬ� RGB = 0.3
        if (TImg0 != null)
        {
            DOTween.To(() => TImg0.color.r, x =>
            {
                Color newColor = TImg0.color;
                newColor.r = x;
                TImg0.color = newColor;
            }, 0.3f, dur).SetEase(Ease.Linear);

            DOTween.To(() => TImg0.color.g, x =>
            {
                Color newColor = TImg0.color;
                newColor.g = x;
                TImg0.color = newColor;
            }, 0.3f, dur).SetEase(Ease.Linear);

            DOTween.To(() => TImg0.color.b, x =>
            {
                Color newColor = TImg0.color;
                newColor.b = x;
                TImg0.color = newColor;
            }, 0.3f, dur).SetEase(Ease.Linear);
        }

        if (TImg1 != null)
        {
            DOTween.To(() => TImg1.color.r, x =>
            {
                Color newColor = TImg1.color;
                newColor.r = x;
                TImg1.color = newColor;
            }, 0.3f, dur).SetEase(Ease.Linear);

            DOTween.To(() => TImg1.color.g, x =>
            {
                Color newColor = TImg1.color;
                newColor.g = x;
                TImg1.color = newColor;
            }, 0.3f, dur).SetEase(Ease.Linear);

            DOTween.To(() => TImg1.color.b, x =>
            {
                Color newColor = TImg1.color;
                newColor.b = x;
                TImg1.color = newColor;
            }, 0.3f, dur).SetEase(Ease.Linear);
        }
    }

    public void SetExpression(string expression = "�L", string transitionType = "slideup", float dur = 1f ) 
    {
        if (string.IsNullOrEmpty(expression)) expression = GetDefaultExpression("�L");
        Debug.Log("���ഫ�S�ġG" + transitionType);
        string imgUrl = imagePathsExpression["�L"]; ;
        if (imagePathsExpression.ContainsKey(expression))
        {
            imgUrl = imagePathsExpression[expression];
        }
        else
        {
            Debug.LogWarning($"Key '{expression}' ���s�b�� imagePathsExpression�A��ιw�]�� �L�C");
        }
        TImg.StartTransition(imgUrl, transitionType, dur);
    }

    private string GetDefaultExpression(string defaultEmoKey = "�L")
    {
        // ���w���L�ȡA��w�]��
        if (imagePathsExpression != null)
        {
            if (imagePathsExpression.ContainsKey("�L") && !string.IsNullOrEmpty(imagePathsExpression["�L"]))
            {
                defaultEmoKey = "�L";
            }
            else
            {
                foreach (var pair in imagePathsExpression)
                {
                    if (!string.IsNullOrEmpty(pair.Key) && !string.IsNullOrEmpty(pair.Value))
                    {
                        defaultEmoKey = pair.Key;
                        break;
                    }
                }
            }
        }
        return defaultEmoKey;
    }

    public void SetSimbol(string simbolName)
    {
        if (imagePathsSimbols.ContainsKey(simbolName))
        {
            Image simbolImage = Instantiate(SimbolPrefab, transform).GetComponent<Image>();
            simbolImage.rectTransform.localPosition = new Vector3(SimbolX, SimbolY, 0f);
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
