using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SheetToDB : MonoBehaviour
{
    public SQLiteManager dbManager;

    // 定义一个与 events 表结构相匹配的类
    public class HistoryEvent
    {
        public int Id { get; set; }
        public string 日期 { get; set; }
        public int 日期編號 { get; set; }
        public string 歷史 { get; set; }
    }

    void Start()
    {
        // 呼叫測試函數
        //StartCoroutine(Test());
    }

    public IEnumerator Test()
    {
        yield return new WaitForSeconds(1f);
        //yield return GetGoogleSheetsAsCSV();
        //yield return DownloadCSV();
        //yield return DownloadAllCSV();
        //TestDB();
    }

    public IEnumerator LoadExcel(string filePath)
    {
        yield return StreamingAssets.Inst.LoadExcel(filePath, resultString => {
            Debug.Log($"LoadExcel from StreamingAsset result:");
            Debug.Log($"{resultString}");
            var csvList = JSON2CSV.ConvertJsonToCsv( resultString );
            foreach (var item in csvList)
            {
                Debug.Log($"PageName: {item["PageName"]}");
                Debug.Log($"CSVData: {item["CSVData"]}");
                Db_CreateByCSV(item);
            }
        });
    }

    private IEnumerator GetGoogleSheetsAsCSV()
    {
        List<string> sheetIdArr = new()
        {
            "1rzPMs8Hbh12_HThp1IMJjsslG4f3Ehhyni8Vq9NSXyE",
        };
        yield return Gas.Inst.GetGoogleSheetsAsCSV(sheetIdArr, resultDict => {
            Debug.Log($"GetGoogleSheetsAsCSV result:");
            Debug.Log($"{resultDict["TextData"]}");
        });
    }

    private IEnumerator DownloadCSV(string sheetId, string gid)
    {
        string urlHead = "https://docs.google.com/spreadsheets/d/e/2PACX-";
        string urlTail = $"/pub?gid={gid}&single=true&output=csv";
        string url = $"{urlHead}{sheetId}{urlTail}";
        yield return CSVDownloader.Inst.DownloadCSV(url, resultDict => {
            Db_CreateByCSV(resultDict);
        });
    }

    private IEnumerator DownloadAllCSV()
    {
        string urlHead = "https://docs.google.com/spreadsheets/d/e/2PACX-";
        string urlTail = "/pub?gid=0&single=true&output=csv";
        List<string> csvUrls = new()
        {
            urlHead + "1vR19m0xM9Qr7HeJqcNWUeN0cvryz9DWiWcAnp_2QSYi2F9evqBL5gIL2QeVAQ2iXUkJPY4nh-Xa6vPc" + urlTail,
        };

        yield return CSVDownloader.Inst.DownloadAllCSV(csvUrls, resultDict => {
            Db_CreateByCSV(resultDict);
        });
    }

    public void Db_CreateByCSV(Dictionary<string, string> resultDict)
    {
        //印出resultDict
        foreach (var kvp in resultDict)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value}");
        }

        //取得db path
        string dbPath = Path.Combine(Application.persistentDataPath, "dynamicDatabase.db");
        Debug.Log(dbPath);

        // 在 CSV 下載完成後啟動資料庫導入
        //SaveCSVToFile(resultDict["CSVData"]);
        CSVToSQLite.Inst.ImportCSVToDatabase(resultDict["PageName"], resultDict["CSVData"], dbPath);
    }

    private void TestDB()
    {
        dbManager = new SQLiteManager(Path.Combine(Application.persistentDataPath, "dynamicDatabase.db"));
        Db_PrintAll("events");
        Db_InsertFromHeaderValue();
        Db_PrintAll("events");
        Db_InsertFromObj();
        Db_PrintAll("events");
        Db_Query();

        Db_Delete();
        Db_PrintAll("events");
        Db_Update();
        Db_PrintAll("events");
    }

    public void Db_PrintAll(string pageName)
    {
        // 基於Event class取得"events"表單中的資料映射到allEvents物件中
        List<HistoryEvent> allEvents = dbManager.QueryTable<HistoryEvent>(pageName);
        // 印出allEvents物件中的各列指定內容
        foreach (var eventItem in allEvents)
        {
            Debug.Log($"Event: {eventItem.歷史}, Date: {eventItem.日期}");
        }
    }
    public void Db_InsertFromHeaderValue()
    {
        Debug.Log("---insert header-value---");
        // 依header-value插入数据
        string[] headers = { "日期", "日期編號", "歷史" };
        object[] values = { "2024-03-08", 20240308, "婦女節" };
        dbManager.InsertData("events", headers, values);
    }
    public void Db_InsertFromObj()
    {
        Debug.Log("---insert obj---");
        HistoryEvent myEvent = new()
        {
            日期 = "2024-01-01",
            日期編號 = 20240101,
            歷史 = "新年慶祝活動"
        };
        // 依物件插入数据
        dbManager.InsertDataFromObject("events", myEvent);
    }
    public void Db_Query()
    {
        Debug.Log("---query---");
        // 查詢數據
        // 基於Event class取得"events"表單中的資料映射到allEvents物件中
        List<HistoryEvent> allEvents = dbManager.QueryTable<HistoryEvent>("events", "日期編號 < 20240101", "日期編號 ASC");//DESC
        // 印出allEvents物件中的各列指定內容
        foreach (var eventItem in allEvents)
        {
            Debug.Log($"Event: {eventItem.歷史}, Date: {eventItem.日期}");
        }
    }
    public void Db_Delete()
    {
        Debug.Log("---delete---");
        // 刪除數據
        dbManager.DeleteData("events", "日期編號 < 20231231");
    }
    public void Db_Update()
    {
        Debug.Log("---update---");
        // 更新數據
        string[] headers = { "日期", "歷史" };
        string[] values = { "2024-02-14", "情人節" };
        dbManager.UpdateData("events", headers, values, "日期編號 = 20240101");//依id更新："Id = 1"
    }
}
