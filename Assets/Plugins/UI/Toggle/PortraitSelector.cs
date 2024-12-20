using UnityEngine;
using UnityEngine.UI;

public class PortraitManager : MonoBehaviour
{
    public Button portraitSelector; // PortraitSelector ���s
    public GameObject panel; // Panel�]�]�t Toggles �M ToggleGroup�^
    public ToggleGroup toggleGroup; // Panel �W�� ToggleGroup

    private const string PORTRAIT_PREF_KEY = "Portrait";

    void Start()
    {
        // ��l�� Panel �� inactive
        panel.SetActive(false);

        // �ˬd PlayerPrefs �O�_���O�s Portrait ��
        string savedPortrait = PlayerPrefs.GetString(PORTRAIT_PREF_KEY, null);

        if (!string.IsNullOrEmpty(savedPortrait))
        {
            // �p�G���O�s Portrait�A�������� Toggle�A�]�� isOn
            foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
            {
                Image backgroundImage = toggle.transform.Find("Background").GetComponent<Image>();
                if (backgroundImage != null && backgroundImage.sprite.name == savedPortrait)
                {
                    // ��s PortraitSelector ���Ϥ�
                    UpdatePortraitSelectorImage(backgroundImage.sprite);
                    toggle.isOn = true;
                    break;
                }
            }
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
                    PlayerPrefs.SetString(PORTRAIT_PREF_KEY, backgroundImage.sprite.name);
                    PlayerPrefs.Save();

                    // ��s PortraitSelector ���Ϥ�
                    UpdatePortraitSelectorImage(backgroundImage.sprite);
                }
            }
        }

        // �]�w PortraitSelector ���s���I���ƥ�
        portraitSelector.onClick.AddListener(() => panel.SetActive(true));

        // �]�w�C�� Toggle ���I���ƥ�
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

        // �]�w Panel ���I���ƥ�
        Button panelButton = panel.GetComponent<Button>();
        if (panelButton != null)
        {
            panelButton.onClick.AddListener(() => panel.SetActive(false));
        }
    }

    private void OnToggleSelected(Toggle selectedToggle)
    {
        // ����襤 Toggle ���I���Ϥ��W��
        Image backgroundImage = selectedToggle.transform.Find("Background").GetComponent<Image>();
        if (backgroundImage != null)
        {
            string portraitName = backgroundImage.sprite.name;

            // ��s PlayerPrefs
            PlayerPrefs.SetString(PORTRAIT_PREF_KEY, portraitName);
            PlayerPrefs.Save();

            // ��s PortraitSelector ���Ϥ�
            UpdatePortraitSelectorImage(backgroundImage.sprite);

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
}
