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
            button.interactable = false; // �T�Ϋ��s
            yield return StartCoroutine(downloader.StartDownload(fileUrl, fileName));// �ҰʤU����{�A�õ��ݨ䧹��
            button.interactable = true;// �U�������᭫�s�ҥΫ��s
        }
    }
}
