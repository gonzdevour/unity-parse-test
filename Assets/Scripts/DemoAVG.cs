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
        if (AVG.Inst != null)
        {
            AVG.Inst.Off();
        }
    }

    IEnumerator StartAVG()
    {
        yield return null; //����Global scene��l��
        var panelSpinner = CanvasUI.Inst.panelSpinner;
        panelSpinner.gameObject.SetActive(true);
        panelSpinner.SetMessage("�u�P�Ǫ��԰����H�A����p�ߦۤv���n�����Ǫ��C�v");

        //AVG.Inst.On();

        yield return sheetToDB.LoadExcel("Mods/Official/Scripts/ModData.xlsx");
        yield return AVG.Inst.Init(); //Ū����Ƨ�����~���l��
        panelSpinner.gameObject.SetActive(false);

        // �L�o�ŦX���󪺼@��
        var stories = AVG.Inst.FilterStories("StoryList", "Tag LIKE '%�D�u%'");

        yield return AVG.Inst.AVGStart(stories);
    }

    void DataSet<T>(string key, T value)
    {
        PPM.Inst.Set(key, value);
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
