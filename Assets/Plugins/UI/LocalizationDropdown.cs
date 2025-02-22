using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using System.Collections.Generic;

public class LocalizationDropdown : MonoBehaviour
{
    public Dropdown languageDropdown; // 指向 UI Dropdown 元件

    // 自定義語言名稱映射表
    private Dictionary<string, string> languageNames = new Dictionary<string, string>
    {
        { "en", "English" },
        { "zh-Hans", "简体中文" },
        { "zh-TW", "正體中文" },
        { "ja", "日本語" }
    };

    private void Start()
    {
        // 等待本地化系統初始化完成
        LocalizationSettings.InitializationOperation.Completed += operation =>
        {
            // 初始化下拉選單
            InitializeDropdown();
        };
    }

    private void InitializeDropdown()
    {
        // 獲取所有可用語系
        var availableLocales = LocalizationSettings.AvailableLocales.Locales;

        // 清空並填充下拉選單
        languageDropdown.options.Clear();
        foreach (var locale in availableLocales)
        {
            // 使用自定義名稱，默認使用代碼作為名稱
            string localeCode = locale.Identifier.Code;
            string customName = languageNames.ContainsKey(localeCode) ? languageNames[localeCode] : localeCode;

            languageDropdown.options.Add(new Dropdown.OptionData(customName));
        }

        // 設置當前語系為選中的選項
        var currentLocale = LocalizationSettings.SelectedLocale;
        languageDropdown.value = availableLocales.IndexOf(currentLocale);

        // 添加事件監聽
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

    private void OnLanguageChanged(int index)
    {
        // 獲取選中的語系
        var selectedLocale = LocalizationSettings.AvailableLocales.Locales[index];

        // 切換語系
        LocalizationSettings.SelectedLocale = selectedLocale;

        Debug.Log($"Language switched to: {selectedLocale.Identifier.Code}");
    }
}
