using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq; // 請確保安裝 Newtonsoft.Json NuGet 套件

public class JSON2CSV
{
    public static List<Dictionary<string, string>> ConvertJsonToCsv(string jsonData)
    {
        // 解析 JSON 字串
        var jsonObject = JObject.Parse(jsonData);

        // 獲取 "data" 節點
        var dataNode = jsonObject["data"];
        if (dataNode == null)
        {
            throw new Exception("Invalid JSON format: 'data' field not found.");
        }

        // 保存結果
        var csvList = new List<Dictionary<string, string>>();

        // 遍歷每個分頁
        foreach (var page in dataNode)
        {
            var property = (JProperty)page; // 將 JToken 轉型為 JProperty
            string pageName = property.Name; // 使用 Name 屬性取得鍵
            string csvData = property.Value.ToString(); // 使用 Value 屬性取得值

            // 添加到結果
            var csvEntry = new Dictionary<string, string>
            {
                { "PageName", pageName },
                { "CSVData", csvData }
            };

            csvList.Add(csvEntry);
        }

        return csvList;
    }
}
