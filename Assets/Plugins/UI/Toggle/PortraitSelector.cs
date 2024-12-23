using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;


public class PortraitSelector : MonoBehaviour
{
    public ItemsGenFromDB itemsGenFromDB;

    public Button portraitSelector; // PortraitSelector ���s
    public GameObject panel; // Panel�]�]�t Toggles �M ToggleGroup�^
    public ToggleGroup toggleGroup; // Panel �W�� ToggleGroup

    private readonly string PORTRAIT_URL_PREF_KEY = "PortraitUrl";
    private readonly string PORTRAIT_RESOURCES_PATH = "Resources://Sprites/KennyArt/square" + "|"; //multiple sprite

    void Start()
    {
        // ��l�� Panel �� inactive
        panel.SetActive(false);

        // �]�w PortraitSelector ���s���I���ƥ�
        portraitSelector.onClick.AddListener(() =>
        {
            //Debug.Log("portraitSelector on clicked");
            panel.SetActive(true);
        });

        // Panel�Yclick outside�h����
        Button panelButton = panel.GetComponent<Button>();
        if (panelButton != null)
        {
            panelButton.onClick.AddListener(() => panel.SetActive(false));
        }
        // ���oPortrait�Ϥ��A���w�]��Toggles���UisOn�ƥ�
        SelectorInit();

        // �qcsv��ƫإ�toggle��
        //StartCoroutine(itemsGenFromDB.MakeItemTable());
    }

    private void SelectorInit()
    {
        UpdateToggleLabelNameForInit();
        // �ˬd PlayerPrefs �O�_���O�s Portrait ��
        string savedPortraitUrl = PlayerPrefs.GetString(PORTRAIT_URL_PREF_KEY, null);

        if (!string.IsNullOrEmpty(savedPortraitUrl))
        {
            Toggle matchedToggle = GetMatchedToggle(savedPortraitUrl);
            //�s�b�W�ٹ�����url��Toggle
            if (matchedToggle != null) 
            {
                Image backgroundImage = matchedToggle.transform.Find("Background").GetComponent<Image>();
                UpdatePortraitSelectorImage(backgroundImage.sprite);
            }
            //�S���W�ٹ�����url��Toggle�A�i��O�s�����Ϥ��Ӧ�selector�H�~�����|
            else
            {
                StartCoroutine(UpdatePortraitSelectorImageFromUrl(savedPortraitUrl));
            }
            //��sisOn
            UpdateToggleIsOn(savedPortraitUrl);
        }
        else
        {
            // �p�G�S���O�s Portrait�A�ϥβĤ@�� Toggle �@���w�]��
            Toggle defaultToggle = toggleGroup.GetFirstActiveToggle();
            if (defaultToggle != null)
            {
                Image backgroundImage = defaultToggle.transform.Find("Background").GetComponent<Image>();
                if (backgroundImage != null)
                {
                    string portraitName = backgroundImage.sprite.name;
                    // ex: "Resources://Sprites/KennyArt/square" + "|" + "penguin" //multiple sprite����penguin
                    PlayerPrefs.SetString(PORTRAIT_URL_PREF_KEY, PORTRAIT_RESOURCES_PATH + portraitName);
                    PlayerPrefs.Save();

                    // ��s PortraitSelector ���Ϥ�
                    UpdatePortraitSelectorImage(backgroundImage.sprite);
                }
            }
        }
        // �]�w�C�� Toggle ���I���ƥ�
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

        // �������� Toggle�A�]�� isOn
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
        yield return new WaitForSeconds(0f); //����SpriteCacher�bGlobal�Q��l��
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
        // ����襤 Toggle ���I���Ϥ��W��
        Image backgroundImage = selectedToggle.transform.Find("Background").GetComponent<Image>();
        if (backgroundImage != null)
        {
            string portraitName = backgroundImage.sprite.name;
            if (string.IsNullOrEmpty(imgUrl))
            {
                // ex: "Resources://Sprites/KennyArt/square" + "|" + "penguin" //multiple sprite����penguin
                PlayerPrefs.SetString(PORTRAIT_URL_PREF_KEY, PORTRAIT_RESOURCES_PATH + portraitName);
                PlayerPrefs.Save();
                Debug.Log($"�x�s{PORTRAIT_URL_PREF_KEY}��{PORTRAIT_RESOURCES_PATH + portraitName}");
                // ��s PortraitSelector ���Ϥ�
                UpdatePortraitSelectorImage(backgroundImage.sprite);
            }
            else 
            {
                PlayerPrefs.SetString(PORTRAIT_URL_PREF_KEY, imgUrl);
                PlayerPrefs.Save();
                Debug.Log($"�x�s{PORTRAIT_URL_PREF_KEY}��{imgUrl}");
                StartCoroutine(UpdatePortraitSelectorImageFromUrl(imgUrl));
            }

            // ���� Panel
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

        // �ˬd�O�_�� '|' �Ÿ�
        int lastPipeIndex = url.LastIndexOf('|');
        if (lastPipeIndex >= 0)
        {
            // �Y�� '|'�A�����q���}�l����
            string fileName = url.Substring(lastPipeIndex + 1);
            return Path.GetFileNameWithoutExtension(fileName);
        }

        // �Y�L '|'�A�h�ˬd�̫�@�� '/'
        int lastSlashIndex = url.LastIndexOf('/');
        if (lastSlashIndex >= 0)
        {
            string fileName = url.Substring(lastSlashIndex + 1);
            return Path.GetFileNameWithoutExtension(fileName);
        }

        // �Y�L���j�šA������url��^
        return url;
    }
}
