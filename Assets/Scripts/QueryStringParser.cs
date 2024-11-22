using System.Runtime.InteropServices;
using UnityEngine;

public class QueryStringParser : MonoBehaviour
{
    // まノ WebGL Plugin い GetQueryStringValue ㄧ计
    [DllImport("__Internal")]
    private static extern System.IntPtr GetQueryStringValue(string paramName);

    /// <summary>
    /// 莉﹚ Query String 把计
    /// </summary>
    /// <param name="paramName">呼把计嘿</param>
    /// <returns>把计狦ぃ﹃</returns>
    public string Get(string paramName)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // ㊣ JS ㄧ计盢皐锣传才﹃
        System.IntPtr valuePtr = GetQueryStringValue(paramName);
        string value = Marshal.PtrToStringAuto(valuePtr);
        return value;
#else
        Debug.LogWarning("Query string handling is only supported in WebGL builds.");
        return string.Empty;
#endif
    }

    // 代刚ㄧ计
    public void TestQueryString()
    {
        string paramName = "user";
        string paramValue = Get(paramName);
        Debug.Log($"Query string value for '{paramName}': {paramValue}");
    }
}
