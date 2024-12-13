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
            filter = "All Files\0*.*\0\0", // �L�o����]\0 ���j�h�ӱ���A�������� \0�^
            file = new string(new char[260]), // �Ω��x�s�ɮ׸��|���r���}�C
            maxFile = 260, // �̤j�ɮ׸��|����
            fileTitle = new string(new char[260]),
            maxFileTitle = 260,
            initialDir = "C:\\", // ��l�ؿ�
            title = "Select a File", // ��ܮؼ��D
            flags = 0x00000008 | 0x00080000 | 0x00000004 // OFN_PATHMUSTEXIST | OFN_FILEMUSTEXIST | OFN_NOCHANGEDIR
        };

        if (GetOpenFileName(openFileName))
        {
            return openFileName.file; // ��^�ɮ׸��|
        }

        return null; // �Y�ϥΪ̨�����ܫh��^ null
    }
}
