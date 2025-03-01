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
    private Dictionary<string, string> imagePathsExpression = new(); // 這個角色的表情立繪路徑表
    private Dictionary<string, string> imagePathsSimbols;

    public string UID { get; set; } // 唯一識別碼
    public string 姓 { get; set; } // 姓
    public string 名 { get; set; } // 名
    public string 敬稱 { get; set; } // 敬稱 (如：先生/小姐)
    public string 職稱 { get; set; } // 職稱
    public string 暱稱1 { get; set; } // 暱稱1
    public string 暱稱2 { get; set; } // 暱稱2
    public string 立繪 { get; set; } // 暱稱2
    public string 頭圖 { get; set; } // 暱稱2
    public float Scale { get; set; } // 縮放比例
    public float YAdd { get; set; } // Y 軸位移
    public float SimbolX { get; set; } // simbol X偏移
    public float SimbolY { get; set; } // simbol Y偏移
    public string AssetID { get; set; } // 資產 ID
    public string Expression { get; set; } //目前的表情

    public void Init(Dictionary<string, string> CharData, string CharEmo = "無", string CharSimbol = "")
    {
        // 設定符號表
        imagePathsSimbols = Director.Inst.imagePathsSimbols;

        Debug.Log($"初始化角色資料：{CharData["UID"]}");
        // 設置屬性
        UID = CharData.GetValueOrDefault("UID", string.Empty);
        姓 = CharData.GetValueOrDefault("姓", string.Empty);
        名 = CharData.GetValueOrDefault("名", string.Empty);
        敬稱 = CharData.GetValueOrDefault("敬稱", string.Empty);
        職稱 = CharData.GetValueOrDefault("職稱", string.Empty);
        暱稱1 = CharData.GetValueOrDefault("暱稱1", string.Empty);
        暱稱2 = CharData.GetValueOrDefault("暱稱2", string.Empty);
        立繪 = CharData.GetValueOrDefault("立繪", string.Empty);
        頭圖 = CharData.GetValueOrDefault("頭圖", string.Empty);

        // 設置 Scale, YAdd, SimbolX, SimbolY
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

        // 取得此角色的所有表情路徑
        imagePathsExpression = Director.Inst.GetPaths_CharExpressions(CharData);

        // 設定初始表情
        Expression = CharEmo;
        if (imagePathsExpression != null)
        {
            // 表情列表內有值
            if (imagePathsExpression.ContainsKey(CharEmo))
            {
                // 指定表情有值
                SpriteCacher.Inst.GetSprite(imagePathsExpression[CharEmo], (sprite) =>
                {
                    TImg0.sprite = sprite;
                    TImg0.SetNativeSize();
                });
            }
            else
            {
                // 指定表情無值，尋找預設表情
                string defaultEmoKey = GetDefaultExpression("無");
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
            // 表情列表內無值
            Debug.Log($"{UID} has no expressions");
        }

        // 產生符號
        if (!string.IsNullOrEmpty(CharSimbol))
        {
            SetSimbol(CharSimbol); // 設定符號
            Debug.Log($"{UID}產生符號：{CharSimbol}");
        }

        Debug.Log($"Character {UID} initialized with name {姓}{名}.");
    }

    public void Focus(float dur = 0f)
    {
        // 將物件移到所有同層物件的最前方
        transform.SetAsLastSibling();

        //// 直接將 TImg0 和 TImg1 的顏色變為 RGB = 1
        //TImg0.color = new Color(1f, 1f, 1f);
        //TImg1.color = new Color(1f, 1f, 1f);

        // 使用 DOTween 將 TImg0 和 TImg1 的顏色變為 RGB = 1
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
        //// 直接將 TImg0 和 TImg1 的顏色變為 RGB = 0.3
        //TImg0.color = new Color(0.3f, 0.3f, 0.3f);
        //TImg1.color = new Color(0.3f, 0.3f, 0.3f);

        // 使用 DOTween 將 TImg0 和 TImg1 的顏色變為 RGB = 0.3
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

    public void SetExpression(string expression = "無", string transitionType = "slideup", float dur = 1f ) 
    {
        if (string.IsNullOrEmpty(expression)) expression = GetDefaultExpression("無");
        Debug.Log("表情轉換特效：" + transitionType);
        string imgUrl = imagePathsExpression["無"]; ;
        if (imagePathsExpression.ContainsKey(expression))
        {
            imgUrl = imagePathsExpression[expression];
        }
        else
        {
            Debug.LogWarning($"Key '{expression}' 不存在於 imagePathsExpression，改用預設值 無。");
        }
        TImg.StartTransition(imgUrl, transitionType, dur);
    }

    private string GetDefaultExpression(string defaultEmoKey = "無")
    {
        // 指定表情無值，找預設值
        if (imagePathsExpression != null)
        {
            if (imagePathsExpression.ContainsKey("無") && !string.IsNullOrEmpty(imagePathsExpression["無"]))
            {
                defaultEmoKey = "無";
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

        // 設置初始位置
        var rect = GetComponent<RectTransform>();
        rect.anchoredPosition = fromPoint;

        // 起終點不同時，使用 DOTween 進行移動
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

        // 設置初始位置
        var rect = GetComponent<RectTransform>();
        Vector2 currentPos = rect.anchoredPosition;
        rect.anchoredPosition = new Vector2(fromPoint.x, currentPos.y); // 只更新 X 軸

        // 起終點不同時，使用 DOTween 進行移動
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

        // 設置初始位置
        var rect = GetComponent<RectTransform>();
        Vector2 currentPos = rect.anchoredPosition;
        rect.anchoredPosition = new Vector2(currentPos.x, fromPoint.y); // 只更新 Y 軸

        // 起終點不同時，使用 DOTween 進行移動
        if (fromPoint.y != toPoint.y)
        {
            rect.DOAnchorPosY(toPoint.y, dur).SetEase(Ease.Linear).OnComplete(() =>
            {
                Debug.Log($"Character {UID} moved on Y-axis from {fromPoint.y} to {toPoint.y} in {dur} seconds.");
            });
        }
    }
}
