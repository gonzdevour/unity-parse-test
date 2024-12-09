using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Localization.Settings;

// 定义一个与 events 表结构相匹配的类
public class Event
{
    public int Id { get; set; }
    public string 日期 { get; set; }
    public int 日期編號 { get; set; }
    public string 歷史 { get; set; }
}

public class Demo : MonoBehaviour
{
    public StreamingAssets sa;
    public Gas gas;
    public CSVDownloader csvDownloader;
    public CSVToSQLite csv2sq;
    public CSVToLocalization csv2Loc = new();

    public GameObject panelSpinner;
    private SQLiteManager dbManager;

    void Start()
    {
        // 呼叫測試函數
        StartCoroutine(Test());
    }

    public IEnumerator Test()
    {
        panelSpinner.SetActive(true);
        //yield return TestLoadTxt();
        //yield return TestLoadDocx();
        //yield return TestLoadExcel();
        //yield return TestGetGoogleDoc();
        //yield return TestGetGoogleSheetsAsCSV();
        //yield return TestGetDocsInGoogleDriveFolder();
        yield return TestDownloadCSV();
        //yield return TestDownloadAllCSV();
        //TestDB(); 
        panelSpinner.SetActive(false);

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

    private IEnumerator TestLoadTxt()
    {
        yield return sa.LoadTxt("gogo.txt", resultDict => {
            Debug.Log($"LoadTxt result:");
            Debug.Log($"{resultDict["TextData"]}");
        });
    }

    private IEnumerator TestLoadDocx()
    {
        yield return sa.LoadDocx("12n.docx", resultDict => {
            Debug.Log($"LoadDocx result:");
            Debug.Log($"{resultDict["TextData"]}");
        });
    }

    private IEnumerator TestLoadExcel()
    {
        yield return sa.LoadExcel("test.xlsx", resultDict => {
            Debug.Log($"LoadExcel result:");
            foreach (var page in resultDict)
            {
                Debug.Log($"Page Name: {page["PageName"]}");
                Debug.Log($"CSV Data:\n{page["CSVData"]}");
            }
        });
    }

    private IEnumerator TestGetGoogleDoc()
    {
        string docId = "1-yQjykZ4lPBzaO8qpXPmy8ITUS8WRD_kgGDnug94pzQ";
        yield return gas.GetGoogleDoc(docId, resultDict => {
            Debug.Log($"GetGoogleDoc result:");
            Debug.Log($"{resultDict["TextData"]}");
        });
    }

    private IEnumerator TestGetGoogleSheetsAsCSV()
    {
        List<string> sheetIdArr = new()
        {
            "1rzPMs8Hbh12_HThp1IMJjsslG4f3Ehhyni8Vq9NSXyE",
        };
        yield return gas.GetGoogleSheetsAsCSV(sheetIdArr, resultDict => {
            Debug.Log($"GetGoogleSheetsAsCSV result:");
            Debug.Log($"{resultDict["TextData"]}");
        });
    }

    private IEnumerator TestGetDocsInGoogleDriveFolder()
    {
        string folderId = "1vmjdReoeZM5vqTjgkIEWPbfm7_OLEu9U";
        yield return gas.GetDocsInGoogleDriveFolder(folderId, resultDict => {
            Debug.Log($"GetDocsInGoogleDriveFolder result:");
            Debug.Log($"{resultDict["TextData"]}");
        });
    }

    private IEnumerator TestDownloadCSV()
    {
        string urlHead = "https://docs.google.com/spreadsheets/d/e/2PACX-";
        string urlTail = "/pub?gid=0&single=true&output=csv";
        string url = urlHead + "1vRqq-0kSrH6IUFzb6ovIfL-o0ER7_WYjlmBiEMM2GNRhIs1wPCbdbjbhEKXZinXPa_NlpRxtuvuk5Fk" + urlTail;

        yield return csvDownloader.DownloadCSV(url, resultDict => {
            Debug.Log($"DownloadCSV result:");
            Debug.Log($"Page Name: {resultDict["PageName"]}");
            Debug.Log($"CSV Data:\n{resultDict["CSVData"]}");

            csv2Loc.Update("StringLoc", resultDict["CSVData"]);
        });
    }

    private IEnumerator TestDownloadAllCSV()
    {
        string urlHead = "https://docs.google.com/spreadsheets/d/e/2PACX-";
        string urlTail = "/pub?gid=0&single=true&output=csv";
        List<string> csvUrls = new()
        {
            urlHead + "1vR19m0xM9Qr7HeJqcNWUeN0cvryz9DWiWcAnp_2QSYi2F9evqBL5gIL2QeVAQ2iXUkJPY4nh-Xa6vPc" + urlTail,
        };

        yield return csvDownloader.DownloadAllCSV(csvUrls, resultDict => {
            Db_CreateByCSV(resultDict);
        });
    }

    public void Db_CreateByCSV(Dictionary<string,string> resultDict)
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
        csv2sq.ImportCSVToDatabase(resultDict["PageName"], resultDict["CSVData"], dbPath);
    }
    public void Db_PrintAll(string pageName) 
    {
        // 基於Event class取得"events"表單中的資料映射到allEvents物件中
        List<Event> allEvents = dbManager.QueryTable<Event>(pageName);
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
        Event myEvent = new()
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
        List<Event> allEvents = dbManager.QueryTable<Event>("events", "日期編號 < 20240101", "日期編號 ASC");//DESC
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
