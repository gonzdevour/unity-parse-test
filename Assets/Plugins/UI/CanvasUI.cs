using UnityEngine;

public class CanvasUI : MonoBehaviour
{
    public GameObject cover;
    public PanelLoadingProgress panelLoadingProgress;
    public PanelSpinner panelSpinner;
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
            -boundHeight / 2 + height * (rect.pivot.y) + padding // Y �̤p�ȭץ�
        );

        Vector2 maxBounds = new Vector2(
            boundWidth / 2 - width * (1 - rect.pivot.x) - padding,  // X �̤j��
            boundHeight / 2 - height * (1 - rect.pivot.y) - padding // Y �̤j�ȭץ�
        );

        // ���� anchoredPosition�A�T�O UI �b�d�սd��
        Vector2 clampedPosition = rect.anchoredPosition;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minBounds.x, maxBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minBounds.y, maxBounds.y);

        // ��s UI ��m
        rect.anchoredPosition = clampedPosition;

        // Debug �T��
        Debug.Log($"[ClampToBounds] ��l��m: {rect.anchoredPosition}");
        Debug.Log($"[ClampToBounds] �ץ����m: {clampedPosition}");
    }

}
