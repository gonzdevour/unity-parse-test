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

    public string serverUrl = "https://playoneapps.com.tw/fileparser/upload"; // ���A���W�Ǹ���

    // WebGL ���ɮפW�Ǩ��
    [DllImport("__Internal")]
    private static extern void UploadFileWebGL(string objectName);

    private Action<object> callback; // �Ω��x�s�̫�^�I���G���e��

    // ���@�W�Ǳ��f�A�ھڹB�業�x��ܾA���k
    // �o�̨ϥΪx�����ت��G�I�s�̥i�H���w�Ʊ���o����ƫ��O T
    public void UploadFileFromDialog<T>(Action<T> onComplete = null)
    {
        callback = result => onComplete?.Invoke((T)result); // �N���G�j���૬�� T �ð���^��

#if UNITY_WEBGL && !UNITY_EDITOR
        UploadFileForWebGL();
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
        UploadFileForPC<T>();
#else
        Debug.LogWarning("File upload is not supported on this platform.");
#endif
    }

    // PC ���x���ɮפW��
    private void UploadFileForPC<T>()
    {
        string filePath = FileDialog.ShowFileDialog(); // �ե��ɮ׿�ܹ�ܮ�

        if (!string.IsNullOrEmpty(filePath))
        {
            GetFileFromPathForPC<T>(filePath);
        }
        else
        {
            Debug.Log("File selection cancelled.");
        }
    }

    // WebGL ���x���ɮפW��
    private void UploadFileForWebGL()
    {
        Debug.Log("Starting WebGL file upload...");
        UploadFileWebGL(gameObject.name);
    }

    private void GetFileFromPathForPC<T>(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath); // Ū���ɮ׬��줸�հ}�C
        string fileName = Path.GetFileName(filePath); // ���o�ɮצW��
        string fileExtension = Path.GetExtension(filePath).ToLower(); // �ѪR���ɦW
        StartCoroutine(ReadyToUpload<T>(fileData, fileName, fileExtension));
    }

    [Serializable]
    private class FileInfoFromWebgl
    {
        public string fileData; // Base64 �s�X�����ƾ�
        public string fileName; // ���W��
        public string fileExtension; // �����ɦW
    }

    // WebGL �I�s���J�f�I (�D�x��)
    public void GetFileFromPathForWebGL(string jsonString)
    {
        try
        {
            // �ѪR JSON �ƾ�
            var fileInfo = JsonUtility.FromJson<FileInfoFromWebgl>(jsonString);
            // �N base64 �ѽX�� byte[]
            byte[] fileData = Convert.FromBase64String(fileInfo.fileData);

            Debug.Log($"File received from webgl: {fileInfo.fileName} (Extension: {fileInfo.fileExtension})");

            // �o�̵L�k�o�� T �O����A�]���@�ߨϥ� string ���^�ǫ��O
            // �N���G�浹 ReadyToUpload<string> ��ƶi��B�z
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

    // �W���ɮת���{
    // �ھ��ɮװ��ɦW�M�w�O�u�W�ѪR�٬O�����^��base64�r��
    private IEnumerator ReadyToUpload<T>(byte[] fileData, string fileName, string fileExtension)
    {
        if (ParsingOnlineList.Contains(fileExtension)) // �Y���ɦW�䴩�u�W�ѪR
        {
            // �ϥ� UploadRequest<T>�AT �M�w�O�n�r���٬O JSON ����
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
            // ���䴩�u�W�ѪR�ɡA�����^��base64�r��
            Debug.Log("File uploaded to local");
            string base64Data = Convert.ToBase64String(fileData);

            // �o�̦p�G T ���O string�A�N�|�૬���ѡA�ϥΪ̶��T�O T �O string�C
            callback?.Invoke(base64Data);
        }
    }

    public IEnumerator UploadRequest<T>(string fileName, byte[] fileData, Action<T> onSuccess = null, Action<string> onError = null)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, fileName); // �K�[�ɮ׼ƾڨ���

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
                        Debug.Log("�ɮץ�Nodejs�u�W�ѪR���\�A�^��������string");
                        response = (T)(object)responseText; // �Y T �O string
                    }
                    else
                    {
                        Debug.Log("�ɮץ�Nodejs�u�W�ѪR���\�A�^��������Json");
                        response = JsonUtility.FromJson<T>(responseText); // �Y T ���O string�A���� JSON �ϧǦC��
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

    // WebGL ���x���W�Ǧ��\�^��
    public void OnUploadComplete(string response)
    {
        Debug.Log("File uploaded successfully");
        callback?.Invoke(response); // �^�I�N�H�r��Φ��Ǧ^
    }

    // WebGL ���x���W�ǥ��Ѧ^��
    public void OnUploadFailed(string error)
    {
        Debug.LogError("File upload failed: " + error);
        // ������^�I�A�ȰO�����~
    }
}
