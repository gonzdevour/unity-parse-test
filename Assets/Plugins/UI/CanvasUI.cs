using Unity.VisualScripting;
using UnityEngine;

public class CanvasUI : MonoBehaviour
{
    public GameObject cover;
    public PanelLoadingProgress panelLoadingProgress;
    public PanelSpinner panelSpinner;
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
}
