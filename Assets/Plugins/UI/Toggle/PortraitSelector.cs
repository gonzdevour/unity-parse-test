using UnityEngine;
using UnityEngine.UI;

public class PortraitManager : MonoBehaviour
{
    public Button portraitSelector; // PortraitSelector 按鈕
    public GameObject panel; // Panel（包含 Toggles 和 ToggleGroup）
    public ToggleGroup toggleGroup; // Panel 上的 ToggleGroup

    private const string PORTRAIT_PREF_KEY = "Portrait";

    void Start()
    {
        // 初始化 Panel 為 inactive
        panel.SetActive(false);

        // 檢查 PlayerPrefs 是否有保存 Portrait 值
        string savedPortrait = PlayerPrefs.GetString(PORTRAIT_PREF_KEY, null);

        if (!string.IsNullOrEmpty(savedPortrait))
        {
            // 如果有保存 Portrait，找到對應的 Toggle，設為 isOn
            foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
            {
                Image backgroundImage = toggle.transform.Find("Background").GetComponent<Image>();
                if (backgroundImage != null && backgroundImage.sprite.name == savedPortrait)
                {
                    // 更新 PortraitSelector 的圖片
                    UpdatePortraitSelectorImage(backgroundImage.sprite);
                    toggle.isOn = true;
                    break;
                }
            }
        }
        else
        {
            // 如果沒有保存 Portrait，使用第一個 Toggle 作為預設值
            Toggle defaultToggle = toggleGroup.GetFirstActiveToggle();
            if (defaultToggle != null)
            {
                Image backgroundImage = defaultToggle.transform.Find("Background").GetComponent<Image>();
                if (backgroundImage != null)
                {
                    PlayerPrefs.SetString(PORTRAIT_PREF_KEY, backgroundImage.sprite.name);
                    PlayerPrefs.Save();

                    // 更新 PortraitSelector 的圖片
                    UpdatePortraitSelectorImage(backgroundImage.sprite);
                }
            }
        }

        // 設定 PortraitSelector 按鈕的點擊事件
        portraitSelector.onClick.AddListener(() => panel.SetActive(true));

        // 設定每個 Toggle 的點擊事件
        foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
        {
            toggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    OnToggleSelected(toggle);
                }
            });
        }

        // 設定 Panel 的點擊事件
        Button panelButton = panel.GetComponent<Button>();
        if (panelButton != null)
        {
            panelButton.onClick.AddListener(() => panel.SetActive(false));
        }
    }

    private void OnToggleSelected(Toggle selectedToggle)
    {
        // 獲取選中 Toggle 的背景圖片名稱
        Image backgroundImage = selectedToggle.transform.Find("Background").GetComponent<Image>();
        if (backgroundImage != null)
        {
            string portraitName = backgroundImage.sprite.name;

            // 更新 PlayerPrefs
            PlayerPrefs.SetString(PORTRAIT_PREF_KEY, portraitName);
            PlayerPrefs.Save();

            // 更新 PortraitSelector 的圖片
            UpdatePortraitSelectorImage(backgroundImage.sprite);

            // 隱藏 Panel
            panel.SetActive(false);
        }
    }

    private void UpdatePortraitSelectorImage(Sprite newSprite)
    {
        Image selectorImage = portraitSelector.GetComponent<Image>();
        if (selectorImage != null)
        {
            selectorImage.sprite = newSprite;
        }
    }
}
