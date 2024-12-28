using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86;

public class DemoAVG : MonoBehaviour
{
    SQLiteManager dbManager;
    SheetToDB sheetToDB;

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
    }

    void Start()
    {
        dbManager = new SQLiteManager(Path.Combine(Application.persistentDataPath, "dynamicDatabase.db"));
        sheetToDB = GetComponent<SheetToDB>();

        // 在 PlayerPrefs 中設置測試數據
        PlayerPrefs.SetString("姓名", "蘇  東坡");
        PlayerPrefs.SetFloat("體重", 40.3f);
        PlayerPrefs.SetInt("日期", 20240103);
        PlayerPrefs.SetInt("hp", 1);
        PlayerPrefs.SetInt("mp", 2);
        PlayerPrefs.SetInt("目前時間", 1500);
        PlayerPrefs.SetInt("王小美好感度", 4);
        PlayerPrefs.SetInt("李大花好感度", 5);

        //StartCoroutine(Test());
        //Test2();
        StartCoroutine(StartAVG());
    }

    IEnumerator StartAVG()
    {
        yield return null;
        yield return sheetToDB.LoadExcel("Story.xlsx");
        FilterStories("StoryList");//遍歷判斷目前符合條件的劇本，將劇本名稱加入AVG player
        yield return AVG.Inst.StoryStart();

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
