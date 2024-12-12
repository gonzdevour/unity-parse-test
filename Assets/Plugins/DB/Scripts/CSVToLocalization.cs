using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
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
                //印出所有語系的鍵值
                //Debug.Log($"[{key}][{localeCode}] = {value}");
            }
        }

        Debug.Log("本地化資料外部讀取成功");
        return localization;
    }

    public IEnumerator Update(string tableName, string csvData)
    {
        // 確認開始執行函數
        //Debug.Log("Update function started.");

        // 等待本地化系統初始化完成
        yield return LocalizationSettings.InitializationOperation;
        if (LocalizationSettings.InitializationOperation.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("LocalizationSettings initialization failed.");
            yield break;
        }
        //Debug.Log("LocalizationSettings initialized successfully.");

        var localization = Load(csvData);
        if (localization == null)
        {
            Debug.LogError("Failed to load localization data.");
            yield break;
        }
        //Debug.Log("Localization data loaded successfully.");

        foreach (var localePair in localization)
        {
            string key = localePair.Key;
            //Debug.Log($"Processing key: {key}");

            foreach (var translation in localePair.Value)
            {
                string localeCode = translation.Key;
                string value = translation.Value;
                //Debug.Log($"Processing translation - Locale: {localeCode}, Value: {value}");

                // 使用同步的 GetLocale 方法
                var locale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
                if (locale == null)
                {
                    Debug.LogError($"Locale '{localeCode}' not found.");
                    continue;
                }
                //Debug.Log($"Locale '{localeCode}' found.");

                // 异步获取字符串表
                var tableOperation = LocalizationSettings.StringDatabase.GetTableAsync(tableName, locale);
                //Debug.Log($"Fetching StringTable for locale '{localeCode}' in table '{tableName}'...");
                yield return tableOperation;

                if (tableOperation.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError($"Failed to fetch StringTable for locale '{localeCode}' in table '{tableName}'.");
                    continue;
                }

                var table = tableOperation.Result as StringTable;
                if (table == null)
                {
                    Debug.LogError($"StringTable for locale '{localeCode}' not found in table '{tableName}'.");
                    continue;
                }
                //Debug.Log($"StringTable for locale '{localeCode}' loaded successfully.");

                // 更新或添加条目
                var entry = table.GetEntry(key);
                if (entry != null)
                {
                    entry.Value = value; // 条目已存在，更新值
                    //Debug.Log($"Updated entry for key '{key}' with value '{value}'.");
                }
                else
                {
                    table.AddEntry(key, value); // 条目不存在，添加新条目
                    //Debug.Log($"Added new entry for key '{key}' with value '{value}'.");
                }
            }
        }

        Debug.Log($"'{tableName}' 本地化表格更新成功");
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
