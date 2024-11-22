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
    public void TestUserAgent()
    {
        string keyword = "Chrome";
        bool contains = Has(keyword);
        Debug.Log($"UserAgent contains \"{keyword}\": {contains}");

        System.Text.StringBuilder randomString = new System.Text.StringBuilder(550); // 箇砞甧秖菠500箇痙传︽才
        for (int i = 0; i < 500; i++)
        {
            int randomDigit = Random.Range(0, 10); // 玻ネ09ぇ丁繦诀计
            randomString.Append(randomDigit);

            // –10计睰传︽才
            if ((i + 1) % 10 == 0)
            {
                randomString.Append("\n");
            }
        }
        Debug.Log(randomString.ToString());


    }
}
