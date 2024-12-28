using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq; // �нT�O�w�� Newtonsoft.Json NuGet �M��

public class JSON2CSV
{
    public static List<Dictionary<string, string>> ConvertJsonToCsv(string jsonData)
    {
        // �ѪR JSON �r��
        var jsonObject = JObject.Parse(jsonData);

        // ��� "data" �`�I
        var dataNode = jsonObject["data"];
        if (dataNode == null)
        {
            throw new Exception("Invalid JSON format: 'data' field not found.");
        }

        // �O�s���G
        var csvList = new List<Dictionary<string, string>>();

        // �M���C�Ӥ���
        foreach (var page in dataNode)
        {
            var property = (JProperty)page; // �N JToken �૬�� JProperty
            string pageName = property.Name; // �ϥ� Name �ݩʨ��o��
            string csvData = property.Value.ToString(); // �ϥ� Value �ݩʨ��o��

            // �K�[�쵲�G
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
