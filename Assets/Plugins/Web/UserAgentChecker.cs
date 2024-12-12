using System.Runtime.InteropServices;
using UnityEngine;

public class UserAgentChecker : MonoBehaviour
{
    // �ޥ� WebGL Plugin ���� CheckUserAgent ���
    [DllImport("__Internal")]
    private static extern int CheckUserAgent(string keyword);

    // �Ω�ե��˴� UserAgent ����k
    public bool Has(string keyword)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // �I�s JS ��ƨ��ഫ��^�Ȭ����L��
        return CheckUserAgent(keyword) == 1;
#else
        Debug.LogWarning("UserAgent detection is only supported in WebGL builds.");
        return false;
#endif
    }

    // ���ը��
    public void TestUserAgent(string keyword) //keyword ex: "Chrome"
    {
        bool contains = Has(keyword);
        Debug.Log($"UserAgent contains \"{keyword}\": {contains}");
    }
}
