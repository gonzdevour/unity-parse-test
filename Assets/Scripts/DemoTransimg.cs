using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Demo_Transimg : MonoBehaviour
{
    public ToggleSelector EffectSelector;
    public Button Button_Switch; // 指定的按鈕
    public TransitionImage TImg; // 假設 TImg 是一個負責處理圖片轉換的類

    private Dictionary<string, string> imagePaths = new Dictionary<string, string>(); // 儲存圖片資源路徑
    private List<string> pathKeys; // 用於按順序存取路徑的鍵集合
    private int currentIndex = 0; // 當前圖片索引
    private string resPathBg = "StreamingAssets://Mods/Official/Images/AVG/Backgrounds/";

    void Start()
    {
        // 初始化圖片資源路徑
        imagePaths["Image1"] = resPathBg + "AChos001_19201080.jpg";
        imagePaths["Image2"] = resPathBg + "130machi_19201080.jpg";
        imagePaths["Image3"] = resPathBg + "rtshoppingstreet01_19201080.jpg";

        pathKeys = new List<string>(imagePaths.Keys);

        // 確保按鈕已分配，並綁定點擊事件
        if (Button_Switch != null)
        {
            Button_Switch.onClick.AddListener(OnButtonSwitch);
        }
    }

    private void OnButtonSwitch()
    {
        if (TImg != null && pathKeys.Count > 0)
        {
            // 重設透明度(以免被fade的結果影響其他effect)
            TImg.ResetAlpha();
            // 取得當前索引的圖片路徑
            string currentPath = imagePaths[pathKeys[currentIndex]];
            string effectType = EffectSelector.SelectedToggleNamesString;
            TImg.StartTransition(currentPath, effectType, 2f);

            // 更新索引以指向下一張圖片，並循環
            currentIndex = (currentIndex + 1) % pathKeys.Count;
        }
    }
}
