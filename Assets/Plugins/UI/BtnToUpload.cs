using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BtnToUpload : MonoBehaviour
{
    public Button button;
    public string fileUrl = "https://playoneapps.com.tw/Test/unity-parse-test/v14/StreamingAssets/test.xlsx";
    public string fileName = "test.xlsx";

    void Start()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        StartCoroutine(DownloadAndToggleButton());
    }

    private IEnumerator DownloadAndToggleButton()
    {
        var downloader = FindObjectOfType<Downloader>();
        if (downloader != null)
        {
            button.interactable = false; // 禁用按鈕
            yield return StartCoroutine(downloader.StartDownload(fileUrl, fileName));// 啟動下載協程，並等待其完成
            button.interactable = true;// 下載完成後重新啟用按鈕
        }
    }
}
