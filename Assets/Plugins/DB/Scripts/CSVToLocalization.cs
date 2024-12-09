using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class CSVToLocalization
{
    public Dictionary<string, Dictionary<string, string>> Load(string csvData)
    {
        if (string.IsNullOrEmpty(csvData))
        {
            Debug.LogError("CSV data is null or empty!");
            return null;
        }

        string[] lines = csvData.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2)
        {
            Debug.LogError("CSV data does not contain enough lines!");
            return null;
        }

        string[] headers = lines[0].Split(',');

        var localization = new Dictionary<string, Dictionary<string, string>>();

        for (int i = 1; i < lines.Length; i++) 
        {
            string[] columns = lines[i].Split(',');
            string key = columns[0];

            for (int j = 2; j < headers.Length; j++) //從2開始，因為要先略過key, id兩欄
            {
                string localeCode = ExtractLocaleFromHeader(headers[j]);
                string value = columns[j];

                if (!localization.ContainsKey(key))
                {
                    localization[key] = new Dictionary<string, string>();
                }

                localization[key][localeCode] = value;
                Debug.Log($"[{key}][{localeCode}] = {value}");
            }
        }

        Debug.Log("Localization data loaded successfully!");
        return localization;
    }

    public void Update(string tableName, string csvData)
    {
        var localization = Load(csvData);
        if (localization == null)
        {
            Debug.LogError("Failed to load localization data.");
            return;
        }

        foreach (var localePair in localization)
        {
            string key = localePair.Key;

            foreach (var translation in localePair.Value)
            {
                string localeCode = translation.Key;
                string value = translation.Value;

                var locale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
                if (locale == null)
                {
                    Debug.LogError($"Locale '{localeCode}' not found.");
                    continue;
                }

                var table = LocalizationSettings.StringDatabase.GetTable(tableName, locale) as StringTable;
                if (table == null)
                {
                    Debug.LogError($"StringTable for locale '{localeCode}' not found in table '{tableName}'.");
                    continue;
                }

                var entry = table.GetEntry(key);
                if (entry != null)
                {
                    entry.Value = value; // 條目已存在，更新值
                }
                else
                {
                    table.AddEntry(key, value); // 條目不存在，添加新條目
                }
            }
        }

        Debug.Log($"Localization data updated for table '{tableName}' successfully!");
    }



    public string ExtractLocaleFromHeader(string header)
    {
        int start = header.LastIndexOf('('); // 找到最後一個左括號
        int end = header.LastIndexOf(')');  // 找到最後一個右括號
        if (start >= 0 && end > start)
        {
            return header.Substring(start + 1, end - start - 1).Trim(); // 提取括號內的內容
        }
        return header.Trim(); // 如果沒有括號，返回整個標題
    }


    public string GetLocalizedText(string tableName, string localeCode, string key)
    {
        var locale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
        if (locale == null)
        {
            Debug.LogError($"Locale '{localeCode}' not found.");
            return null;
        }

        var table = LocalizationSettings.StringDatabase.GetTable(tableName, locale) as StringTable;
        if (table != null)
        {
            var entry = table.GetEntry(key);
            if (entry != null)
            {
                return entry.Value; // 返回條目的值
            }
        }

        return $"Missing text for {localeCode}:{key} in table '{tableName}'";
    }

}
