using UnityEngine;

public class Live2DRenderGrid : MonoBehaviour
{
    public static int live2DCount = 0; // �檬�ƦC�Ϊ�����
    public static int gridColumnCount = 4; // �@���X�� Live2D �ҫ�
    public static string modelLayerName = "Model"; // ��ʳ]�w�n�� Layer �W��

    /// <summary>
    /// �b�檬�ƥ����إߤ@�� Live2D ��V�椸�A���ͱM�� Camera �P RenderTexture
    /// </summary>
    public static RenderTexture SetupLive2DInGrid(GameObject live2DInstance, Vector2Int textureSize)
    {
        // ���o Layer �s��
        int modelLayer = LayerMask.NameToLayer(modelLayerName);
        if (modelLayer == -1)
        {
            Debug.LogError($"Layer \"{modelLayerName}\" �|���b Tags & Layers ���]�w�I");
            return null;
        }

        // �p��檬��m
        int col = live2DCount % gridColumnCount;
        int row = live2DCount / gridColumnCount;
        float unitSize = 10f; // �C�檺�@�ɳ��Z���A�̼ҫ���ҽվ�

        Vector3 live2DPos = new Vector3(col * unitSize, -row * unitSize, 0);
        live2DInstance.transform.position = live2DPos;
        SetLayerRecursively(live2DInstance, modelLayer);

        // �إ� RenderTexture
        RenderTexture rt = new RenderTexture(textureSize.x, textureSize.y, 24, RenderTextureFormat.ARGB32);
        rt.name = $"{live2DInstance.name}_RT";

        // �إ� Camera
        GameObject camGO = new GameObject($"{live2DInstance.name}_Camera");
        Camera cam = camGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0, 0, 0, 0); // ���z���I��
        cam.orthographic = true;
        cam.orthographicSize = unitSize / 2f;
        cam.cullingMask = 1 << modelLayer;
        cam.targetTexture = rt;
        cam.transform.position = live2DPos + new Vector3(0, 0, -10); // �q�����ݼҫ�

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
