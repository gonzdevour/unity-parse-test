using Unity.VisualScripting;
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
}
