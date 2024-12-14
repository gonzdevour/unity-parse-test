using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;

public class Uploader : MonoBehaviour
{
    public string serverUrl = "https://playoneapps.com.tw/fileparser/upload"; // ���A���W�Ǹ���

    // WebGL ���ɮפW�Ǩ��
    [DllImport("__Internal")]
    private static extern void UploadFileWebGL(string uploadUrl, string objectName);

    private Action<string> callback; // �Ω�s�x�^�ը��

    // ���@�W�Ǳ��f�A�ھڹB�業�x��ܾA���k
    public void UploadFile(Action<string> onComplete = null)
    {
        callback = onComplete; // �]�m�^�ը��

#if UNITY_WEBGL && !UNITY_EDITOR
        UploadFileForWebGL();
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
        UploadFileForPC();
#else
        Debug.LogWarning("File upload is not supported on this platform.");
#endif
    }

    // PC ���x���ɮפW��
    private void UploadFileForPC()
    {
        string filePath = FileDialog.ShowFileDialog(); // �ե��ɮ׿�ܹ�ܮ�

        if (!string.IsNullOrEmpty(filePath))
        {
            StartCoroutine(GetFileFromPath(filePath));
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
        UploadFileWebGL(serverUrl, gameObject.name);
    }

    // �W���ɮת���{ (�A�Ω� PC ���x)
    private IEnumerator GetFileFromPath(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath); // Ū���ɮ׬��줸�հ}�C
        string fileName = Path.GetFileName(filePath); // ���o�ɮצW��
        yield return UploadRequest(fileName, fileData);
    }

    public IEnumerator UploadRequest(string fileName, byte[] fileData, Action<String> onSuccess = null, Action<String> onError = null)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, fileName); // �K�[�ɮ׼ƾڨ���

        using (UnityWebRequest www = UnityWebRequest.Post(serverUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text;
                Debug.Log("File uploaded successfully");
                callback?.Invoke(response); // UploadFile���\�ɰ���^�I
                onSuccess?.Invoke(response);
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
        callback?.Invoke(response); // �Ȧb���\�ɰ���^�I
    }

    // WebGL ���x���W�ǥ��Ѧ^��
    public void OnUploadFailed(string error)
    {
        Debug.LogError("File upload failed: " + error);
        // ������^�I�A�ȰO�����~
    }
}
