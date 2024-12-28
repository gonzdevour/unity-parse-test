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

    // �w�q����Ƶ��c
    public class StoryList
    {
        public string Title { get; set; }
        public string Condition { get; set; }
        public string Description { get; set; }
    }

    public class StoryCut
    {
        public int ���� { get; set; }
        public string ��m { get; set; }
        public string ���ܪ� { get; set; }
        public string �� { get; set; }
        public string ���ܤ��e { get; set; }
        public string �ﶵ { get; set; }
        public string �e�� { get; set; }
        public string ���ܫe { get; set; }
        public string ���ܫ� { get; set; }
    }

    void Start()
    {
        dbManager = new SQLiteManager(Path.Combine(Application.persistentDataPath, "dynamicDatabase.db"));
        sheetToDB = GetComponent<SheetToDB>();

        // �b PlayerPrefs ���]�m���ռƾ�
        PlayerPrefs.SetString("�m�W", "Ĭ  �F�Y");
        PlayerPrefs.SetFloat("�魫", 40.3f);
        PlayerPrefs.SetInt("���", 20240103);
        PlayerPrefs.SetInt("hp", 1);
        PlayerPrefs.SetInt("mp", 2);
        PlayerPrefs.SetInt("�ثe�ɶ�", 1500);
        PlayerPrefs.SetInt("���p���n�P��", 4);
        PlayerPrefs.SetInt("���j��n�P��", 5);

        //StartCoroutine(Test());
        //Test2();
        StartCoroutine(StartAVG());
    }

    IEnumerator StartAVG()
    {
        yield return null;
        yield return sheetToDB.LoadExcel("Story.xlsx");
        FilterStories("StoryList");//�M���P�_�ثe�ŦX���󪺼@���A�N�@���W�٥[�JAVG player
        yield return AVG.Inst.StoryStart();

    }

    IEnumerator Test()
    {
        yield return null; //����global scene
        // Ū����Ʈw
        dbManager = new SQLiteManager(Path.Combine(Application.persistentDataPath, "dynamicDatabase.db"));
        // �ˬd�O�_��d�ߪ��ƾ�
        List<StoryList> allItems = null;
        try
        {
            allItems = dbManager.QueryTable<StoryList>("StoryList");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to query table: {e.Message}");
        }
        // �p�G�ƾڪ��šA�U�� CSV �þɤJ�ƾ�
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
//�ثe�ɶ�>=1207 || �ثe�ɶ�<1010
//���p���n�P��  >  3 && ���j��n�P��>=3
//�魫>40.2
//�m�W   =  Ĭ  �F�Y
//";

        //// �ե� EvaluateCondition ��������G
        //bool result = Judge.EvaluateCondition(condition);
        //// �d�ݵ��G
        //Debug.Log($"���󵲪G: {result}");

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

                // �N�ŦX���� Title �K�[�� PendingStoryTitles
                AVG.Inst.PendingStoryTitles.Add(item.Title);
            }
        }
    }


    public void Db_CreateByCSV(Dictionary<string, string> resultDict)
    {
        //�L�XresultDict
        foreach (var kvp in resultDict)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value}");
        }

        //���odb path
        string dbPath = Path.Combine(Application.persistentDataPath, "dynamicDatabase.db");
        Debug.Log(dbPath);

        // �b CSV �U��������Ұʸ�Ʈw�ɤJ
        //SaveCSVToFile(resultDict["CSVData"]);
        CSVToSQLite.Inst.ImportCSVToDatabase(resultDict["PageName"], resultDict["CSVData"], dbPath);
    }
}
