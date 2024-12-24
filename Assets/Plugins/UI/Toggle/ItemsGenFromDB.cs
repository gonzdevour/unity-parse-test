using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ItemsGenFromDB : MonoBehaviour
{
    SQLiteManager dbManager;

    public PortraitSelector itemSelector;
    public GameObject itemList;
    public GameObject itemPrefab;

    // 定義表格資料結構
    public class HistoryEvent
    {
        public string name { get; set; }
        public string born { get; set; }
        public string 簡介 { get; set; }
    }

    void Start()
    {
        //StartCoroutine(MakeItemTable());
    }

    public IEnumerator MakeItemTable()
    {
        // 清空 itemList 的所有子物件
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }
        // 每秒檢查Global scene是否已加載
        yield return WaitForScene("Global", 1f);
        Debug.Log("Global scene is loaded");

        // 讀取資料庫
        dbManager = new SQLiteManager(Path.Combine(Application.persistentDataPath, "dynamicDatabase.db"));
        List<HistoryEvent> allEvents = dbManager.QueryTable<HistoryEvent>("查找器");
        if (allEvents.Count == 0)
        {
            Debug.Log("Start DownloadCSV");
            yield return DownloadCSV(
                "1vT6trpTZO0gdjTGiRosRmHsd1fjzbRfBH1xLLbAHer5xWmZjglEZfYNAPUbKjp0Pj3o4et4AsS0bm-z", //sheetId
                "1874800715" //gid
                );
        }
        Db_MapToItems("查找器");
        itemSelector.UpdateToggleIsOn();
    }

    private IEnumerator WaitForScene(string sceneName, float interval)
    {
        // 等待Global場景加載
        while (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            Debug.Log("waiting for Global scene");
            yield return new WaitForSeconds(interval); // 每隔一段時間檢查一次
        }
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

    public void Db_MapToItems(string pageName)
    {
        // 清空 itemList 的所有子物件
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        // 基於Event class取得"events"表單中的資料映射到allEvents物件中
        List<HistoryEvent> allEvents = dbManager.QueryTable<HistoryEvent>(pageName);
        // 印出allEvents物件中的各列指定內容
        foreach (var eventItem in allEvents)
        {
            Debug.Log($"姓名:{eventItem.name}, 生日:{eventItem.born}, 簡介:{eventItem.簡介}");
            ItemGen(eventItem);
        }
    }

    private void ItemGen(HistoryEvent eventItem)
    {
        // 從 itemList 取得 ToggleGroup
        ToggleGroup toggleGroup = itemList.GetComponent<ToggleGroup>();
        // 生成新的 Toggle 子物件
        GameObject newToggle = Instantiate(itemPrefab, itemList.transform);
        // 取得生成物件上的 Toggle，設定 Group
        Toggle ToggleComponent = newToggle.GetComponent<Toggle>();
        ToggleComponent.group = toggleGroup;

        // 設置 Toggle 的名稱
        newToggle.name = $"Toggle_{eventItem.name}";

        // 設置 Toggle的LabelName的名稱
        ToggleVariables toggleVar = newToggle.GetComponent<ToggleVariables>();
        toggleVar.SetLabelName(eventItem.name);

        // 找到 Background 的 Image 組件
        Image backgroundImage = newToggle.transform.Find("Background").GetComponent<Image>();

        string imgUrl = $"https://playoneapps.com.tw/images/roc/portrait/{eventItem.name}.png";
        SpriteCacher.Inst.GetSprite(imgUrl, (sprite) =>
        {
            if (backgroundImage != null)
            {
                //Debug.Log($"spriteName: {backgroundImage.sprite.name}");
                backgroundImage.sprite = sprite;
            }
            else
            {
                Debug.LogError($"Background Image not found in {newToggle.name}");
            }
        });
        itemSelector.RegisterToggleEvent(ToggleComponent, imgUrl);
    }
}
