using UnityEngine;
using UnityEngine.UI;

public class Live2DUIExample : MonoBehaviour
{
    public Transform uiParent;       // ���w RawImage �� UI ������]�Ҧp�@�ӪŪ� GameObject �b Canvas �U�^

    public Vector2Int renderTextureSize = new(1024, 1024);
    public Vector2 rawImageSize = new(200, 200); // UI �W RawImage �e��

    private void Start()
    {
        CreateTwoLive2DToUI();
    }

    public void CreateTwoLive2DToUI()
    {
        // ���] prefab ���|���GAssets/Resources/Live2D/ModelA.prefab
        GameObject prefabA = Resources.Load<GameObject>("Models/Live2D/hibiki/runtime/hibiki");
        //GameObject prefabB = Resources.Load<GameObject>("Live2D/ModelB");

        // ���͹�ҨóB�z
        GameObject instanceA = Instantiate(prefabA);
        RenderTexture rtA = Live2DRenderGrid.SetupLive2DInGrid(instanceA, renderTextureSize);
        CreateRawImageUI(rtA, "Live2D_UI_A");

        //GameObject instanceB = Instantiate(prefabB);
        //RenderTexture rtB = Live2DRenderGrid.SetupLive2DInGrid(instanceB, renderTextureSize);
        //CreateRawImageUI(rtB, "Live2D_UI_B");
    }


    private void CreateRawImageUI(RenderTexture rt, string name)
    {
        GameObject rawGO = new GameObject(name, typeof(RectTransform), typeof(RawImage));
        rawGO.transform.SetParent(uiParent, false);

        RawImage rawImage = rawGO.GetComponent<RawImage>();
        rawImage.texture = rt;

        RectTransform rtTransform = rawGO.GetComponent<RectTransform>();
        rtTransform.sizeDelta = rawImageSize;
    }
}
