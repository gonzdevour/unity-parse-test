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

    public class Preset
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    // 定義表格資料結構
    public class StoryList
    {
        public string Title { get; set; }
        public string Condition { get; set; }
        public string Description { get; set; }
    }

    public class StoryCut
    {
        public int 索引 { get; set; }
        public string 位置 { get; set; }
        public string 說話者 { get; set; }
        public string 表情 { get; set; }
        public string 說話內容 { get; set; }
        public string 選項 { get; set; }
        public string 前往 { get; set; }
        public string 說話前 { get; set; }
        public string 說話後 { get; set; }
        public string 顯示名稱 { get; set; }
    }

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

        AVG.Inst.On();

        yield return sheetToDB.LoadExcel("Story.xlsx");
        panelSpinner.gameObject.SetActive(false);

        UpdatePPM("Preset");//更新預設值
        FilterStories("StoryList");//遍歷判斷目前符合條件的劇本，將劇本名稱加入AVG player

        StartCoroutine(Director.Inst.FadeInWithDelay(1f));//等待背景讀入後再FadeIn

        yield return AVG.Inst.StoryQueueStart<StoryCut>(() => 
        {
            StartCoroutine(EndAVG());
        });
    }

    IEnumerator EndAVG()
    {
        Director.Inst.FadeOut();
        yield return new WaitForSeconds(1f);
        AVG.Inst.Off();
        Debug.Log("Story Fin");
    }

    IEnumerator Test()
    {
        yield return null; //等待global scene
        // 讀取資料庫
        dbManager = new SQLiteManager(Path.Combine(Application.persistentDataPath, "dynamicDatabase.db"));
        // 檢查是否能查詢表單數據
        List<StoryList> allItems = null;
        try
        {
            allItems = dbManager.QueryTable<StoryList>("StoryList");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to query table: {e.Message}");
        }
        // 如果數據表為空，下載 CSV 並導入數據
        if (allItems == null || allItems.Count == 0)
        {
            yield return CSVDownloader.Inst.DownloadCSV(
                "https://docs.google.com/spreadsheets/d/e/2PACX-1vTLZ3ILVCAA8_XsYeM75tTD9WNRJjCT4YhDLxqqSbKvz1LkBEVbFtx_pQ1NCHVtmi5TJ7s88cuolKQJ/pub?gid=0&single=true&output=csv",
                (resultDict) =>
                {
                    Db_CreateByCSV(resultDict);
                }
            );
        }
        FilterStories("StoryList");

//        string condition = @"date=20240103
//hp>=0 && mp<3
//目前時間>=1207 || 目前時間<1010
//王小美好感度  >  3 && 李大花好感度>=3
//體重>40.2
//姓名   =  蘇  東坡
//";

        //// 調用 EvaluateCondition 並獲取結果
        //bool result = Judge.EvaluateCondition(condition);
        //// 查看結果
        //Debug.Log($"條件結果: {result}");

    }

    void DataSet<T>(string key, T value)
    {
        PPM.Inst.Set(key, value);
    }

    public void UpdatePPM(string pageName)
    {
        // 在 PPM 中設置測試字串數據，同時支援TxR
        List<Preset> allItems = dbManager.QueryTable<Preset>(pageName);
        foreach (var item in allItems)
        {
            //Debug.Log($"{item.Key}={item.Value}");
            PPM.Inst.Set(item.Key, item.Value);
        }
    }

    public void FilterStories(string pageName)
    {
        List<StoryList> allItems = dbManager.QueryTable<StoryList>(pageName);

        foreach (var item in allItems)
        {
            Debug.Log($"title:{item.Title}, cond:{item.Condition}, desc:{item.Description}");

            if (Judge.EvaluateCondition(item.Condition))
            {
                Debug.Log($"Condition met: {item.Title}");

                // 將符合條件的 Title 添加到 PendingStoryTitles
                AVG.Inst.PendingStoryTitles.Add(item.Title);
            }
        }
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
