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

    private void OnDestroy()
    {
        // 停止與該物件相關的所有 Tween，避免存取已刪除物件
        DOTween.Kill(gameObject);
        // 移除對應的ModelGO，連同子物件的camera一起刪除
        Destroy(Model);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void Init(Dictionary<string, string> CharData, string CharEmo = "無", string CharSimbol = "")
    {
        // 取出rawImage
        rawImage = gameObject.GetComponentInChildren<RawImage>();
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

        // 設定初始表情

        // 記錄符號預設位置(以免被偏移值影響)
        SimbolMarkerOriginPos = new Vector2(SimbolMarker.localPosition.x, SimbolMarker.localPosition.y);
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
        rawImage.DOColor(new Color(1f, 1f, 1f), dur);
    }

    public void Unfocus(float dur = 0f)
    {
        rawImage.DOColor(new Color(0.3f, 0.3f, 0.3f), dur);
    }

    public void SetExpression(string expression = "無", string transitionType = "slideup", float dur = 1f ) 
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
