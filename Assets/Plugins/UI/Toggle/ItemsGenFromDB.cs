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

    // �w�q����Ƶ��c
    public class HistoryEvent
    {
        public string name { get; set; }
        public string born { get; set; }
        public string ²�� { get; set; }
    }

    void Start()
    {
        //StartCoroutine(MakeItemTable());
    }

    public IEnumerator MakeItemTable()
    {
        // �M�� itemList ���Ҧ��l����
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }
        // �C���ˬdGlobal scene�O�_�w�[��
        yield return WaitForScene("Global", 1f);
        Debug.Log("Global scene is loaded");

        // Ū����Ʈw
        dbManager = new SQLiteManager(Path.Combine(Application.persistentDataPath, "dynamicDatabase.db"));
        List<HistoryEvent> allEvents = dbManager.QueryTable<HistoryEvent>("�d�侹");
        if (allEvents.Count == 0)
        {
            Debug.Log("Start DownloadCSV");
            yield return DownloadCSV(
                "1vT6trpTZO0gdjTGiRosRmHsd1fjzbRfBH1xLLbAHer5xWmZjglEZfYNAPUbKjp0Pj3o4et4AsS0bm-z", //sheetId
                "1874800715" //gid
                );
        }
        Db_MapToItems("�d�侹");
        itemSelector.UpdateToggleIsOn();
    }

    private IEnumerator WaitForScene(string sceneName, float interval)
    {
        // ����Global�����[��
        while (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            Debug.Log("waiting for Global scene");
            yield return new WaitForSeconds(interval); // �C�j�@�q�ɶ��ˬd�@��
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

    public void Db_MapToItems(string pageName)
    {
        // �M�� itemList ���Ҧ��l����
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        // ���Event class���o"events"��椤����ƬM�g��allEvents����
        List<HistoryEvent> allEvents = dbManager.QueryTable<HistoryEvent>(pageName);
        // �L�XallEvents���󤤪��U�C���w���e
        foreach (var eventItem in allEvents)
        {
            Debug.Log($"�m�W:{eventItem.name}, �ͤ�:{eventItem.born}, ²��:{eventItem.²��}");
            ItemGen(eventItem);
        }
    }

    private void ItemGen(HistoryEvent eventItem)
    {
        // �q itemList ���o ToggleGroup
        ToggleGroup toggleGroup = itemList.GetComponent<ToggleGroup>();
        // �ͦ��s�� Toggle �l����
        GameObject newToggle = Instantiate(itemPrefab, itemList.transform);
        // ���o�ͦ�����W�� Toggle�A�]�w Group
        Toggle ToggleComponent = newToggle.GetComponent<Toggle>();
        ToggleComponent.group = toggleGroup;

        // �]�m Toggle ���W��
        newToggle.name = $"Toggle_{eventItem.name}";

        // �]�m Toggle��LabelName���W��
        ToggleVariables toggleVar = newToggle.GetComponent<ToggleVariables>();
        toggleVar.SetLabelName(eventItem.name);

        // ��� Background �� Image �ե�
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
