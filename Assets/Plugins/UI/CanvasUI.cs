using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CanvasUI : MonoBehaviour
{
    [Header("黑屏")]
    public GameObject cover;
    [Header("讀取面板")]
    public PanelLoadingProgress panelLoadingProgress;
    public PanelSpinner panelSpinner;
    [Header("ModelUI")]
    public Vector2Int RenderTextureSize = new(1024, 1024);
    public Transform ModelParent;       // 指定模型的父物件
    public static string ModelLayerName = "Model"; // 手動設定好的 Layer 名稱
    private static int modelCellCount = 0; // 格狀排列用的索引
    private static int gridColumnCount = 4; // 一行放幾個 Live2D 模型

    // 單例模式
    public static CanvasUI Inst { get; private set; }

    private void Awake()
    {
        // 確保單例唯一性
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject); // 場景切換時保持不銷毀

        cover.SetActive(true);
        panelSpinner.gameObject.SetActive(false);
        panelLoadingProgress.gameObject.SetActive(false);
    }

    public void ClampToBounds(RectTransform rect, float padding = 0, RectTransform boundRect = null)
    {
        if (boundRect == null) boundRect = gameObject.GetComponent<RectTransform>(); // 預設為 Canvas

        // 獲取真實大小 (避免使用 sizeDelta)
        float width = rect.rect.width;
        float height = rect.rect.height;
        float boundWidth = boundRect.rect.width;
        float boundHeight = boundRect.rect.height;

        // 修正：考慮 Pivot 影響 (Y 軸)
        Vector2 minBounds = new Vector2(
            -boundWidth / 2 + width * rect.pivot.x + padding,  // X 最小值
            -boundHeight / 2 + height * rect.pivot.y + padding // Y 最小值修正
        );
        Vector2 maxBounds = new Vector2(
            boundWidth / 2 - width * (1 - rect.pivot.x) - padding,  // X 最大值
            boundHeight / 2 - height * (1 - rect.pivot.y) - padding // Y 最大值修正
        );

        // 儲存原始位置用於調試
        Vector2 originalPosition = rect.anchoredPosition;

        // 限制 anchoredPosition，確保 UI 在留白範圍內
        Vector2 clampedPosition = rect.anchoredPosition;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minBounds.x, maxBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minBounds.y, maxBounds.y);

        // Debug 訊息 - 修正前
        Debug.Log($"[ClampToBounds] 原始位置: {originalPosition}");
        Debug.Log($"[ClampToBounds] 邊界範圍: X({minBounds.x}~{maxBounds.x}) Y({minBounds.y}~{maxBounds.y})");

        // 更新 UI 位置
        rect.anchoredPosition = clampedPosition;

        // Debug 訊息 - 修正後
        Debug.Log($"[ClampToBounds] 修正後位置: {clampedPosition}");

        // 檢查是否有修正發生
        if (originalPosition != clampedPosition)
        {
            Debug.Log($"[ClampToBounds] 位置已被修正! X差異:{clampedPosition.x - originalPosition.x} Y差異:{clampedPosition.y - originalPosition.y}");
        }
    }

    public GameObject CreateModelToRawImage(string address, RawImage rawImage)
    {
        Debug.Log($"modePrefab address: {address}");
        GameObject modelPrefab = Resources.Load<GameObject>(address);
        if (modelPrefab == null)
        {
            Debug.Log("Prefab not found");
        }
        GameObject modelGO = Instantiate(modelPrefab, ModelParent);

        // 取得 Layer 編號
        int modelLayer = LayerMask.NameToLayer(ModelLayerName);
        if (modelLayer == -1)
        {
            Debug.LogError($"Layer \"{ModelLayerName}\" 尚未在 Tags & Layers 中設定！");
            return null;
        }

        // 計算格狀位置
        int col = modelCellCount % gridColumnCount;
        int row = modelCellCount / gridColumnCount;
        float unitSize = 10f;
        Vector3 modelPos = new(col * unitSize, -row * unitSize, 0);
        modelGO.transform.position = modelPos;

        // 將model與所有子物件都移到modelLayer
        modelGO.layer = modelLayer;
        foreach (Transform child in modelGO.transform) child.gameObject.layer = modelLayer;

        // 建立 RenderTexture
        RenderTexture rt = new(RenderTextureSize.x, RenderTextureSize.y, 24, RenderTextureFormat.ARGB32);
        rt.name = $"{modelGO.name}_RT";

        // 建立 Camera 並設為 modelGO 子物件
        GameObject camGO = new($"Camera_{modelGO.name}");
        camGO.transform.SetParent(modelGO.transform, false); // 設為子物件
        camGO.transform.localPosition = new Vector3(0, 0, -10); // 相對位置固定在前方

        Camera cam = camGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0, 0, 0, 0);
        cam.orthographic = true;
        cam.orthographicSize = unitSize / 2f;
        cam.cullingMask = 1 << modelLayer;
        cam.targetTexture = rt;

        modelCellCount++;

        rawImage.gameObject.name = Path.GetFileNameWithoutExtension(address);
        rawImage.texture = rt;

        return modelGO;
    }
}
