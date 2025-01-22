using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharTImg : MonoBehaviour, IChar
{
    public TransitionImage TImg;
    public Image TImg0;
    public Image TImg1;
    private Dictionary<string, string> imagePathsExpression = new(); // 這個角色的表情立繪路徑表

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
    public int YAdd { get; set; } // Y 軸位移
    public string AssetID { get; set; } // 資產 ID

    public void Init(Dictionary<string, string> CharData, string CharEmo = "無")
    {
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

        // 設置 Scale 和 YAdd
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

        // 資產 ID
        AssetID = CharData.GetValueOrDefault("AssetID", string.Empty);
        // 表情相關屬性
        var assetRoot = PPM.Inst.Get("素材來源");
        var assetPath = PPM.Inst.Get("角色素材路徑");
        var resPath = assetRoot + "://" + assetPath;
        string emoTypes = PPM.Inst.Get("表情類型列表"); // "無,喜,怒,樂,驚,疑,暈"
        string[] emos = emoTypes.Split(",");
        // 組合每個表情的鍵值對
        foreach (string emo in emos)
        {
            string fileName = CharData.GetValueOrDefault(emo, string.Empty);
            if (!string.IsNullOrEmpty(fileName))
            {
                // ex: imagePathsExpression["怒"] = StreamingAssets://Image/AVG/Char/A-anger.png
                imagePathsExpression[emo] = resPath + AssetID + "-" + fileName + ".png";
            }
        }
        // 設定初始表情
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
        Debug.Log($"Character {UID} initialized with name {姓}{名}.");
    }

    public void Focus(float dur = 0.5f)
    {
        // 將物件移到所有同層物件的最前方
        transform.SetAsLastSibling();
        // 使用 DOTween 將 TImg0 和 TImg1 的顏色變為 RGB = 1
        if (TImg0 != null)
        {
            TImg0.DOColor(new Color(1f, 1f, 1f), dur).SetEase(Ease.Linear);
        }

        if (TImg1 != null)
        {
            TImg1.DOColor(new Color(1f, 1f, 1f), dur).SetEase(Ease.Linear);
        }
    }

    public void Unfocus(float dur = 0f)
    {
        // 使用 DOTween 將 TImg0 和 TImg1 的顏色變為 RGB = 0.3
        if (TImg0 != null)
        {
            TImg0.DOColor(new Color(0.3f, 0.3f, 0.3f), dur).SetEase(Ease.Linear);
        }

        if (TImg1 != null)
        {
            TImg1.DOColor(new Color(0.3f, 0.3f, 0.3f), dur).SetEase(Ease.Linear);
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
