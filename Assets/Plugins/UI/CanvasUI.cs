using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CanvasUI : MonoBehaviour
{
    [Header("�«�")]
    public GameObject cover;
    [Header("Ū�����O")]
    public PanelLoadingProgress panelLoadingProgress;
    public PanelSpinner panelSpinner;
    [Header("ModelUI")]
    public Vector2Int RenderTextureSize = new(1024, 1024);
    public Transform ModelParent;       // ���w�ҫ���������
    public static string ModelLayerName = "Model"; // ��ʳ]�w�n�� Layer �W��
    private static int modelCellCount = 0; // �檬�ƦC�Ϊ�����
    private static int gridColumnCount = 4; // �@���X�� Live2D �ҫ�

    // ��ҼҦ�
    public static CanvasUI Inst { get; private set; }

    private void Awake()
    {
        // �T�O��Ұߤ@��
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject); // ���������ɫO�����P��

        cover.SetActive(true);
        panelSpinner.gameObject.SetActive(false);
        panelLoadingProgress.gameObject.SetActive(false);
    }

    public void ClampToBounds(RectTransform rect, float padding = 0, RectTransform boundRect = null)
    {
        if (boundRect == null) boundRect = gameObject.GetComponent<RectTransform>(); // �w�]�� Canvas

        // ����u��j�p (�קK�ϥ� sizeDelta)
        float width = rect.rect.width;
        float height = rect.rect.height;
        float boundWidth = boundRect.rect.width;
        float boundHeight = boundRect.rect.height;

        // �ץ��G�Ҽ{ Pivot �v�T (Y �b)
        Vector2 minBounds = new Vector2(
            -boundWidth / 2 + width * rect.pivot.x + padding,  // X �̤p��
            -boundHeight / 2 + height * rect.pivot.y + padding // Y �̤p�ȭץ�
        );
        Vector2 maxBounds = new Vector2(
            boundWidth / 2 - width * (1 - rect.pivot.x) - padding,  // X �̤j��
            boundHeight / 2 - height * (1 - rect.pivot.y) - padding // Y �̤j�ȭץ�
        );

        // �x�s��l��m�Ω�ո�
        Vector2 originalPosition = rect.anchoredPosition;

        // ���� anchoredPosition�A�T�O UI �b�d�սd��
        Vector2 clampedPosition = rect.anchoredPosition;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minBounds.x, maxBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minBounds.y, maxBounds.y);

        // Debug �T�� - �ץ��e
        Debug.Log($"[ClampToBounds] ��l��m: {originalPosition}");
        Debug.Log($"[ClampToBounds] ��ɽd��: X({minBounds.x}~{maxBounds.x}) Y({minBounds.y}~{maxBounds.y})");

        // ��s UI ��m
        rect.anchoredPosition = clampedPosition;

        // Debug �T�� - �ץ���
        Debug.Log($"[ClampToBounds] �ץ����m: {clampedPosition}");

        // �ˬd�O�_���ץ��o��
        if (originalPosition != clampedPosition)
        {
            Debug.Log($"[ClampToBounds] ��m�w�Q�ץ�! X�t��:{clampedPosition.x - originalPosition.x} Y�t��:{clampedPosition.y - originalPosition.y}");
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

        // ���o Layer �s��
        int modelLayer = LayerMask.NameToLayer(ModelLayerName);
        if (modelLayer == -1)
        {
            Debug.LogError($"Layer \"{ModelLayerName}\" �|���b Tags & Layers ���]�w�I");
            return null;
        }

        // �p��檬��m
        int col = modelCellCount % gridColumnCount;
        int row = modelCellCount / gridColumnCount;
        float unitSize = 10f;
        Vector3 modelPos = new(col * unitSize, -row * unitSize, 0);
        modelGO.transform.position = modelPos;

        // �Nmodel�P�Ҧ��l���󳣲���modelLayer
        modelGO.layer = modelLayer;
        foreach (Transform child in modelGO.transform) child.gameObject.layer = modelLayer;

        // �إ� RenderTexture
        RenderTexture rt = new(RenderTextureSize.x, RenderTextureSize.y, 24, RenderTextureFormat.ARGB32);
        rt.name = $"{modelGO.name}_RT";

        // �إ� Camera �ó]�� modelGO �l����
        GameObject camGO = new($"Camera_{modelGO.name}");
        camGO.transform.SetParent(modelGO.transform, false); // �]���l����
        camGO.transform.localPosition = new Vector3(0, 0, -10); // �۹��m�T�w�b�e��

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
