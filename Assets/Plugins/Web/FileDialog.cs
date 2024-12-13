using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class FileDialog : MonoBehaviour
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class OpenFileName
    {
        public int structSize = Marshal.SizeOf(typeof(OpenFileName));
        public IntPtr hwndOwner;
        public IntPtr hInstance;
        [MarshalAs(UnmanagedType.LPTStr)] public string filter;
        [MarshalAs(UnmanagedType.LPTStr)] public string customFilter;
        public int maxCustFilter;
        public int filterIndex;
        [MarshalAs(UnmanagedType.LPTStr)] public string file;
        public int maxFile = 260;
        [MarshalAs(UnmanagedType.LPTStr)] public string fileTitle;
        public int maxFileTitle = 260;
        [MarshalAs(UnmanagedType.LPTStr)] public string initialDir;
        [MarshalAs(UnmanagedType.LPTStr)] public string title;
        public int flags;
        public short fileOffset;
        public short fileExtension;
        [MarshalAs(UnmanagedType.LPTStr)] public string defExt;
        public IntPtr custData;
        public IntPtr hook;
        public IntPtr templateName;
    }

    [DllImport("comdlg32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);

    public static string ShowFileDialog()
    {
        OpenFileName openFileName = new OpenFileName
        {
            filter = "All Files\0*.*\0\0", // 過濾條件（\0 分隔多個條件，結尾需雙 \0）
            file = new string(new char[260]), // 用於儲存檔案路徑的字元陣列
            maxFile = 260, // 最大檔案路徑長度
            fileTitle = new string(new char[260]),
            maxFileTitle = 260,
            initialDir = "C:\\", // 初始目錄
            title = "Select a File", // 對話框標題
            flags = 0x00000008 | 0x00080000 | 0x00000004 // OFN_PATHMUSTEXIST | OFN_FILEMUSTEXIST | OFN_NOCHANGEDIR
        };

        if (GetOpenFileName(openFileName))
        {
            return openFileName.file; // 返回檔案路徑
        }

        return null; // 若使用者取消選擇則返回 null
    }
}
