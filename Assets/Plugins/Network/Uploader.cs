using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Uploader : MonoBehaviour
{
    public static Uploader Inst { get; private set; }
    private void Awake()
    {
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public string serverUrl = "https://playoneapps.com.tw/fileparser/upload"; // 伺服器上傳路由

    // WebGL 的檔案上傳函數
    [DllImport("__Internal")]
    private static extern void UploadFileWebGL(string objectName);

    private Action<object> callback; // 用於儲存最後回呼結果的委派

    // 公共上傳接口，根據運行平台選擇適當方法
    // 這裡使用泛型的目的：呼叫者可以指定希望獲得的資料型別 T
    public void UploadFileFromDialog<T>(Action<T> onComplete = null)
    {
        callback = result => onComplete?.Invoke((T)result); // 將結果強制轉型為 T 並執行回調

#if UNITY_WEBGL && !UNITY_EDITOR
        UploadFileForWebGL();
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
        UploadFileForPC<T>();
#else
        Debug.LogWarning("File upload is not supported on this platform.");
#endif
    }

    // PC 平台的檔案上傳
    private void UploadFileForPC<T>()
    {
        string filePath = FileDialog.ShowFileDialog(); // 調用檔案選擇對話框

        if (!string.IsNullOrEmpty(filePath))
        {
            GetFileFromPathForPC<T>(filePath);
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
        UploadFileWebGL(gameObject.name);
    }

    private void GetFileFromPathForPC<T>(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath); // 讀取檔案為位元組陣列
        string fileName = Path.GetFileName(filePath); // 取得檔案名稱
        string fileExtension = Path.GetExtension(filePath).ToLower(); // 解析副檔名
        StartCoroutine(ReadyToUpload<T>(fileData, fileName, fileExtension));
    }

    [Serializable]
    private class FileInfoFromWebgl
    {
        public string fileData; // Base64 編碼的文件數據
        public string fileName; // 文件名稱
        public string fileExtension; // 文件副檔名
    }

    // WebGL 呼叫的入口點 (非泛型)
    public void GetFileFromPathForWebGL(string jsonString)
    {
        try
        {
            // 解析 JSON 數據
            var fileInfo = JsonUtility.FromJson<FileInfoFromWebgl>(jsonString);
            // 將 base64 解碼為 byte[]
            byte[] fileData = Convert.FromBase64String(fileInfo.fileData);

            Debug.Log($"File received from webgl: {fileInfo.fileName} (Extension: {fileInfo.fileExtension})");

            // 這裡無法得知 T 是什麼，因此一律使用 string 為回傳型別
            // 將結果交給 ReadyToUpload<string> 函數進行處理
            StartCoroutine(ReadyToUpload<string>(fileData, fileInfo.fileName, fileInfo.fileExtension));
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error processing file from webgl: {ex.Message}");
        }
    }

    private static readonly HashSet<string> ParsingOnlineList = new HashSet<string>
    {
        ".docx",
        ".xlsx",
    };

    // 上傳檔案的協程
    // 根據檔案副檔名決定是線上解析還是直接回傳base64字串
    private IEnumerator ReadyToUpload<T>(byte[] fileData, string fileName, string fileExtension)
    {
        if (ParsingOnlineList.Contains(fileExtension)) // 若副檔名支援線上解析
        {
            // 使用 UploadRequest<T>，T 決定是要字串還是 JSON 類型
            yield return UploadRequest<T>(fileName, fileData, (T response) =>
            {
                callback?.Invoke(response);
            }, error =>
            {
                Debug.LogError($"Upload failed with error: {error}");
            });
        }
        else
        {
            // 不支援線上解析時，直接回傳base64字串
            Debug.Log("File uploaded to local");
            string base64Data = Convert.ToBase64String(fileData);

            // 這裡如果 T 不是 string，將會轉型失敗，使用者須確保 T 是 string。
            callback?.Invoke(base64Data);
        }
    }

    public IEnumerator UploadRequest<T>(string fileName, byte[] fileData, Action<T> onSuccess = null, Action<string> onError = null)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, fileName); // 添加檔案數據到表單

        using (UnityWebRequest www = UnityWebRequest.Post(serverUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string responseText = www.downloadHandler.text;
                    T response;

                    if (typeof(T) == typeof(string))
                    {
                        Debug.Log("檔案由Nodejs線上解析成功，回傳類型為string");
                        response = (T)(object)responseText; // 若 T 是 string
                    }
                    else
                    {
                        Debug.Log("檔案由Nodejs線上解析成功，回傳類型為Json");
                        response = JsonUtility.FromJson<T>(responseText); // 若 T 不是 string，嘗試 JSON 反序列化
                    }
                    onSuccess?.Invoke(response);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to parse response: {ex.Message}");
                    onError?.Invoke($"Parsing error: {ex.Message}");
                }
            }
            else
            {
                Debug.LogError("File upload failed: " + www.error);
                onError?.Invoke(www.error);
            }
        }
    }

    // WebGL 平台的上傳成功回調
    public void OnUploadComplete(string response)
    {
        Debug.Log("File uploaded successfully");
        callback?.Invoke(response); // 回呼將以字串形式傳回
    }

    // WebGL 平台的上傳失敗回調
    public void OnUploadFailed(string error)
    {
        Debug.LogError("File upload failed: " + error);
        // 不執行回呼，僅記錄錯誤
    }
}
