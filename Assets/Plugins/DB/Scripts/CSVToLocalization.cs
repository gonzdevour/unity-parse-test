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

            for (int j = 2; j < headers.Length; j++) //�q2�}�l�A�]���n�����Lkey, id����
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
                    entry.Value = value; // ���ؤw�s�b�A��s��
                }
                else
                {
                    table.AddEntry(key, value); // ���ؤ��s�b�A�K�[�s����
                }
            }
        }

        Debug.Log($"Localization data updated for table '{tableName}' successfully!");
    }



    public string ExtractLocaleFromHeader(string header)
    {
        int start = header.LastIndexOf('('); // ���̫�@�ӥ��A��
        int end = header.LastIndexOf(')');  // ���̫�@�ӥk�A��
        if (start >= 0 && end > start)
        {
            return header.Substring(start + 1, end - start - 1).Trim(); // �����A���������e
        }
        return header.Trim(); // �p�G�S���A���A��^��Ӽ��D
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
                return entry.Value; // ��^���ت���
            }
        }

        return $"Missing text for {localeCode}:{key} in table '{tableName}'";
    }

}
