using System.Runtime.InteropServices;
using UnityEngine;

public class UserAgentChecker : MonoBehaviour
{
    // まノ WebGL Plugin い CheckUserAgent ㄧ计
    [DllImport("__Internal")]
    private static extern int CheckUserAgent(string keyword);

    // ノ秸ノ浪代 UserAgent よ猭
    public bool Has(string keyword)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // ㊣ JS ㄧ计锣传ガ狶
        return CheckUserAgent(keyword) == 1;
#else
        Debug.LogWarning("UserAgent detection is only supported in WebGL builds.");
        return false;
#endif
    }

    // 代刚ㄧ计
    public void TestUserAgent(string keyword) //keyword ex: "Chrome"
    {
        bool contains = Has(keyword);
        Debug.Log($"UserAgent contains \"{keyword}\": {contains}");
    }
}
