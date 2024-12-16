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

    public PanelLoadingProgress panelLoadingProgress;
    public GameObject panelSpinner;
    private SQLiteManager dbManager;

    void Start()
    {
        // 呼叫測試函數
        StartCoroutine(Test());
    }

    public void TestLog() 
    {
        System.Text.StringBuilder randomString = new System.Text.StringBuilder(550); // 預設容量略大於500，預留換行符
        for (int i = 0; i < 500; i++)
        {
            int randomDigit = Random.Range(0, 10); // 產生0到9之間的隨機數字
            randomString.Append(randomDigit);

            // 每10個數字後添加換行符
            if ((i + 1) % 10 == 0)
            {
                randomString.Append("\n");
            }
        }
        Debug.Log(randomString.ToString());
    }

    public IEnumerator Test()
    {
        yield return new WaitForSeconds(1f);
        ////過關
        //yield return TestProgress();

        //panelSpinner.SetActive(true);
        //panelSpinner.GetComponent<PanelSpinner>().SetMessage("讀取streamingAsset裡的Txt");
        //yield return TestLoadTxt();
        yield return TestLoadImg();

        //webgl不支援NPOI，改用nodejs app解析.docx和.xlsx
        //panelSpinner.GetComponent<PanelSpinner>().SetMessage("讀取streamingAsset裡的Docx");
        //yield return TestLoadDocx();
        //panelSpinner.GetComponent<PanelSpinner>().SetMessage("讀取streamingAsset裡的Excel");
        //yield return TestLoadExcel();

        ////過關
        //panelSpinner.GetComponent<PanelSpinner>().SetMessage("gas讀取googleDoc");
        //yield return TestGetGoogleDoc();
        //panelSpinner.GetComponent<PanelSpinner>().SetMessage("gas讀取googleSheet依分頁解析為csv");
        //yield return TestGetGoogleSheetsAsCSV();
        //panelSpinner.GetComponent<PanelSpinner>().SetMessage("gas從GoogleDrive資料夾獲取檔案");
        //yield return TestGetDocsInGoogleDriveFolder();
        //panelSpinner.GetComponent<PanelSpinner>().SetMessage("讀取開放連結的csv作為本地化資料");
        //yield return TestDownloadCSV();
        //panelSpinner.GetComponent<PanelSpinner>().SetMessage("讀取開放連結的一群csv匯入sqldb");
        //yield return TestDownloadAllCSV();
        //TestDB();
        //panelSpinner.SetActive(false);

    }

    /// <summary>
    /// 測試進度條功能。
    /// </summary>
    public IEnumerator TestProgress()
    {
        int taskCount = 3;
        panelLoadingProgress.StartProgress(taskCount, "Test Start", "Starting tasks...");

        for (int i = 1; i <= taskCount; i++)
        {
            yield return new WaitForSeconds(Random.Range(1f, 1.5f));
            panelLoadingProgress.Add(1, $"Task {i} Complete", "Processing...");
        }
        yield return new WaitForSeconds(1f);
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

    private IEnumerator TestLoadImg()
    {
        yield return sa.LoadImg("Image/duck.png", resultTexture => {
            Debug.Log($"LoadImg from StreamingAsset complete");
            sa.UpdateImageTexture("Img_FromSA",resultTexture);
        });
    }

    private IEnumerator TestLoadTxt()
    {
        yield return sa.LoadTxt("gogo.txt", resultDict => {
            Debug.Log($"LoadTxt from StreamingAsset result:");
            Debug.Log($"{resultDict["TextData"]}");
        });
    }

    private IEnumerator TestLoadDocx()
    {
        yield return sa.LoadDocx("12n.docx", resultString => {
            Debug.Log($"LoadDocx from StreamingAsset result:");
            Debug.Log($"{resultString}");
        });
    }

    private IEnumerator TestLoadExcel()
    {
        yield return sa.LoadExcel("test.xlsx", resultString => {
            Debug.Log($"LoadExcel from StreamingAsset result:");
            Debug.Log($"{resultString}");
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
            //Debug.Log($"DownloadCSV result:\nPage Name: {resultDict["PageName"]}\nCSV Data:\n{resultDict["CSVData"]}");
            StartCoroutine(csv2Loc.Update("StringLoc", resultDict["CSVData"]));
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
