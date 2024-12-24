using UnityEngine;
using UnityEngine.UI;

public class BtnToScene : MonoBehaviour
{
    public Button button;
    public string targetScene; // 指定要切換的場景名稱

    void Start()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        button.interactable = false; // 禁用按鈕
        var sceneSwitcher = SceneSwitcher.Inst;
        if (sceneSwitcher != null)
        {
            sceneSwitcher.SwitchScene(targetScene);
        }
    }
}
