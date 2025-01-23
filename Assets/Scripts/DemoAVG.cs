using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoAVG : MonoBehaviour
{
    public Button Btn_StoryStart;

    SQLiteManager dbManager;
    SheetToDB sheetToDB;

    void Start()
    {
        Btn_StoryStart.onClick.AddListener(() => StartCoroutine(StartAVG()));

        dbManager = new SQLiteManager(Path.Combine(Application.persistentDataPath, "dynamicDatabase.db"));
        sheetToDB = GetComponent<SheetToDB>();

        //StartCoroutine(Test());
        //Test2();
        StartCoroutine(StartAVG());
    }

    void OnDestroy()
    {
        AVG.Inst.Off();
    }

    IEnumerator StartAVG()
    {
        yield return null; //等待Global scene初始化
        var panelSpinner = CanvasUI.Inst.panelSpinner;
        panelSpinner.gameObject.SetActive(true);
        panelSpinner.SetMessage("「與怪物戰鬥之人，應當小心自己不要成為怪物。」");

        //AVG.Inst.On();

        yield return sheetToDB.LoadExcel("Story.xlsx");
        yield return AVG.Inst.Init(); //讀取資料完成後才能初始化
        panelSpinner.gameObject.SetActive(false);
        yield return AVG.Inst.AVGStart();
    }

    void DataSet<T>(string key, T value)
    {
        PPM.Inst.Set(key, value);
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
}
