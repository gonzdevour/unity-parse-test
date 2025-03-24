using UnityEngine;

public class Live2DRenderGrid : MonoBehaviour
{
    public static int live2DCount = 0; // 格狀排列用的索引
    public static int gridColumnCount = 4; // 一行放幾個 Live2D 模型
    public static string modelLayerName = "Model"; // 手動設定好的 Layer 名稱

    /// <summary>
    /// 在格狀排布中建立一個 Live2D 渲染單元，產生專屬 Camera 與 RenderTexture
    /// </summary>
    public static RenderTexture SetupLive2DInGrid(GameObject live2DInstance, Vector2Int textureSize)
    {
        // 取得 Layer 編號
        int modelLayer = LayerMask.NameToLayer(modelLayerName);
        if (modelLayer == -1)
        {
            Debug.LogError($"Layer \"{modelLayerName}\" 尚未在 Tags & Layers 中設定！");
            return null;
        }

        // 計算格狀位置
        int col = live2DCount % gridColumnCount;
        int row = live2DCount / gridColumnCount;
        float unitSize = 10f; // 每格的世界單位距離，依模型比例調整

        Vector3 live2DPos = new Vector3(col * unitSize, -row * unitSize, 0);
        live2DInstance.transform.position = live2DPos;
        SetLayerRecursively(live2DInstance, modelLayer);

        // 建立 RenderTexture
        RenderTexture rt = new RenderTexture(textureSize.x, textureSize.y, 24, RenderTextureFormat.ARGB32);
        rt.name = $"{live2DInstance.name}_RT";

        // 建立 Camera
        GameObject camGO = new GameObject($"{live2DInstance.name}_Camera");
        Camera cam = camGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0, 0, 0, 0); // 全透明背景
        cam.orthographic = true;
        cam.orthographicSize = unitSize / 2f;
        cam.cullingMask = 1 << modelLayer;
        cam.targetTexture = rt;
        cam.transform.position = live2DPos + new Vector3(0, 0, -10); // 從正面看模型

        live2DCount++;
        return rt;
    }

    private static void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}
