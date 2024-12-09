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
    public void TestUserAgent()
    {
        string keyword = "Chrome";
        bool contains = Has(keyword);
        Debug.Log($"UserAgent contains \"{keyword}\": {contains}");

        System.Text.StringBuilder randomString = new System.Text.StringBuilder(550); // �w�]�e�q���j��500�A�w�d�����
        for (int i = 0; i < 500; i++)
        {
            int randomDigit = Random.Range(0, 10); // ����0��9�������H���Ʀr
            randomString.Append(randomDigit);

            // �C10�ӼƦr��K�[�����
            if ((i + 1) % 10 == 0)
            {
                randomString.Append("\n");
            }
        }
        Debug.Log(randomString.ToString());


    }
}
