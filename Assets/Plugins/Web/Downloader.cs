using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class Downloader : MonoBehaviour
{
    public IEnumerator StartDownload(string url, string fileName)
    {
        Debug.Log($"StartDownload called with URL: {url}, FileName: {fileName}");
        yield return DownloadFileCoroutine(url, fileName);
    }

    private IEnumerator DownloadFileCoroutine(string url, string fileName)
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        // �u�X�O�s����ܮ�
        string savePath = ShowSaveFileDialog(fileName);
        if (string.IsNullOrEmpty(savePath))
        {
            Debug.LogWarning("User canceled the save dialog.");
            yield break; // ���T�B�z���e�h�X�A�O�Ҫ�^��
        }
        Debug.Log($"User selected save path: {savePath}");
#else
    string savePath = GetSavePath(fileName);
    Debug.Log($"Save path determined: {savePath}");
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
    Debug.Log("WebGL platform detected, using WebGLDownloadFile.");
    WebGLDownloadFile(url, fileName);
    yield break; // �O�Ҩ�{�����T�������I
#else
        Debug.Log("Starting UnityWebRequest for file download...");
        using (UnityWebRequest www = new UnityWebRequest(url))
        {
            www.downloadHandler = new DownloadHandlerFile(savePath);
            Debug.Log("UnityWebRequest configured with DownloadHandlerFile.");

            yield return www.SendWebRequest();

            Debug.Log("UnityWebRequest completed.");

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Download failed: {www.error}");
            }
            else
            {
                Debug.Log($"File downloaded successfully to: {savePath}");
            }
        }
#endif
        yield break; // �K�[��{�������A�����^��
    }

    private static string GetSavePath(string fileName)
    {
#if UNITY_ANDROID || UNITY_IOS
        string path = Path.Combine(Application.persistentDataPath, fileName);
        Debug.Log($"Mobile platform detected. SavePath: {path}");
        return path;
#elif UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log($"WebGL platform detected. FileName: {fileName}");
        return fileName; // WebGL ���ݭn���a���|
#else
        string path = Path.Combine(Application.persistentDataPath, fileName);
        Debug.Log($"Non-mobile platform detected. SavePath: {path}");
        return path;
#endif
    }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class OPENFILENAME
    {
        public int lStructSize = Marshal.SizeOf(typeof(OPENFILENAME));
        public IntPtr hwndOwner;
        public IntPtr hInstance;
        public string lpstrFilter;
        public string lpstrCustomFilter;
        public int nMaxCustFilter;
        public int nFilterIndex;
        public string lpstrFile;
        public int nMaxFile = 260; // MAX_PATH
        public string lpstrFileTitle;
        public int nMaxFileTitle = 260; // MAX_PATH
        public string lpstrInitialDir;
        public string lpstrTitle;
        public int Flags;
        public short nFileOffset;
        public short nFileExtension;
        public string lpstrDefExt;
        public IntPtr lCustData;
        public IntPtr lpfnHook;
        public IntPtr lpTemplateName;
    }

    [DllImport("comdlg32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool GetSaveFileName([In, Out] OPENFILENAME ofn);

    private static string ShowSaveFileDialog(string defaultFileName)
    {
        string originalDirectory = Directory.GetCurrentDirectory(); // �O�s��l�u�@�ؿ�
        try
        {
            string extension = Path.GetExtension(defaultFileName);
            string filter = $"{extension} Files (*{extension})\0*{extension}\0All Files (*.*)\0*.*\0";

            OPENFILENAME ofn = new OPENFILENAME
            {
                lpstrFilter = filter, // ���L�o��
                lpstrFile = defaultFileName + new string(' ', 260 - defaultFileName.Length), // ��l�ƽw�İϨö�J�q�{�ɦW
                lpstrFileTitle = new string(new char[260]),
                lpstrTitle = "Select Save Location", // ��ܮؼ��D
                lpstrInitialDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop), // ��l�ؿ�
                lpstrDefExt = extension?.TrimStart('.') ?? "", // �q�{�X�i�W
                Flags = 0x00000002 | 0x00080000 // OFN_OVERWRITEPROMPT | OFN_NOCHANGEDIR
            };

            // ��ܫO�s��ܮ�
            if (GetSaveFileName(ofn))
            {
                Debug.Log($"User selected save path: {ofn.lpstrFile}");
                return ofn.lpstrFile;
            }
            else
            {
                Debug.LogWarning("Save dialog was canceled or failed.");
                return null;
            }
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDirectory); // ��_��l�u�@�ؿ�
        }
    }
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void WebGLDownloadFile(string url, string fileName);
#endif
}
