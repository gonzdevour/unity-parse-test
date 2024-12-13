using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;

public class Uploader : MonoBehaviour
{
    public string serverUrl = "https://playoneapps.com.tw/fileparser/upload"; // 伺服器上傳路由

    // WebGL 的檔案上傳函數
    [DllImport("__Internal")]
    private static extern void UploadFileWebGL(string uploadUrl);

    // 公共上傳接口，根據運行平台選擇適當方法
    public void UploadFile()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        UploadFileForWebGL();
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
        UploadFileForPC();
#else
        Debug.LogWarning("File upload is not supported on this platform.");
#endif
    }


    // PC 平台的檔案上傳
    private void UploadFileForPC()
    {
        string filePath = FileDialog.ShowFileDialog(); // 調用檔案選擇對話框

        if (!string.IsNullOrEmpty(filePath))
        {
            StartCoroutine(UploadFileCoroutine(filePath));
        }
        else
        {
            Debug.Log("File selection cancelled.");
        }
    }

    // WebGL 平台的檔案上傳
    private void UploadFileForWebGL()
    {
        Debug.Log("Starting WebGL file upload...");
        UploadFileWebGL(serverUrl);
    }

    // 上傳檔案的協程 (適用於 PC 平台)
    private IEnumerator UploadFileCoroutine(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath); // 讀取檔案為位元組陣列
        string fileName = Path.GetFileName(filePath); // 取得檔案名稱

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, fileName); // 添加檔案數據到表單

        using (UnityWebRequest www = UnityWebRequest.Post(serverUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("File upload failed: " + www.error);
            }
            else
            {
                Debug.Log("File uploaded successfully: " + www.downloadHandler.text);
            }
        }
    }

    // WebGL 平台的上傳成功回調
    public void OnUploadComplete(string response)
    {
        Debug.Log("File uploaded successfully: " + response);
    }

    // WebGL 平台的上傳失敗回調
    public void OnUploadFailed(string error)
    {
        Debug.LogError("File upload failed: " + error);
    }
}
