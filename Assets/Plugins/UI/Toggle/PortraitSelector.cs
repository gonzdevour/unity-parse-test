using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;


public class PortraitSelector : MonoBehaviour
{
    public ItemsGenFromDB itemsGenFromDB;

    public Button portraitSelector; // PortraitSelector 按鈕
    public GameObject panel; // Panel（包含 Toggles 和 ToggleGroup）
    public ToggleGroup toggleGroup; // Panel 上的 ToggleGroup

    private readonly string PORTRAIT_URL_PREF_KEY = "PortraitUrl";
    private readonly string PORTRAIT_RESOURCES_PATH = "Resources://Sprites/KennyArt/square" + "|"; //multiple sprite

    void Start()
    {
        // 初始化 Panel 為 inactive
        panel.SetActive(false);

        // 設定 PortraitSelector 按鈕的點擊事件
        portraitSelector.onClick.AddListener(() =>
        {
            //Debug.Log("portraitSelector on clicked");
            panel.SetActive(true);
        });

        // Panel若click outside則隱藏
        Button panelButton = panel.GetComponent<Button>();
        if (panelButton != null)
        {
            panelButton.onClick.AddListener(() => panel.SetActive(false));
        }
        // 取得Portrait圖片，幫預設的Toggles註冊isOn事件
        SelectorInit();

        // 從csv資料建立toggle表
        //StartCoroutine(itemsGenFromDB.MakeItemTable());
    }

    private void SelectorInit()
    {
        UpdateToggleLabelNameForInit();
        // 檢查 PlayerPrefs 是否有保存 Portrait 值
        string savedPortraitUrl = PlayerPrefs.GetString(PORTRAIT_URL_PREF_KEY, null);

        if (!string.IsNullOrEmpty(savedPortraitUrl))
        {
            Toggle matchedToggle = GetMatchedToggle(savedPortraitUrl);
            //存在名稱對應於url的Toggle
            if (matchedToggle != null) 
            {
                Image backgroundImage = matchedToggle.transform.Find("Background").GetComponent<Image>();
                UpdatePortraitSelectorImage(backgroundImage.sprite);
            }
            //沒有名稱對應於url的Toggle，可能保存的的圖片來自selector以外的路徑
            else
            {
                StartCoroutine(UpdatePortraitSelectorImageFromUrl(savedPortraitUrl));
            }
            //更新isOn
            UpdateToggleIsOn(savedPortraitUrl);
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
                    string portraitName = backgroundImage.sprite.name;
                    // ex: "Resources://Sprites/KennyArt/square" + "|" + "penguin" //multiple sprite中的penguin
                    PlayerPrefs.SetString(PORTRAIT_URL_PREF_KEY, PORTRAIT_RESOURCES_PATH + portraitName);
                    PlayerPrefs.Save();

                    // 更新 PortraitSelector 的圖片
                    UpdatePortraitSelectorImage(backgroundImage.sprite);
                }
            }
        }
        // 設定每個 Toggle 的點擊事件
        foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
        {
            RegisterToggleEvent(toggle);
        }
    }

    private Toggle GetMatchedToggle(string url) 
    {
        string portaitSpriteName = GetNameFromUrl(url);
        Toggle matchedToggle = null;
        foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
        {
            Image backgroundImage = toggle.transform.Find("Background").GetComponent<Image>();
            string togglelabelName = toggle.GetComponent<ToggleVariables>().LabelName;
            if (backgroundImage != null && togglelabelName == portaitSpriteName)
            {
                matchedToggle = toggle;
                break;
            }
        }
        return matchedToggle;
    }

    public void UpdateToggleLabelNameForInit() 
    {
        foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
        {
            ToggleVariables toggleVar = toggle.GetComponent<ToggleVariables>();
            Image backgroundImage = toggle.transform.Find("Background").GetComponent<Image>();
            string spriteName = backgroundImage.sprite.name;
            if (!string.IsNullOrEmpty(spriteName))
            {
                toggleVar.SetLabelName(spriteName);
            }
        }
    }

    public void UpdateToggleIsOn(string PortraitUrl = "") 
    {
        if (string.IsNullOrEmpty(PortraitUrl))
        {
            PortraitUrl = PlayerPrefs.GetString(PORTRAIT_URL_PREF_KEY, null);
        }
        string portaitSpriteName = GetNameFromUrl(PortraitUrl);
        Debug.Log($"portaitSpriteName:{portaitSpriteName}");

        // 找到對應的 Toggle，設為 isOn
        foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
        {
            Image backgroundImage = toggle.transform.Find("Background").GetComponent<Image>();
            string togglelabelName = toggle.GetComponent<ToggleVariables>().LabelName;
            //Debug.Log($"{togglelabelName} == {portaitSpriteName}");
            if (togglelabelName == portaitSpriteName)
            {
                Debug.Log($"MATCHED!! {togglelabelName} == {portaitSpriteName}");
                toggle.isOn = true;
            }
            else 
            {
                toggle.isOn = false;
            }
        }
    }

    private IEnumerator UpdatePortraitSelectorImageFromUrl(string url)
    {
        yield return new WaitForSeconds(0f); //等待SpriteCacher在Global被初始化
        SpriteCacher spriteCacher = FindObjectOfType<SpriteCacher>();
        spriteCacher.GetSprite(url, (sprite) =>
        {
            UpdatePortraitSelectorImage(sprite);
        });
    }

    public void RegisterToggleEvent(Toggle toggle, string imgUrl = "")
    {
        toggle.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                OnToggleSelected(toggle, imgUrl);
            }
        });
    }

    private void OnToggleSelected(Toggle selectedToggle, string imgUrl = "")
    {
        // 獲取選中 Toggle 的背景圖片名稱
        Image backgroundImage = selectedToggle.transform.Find("Background").GetComponent<Image>();
        if (backgroundImage != null)
        {
            string portraitName = backgroundImage.sprite.name;
            if (string.IsNullOrEmpty(imgUrl))
            {
                // ex: "Resources://Sprites/KennyArt/square" + "|" + "penguin" //multiple sprite中的penguin
                PlayerPrefs.SetString(PORTRAIT_URL_PREF_KEY, PORTRAIT_RESOURCES_PATH + portraitName);
                PlayerPrefs.Save();
                Debug.Log($"儲存{PORTRAIT_URL_PREF_KEY}為{PORTRAIT_RESOURCES_PATH + portraitName}");
                // 更新 PortraitSelector 的圖片
                UpdatePortraitSelectorImage(backgroundImage.sprite);
            }
            else 
            {
                PlayerPrefs.SetString(PORTRAIT_URL_PREF_KEY, imgUrl);
                PlayerPrefs.Save();
                Debug.Log($"儲存{PORTRAIT_URL_PREF_KEY}為{imgUrl}");
                StartCoroutine(UpdatePortraitSelectorImageFromUrl(imgUrl));
            }

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

    public string GetNameFromUrl(string url)
    {
        if (string.IsNullOrEmpty(url)) return "";

        // 檢查是否有 '|' 符號
        int lastPipeIndex = url.LastIndexOf('|');
        if (lastPipeIndex >= 0)
        {
            // 若有 '|'，直接從其後開始提取
            string fileName = url.Substring(lastPipeIndex + 1);
            return Path.GetFileNameWithoutExtension(fileName);
        }

        // 若無 '|'，則檢查最後一個 '/'
        int lastSlashIndex = url.LastIndexOf('/');
        if (lastSlashIndex >= 0)
        {
            string fileName = url.Substring(lastSlashIndex + 1);
            return Path.GetFileNameWithoutExtension(fileName);
        }

        // 若無分隔符，直接原url返回
        return url;
    }
}
