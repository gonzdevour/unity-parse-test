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
        if (boundRect == null) boundRect = gameObject.GetComponent<RectTransform>(); // boundRect �w�]�� Canvas

        // ���o��l anchoredPosition
        Vector2 originalPosition = rect.anchoredPosition;
        Debug.Log($"[ClampToBounds] ��l��m: {originalPosition}");

        // �p��̤p��� (�T�O UI ���U�����W�X�e��)
        Vector2 minBounds = new Vector2(
            rect.sizeDelta.x / 2 + padding,
            rect.sizeDelta.y / 2 + padding
        );

        // �p��̤j��� (�T�O UI �k�W�����W�X�e��)
        Vector2 maxBounds = new Vector2(
            boundRect.sizeDelta.x - rect.sizeDelta.x / 2 - padding,
            boundRect.sizeDelta.y - rect.sizeDelta.y / 2 - padding
        );

        // ���� anchoredPosition�A�T�O UI �b�d�սd��
        Vector2 clampedPosition = rect.anchoredPosition;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minBounds.x, maxBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minBounds.y, maxBounds.y);

        // ��s UI ��m
        rect.anchoredPosition = clampedPosition;

        // ���o�ץ��᪺ anchoredPosition
        Debug.Log($"[ClampToBounds] �ץ����m: {clampedPosition}");
    }

}
