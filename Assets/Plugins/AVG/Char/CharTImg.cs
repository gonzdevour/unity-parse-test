using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharTImg : MonoBehaviour, IChar
{
    public Image TImg0;
    public Image TImg1;

    public string UID { get; set; } // 唯一識別碼
    public string 姓 { get; set; } // 姓
    public string 名 { get; set; } // 名
    public string 敬稱 { get; set; } // 敬稱 (如：先生/小姐)
    public string 職稱 { get; set; } // 職稱
    public string 暱稱1 { get; set; } // 暱稱1
    public string 暱稱2 { get; set; } // 暱稱2
    public float Scale { get; set; } // 縮放比例
    public int YAdd { get; set; } // Y 軸位移
    public string AssetID { get; set; } // 資產 ID

    // 表情相關屬性
    public string 無 { get; set; } // 無表情
    public string 喜 { get; set; } // 喜
    public string 怒 { get; set; } // 怒
    public string 樂 { get; set; } // 樂
    public string 驚 { get; set; } // 驚
    public string 疑 { get; set; } // 疑
    public string 暈 { get; set; } // 暈

    public void Init(Dictionary<string, string> CharData)
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
        Debug.Log($"初始化角色資料：{UID}");

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
        無 = CharData.GetValueOrDefault("無", string.Empty);
        喜 = CharData.GetValueOrDefault("喜", string.Empty);
        怒 = CharData.GetValueOrDefault("怒", string.Empty);
        樂 = CharData.GetValueOrDefault("樂", string.Empty);
        驚 = CharData.GetValueOrDefault("驚", string.Empty);
        疑 = CharData.GetValueOrDefault("疑", string.Empty);
        暈 = CharData.GetValueOrDefault("暈", string.Empty);

        Debug.Log($"Character {UID} initialized with name {姓}{名}.");
    }

    public void Focus()
    {
        // 將物件移到所有同層物件的最前方
        transform.SetAsLastSibling();
        // 使用 DOTween 將 TImg0 和 TImg1 的顏色變為 RGB = 1
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
        // 使用 DOTween 將 TImg0 和 TImg1 的顏色變為 RGB = 0.3
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
