using System.Runtime.InteropServices;
using UnityEngine;

public class QueryStringParser : MonoBehaviour
{
    // �ޥ� WebGL Plugin ���� GetQueryStringValue ���
    [DllImport("__Internal")]
    private static extern System.IntPtr GetQueryStringValue(string paramName);

    /// <summary>
    /// ������w Query String �Ѽƪ���
    /// </summary>
    /// <param name="paramName">���}�W���ѼƦW��</param>
    /// <returns>�Ѽƪ��ȡ]�p�G���s�b�A��^�Ŧr��^</returns>
    public string Get(string paramName)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // �I�s JS ��ƨñN���w�ഫ���r�Ŧ�
        System.IntPtr valuePtr = GetQueryStringValue(paramName);
        string value = Marshal.PtrToStringAuto(valuePtr);
        return value;
#else
        Debug.LogWarning("Query string handling is only supported in WebGL builds.");
        return string.Empty;
#endif
    }

    // ���ը��
    public void TestQueryString()
    {
        string paramName = "user";
        string paramValue = Get(paramName);
        Debug.Log($"Query string value for '{paramName}': {paramValue}");
    }
}
