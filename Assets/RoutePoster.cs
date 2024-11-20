using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;

[Serializable]
public class RequestRank
{
    public string method;
    public string typename;
    public string userID;
    public string descendingBy;
    public int lines;
    public int afterUnix;
}

[Serializable]
public class RequestDataByID
{
    public string method;
    public string typename;
    public string id;
}

[Serializable]
public class RequestDataByUserID
{
    public string method;
    public string typename;
    public string userid;
}

[Serializable]
public class RequestDataByToken
{
    public string method;
    public string typename;
    public string token;
}

[Serializable]
public class RequestDataByDataJSON
{
    public string method;
    public string typename;
    public string datajson;
}

//以代表user的Payload向Jose請求解碼真正的token
[System.Serializable]
public class RequestPayload
{
    public string payload;
    public RequestPayload(string payload) //構造函數：不用在宣告時對整包賦值，而可以傳入參數讓它在函數中賦值完再回傳
    {
        this.payload = payload;
    }
}

//取回的token包含memberId與exp，以memberId為最後的token值
[System.Serializable]
public class ResponseToken
{
    public string memberId;
    public int exp;
}

//取回的jwe
[System.Serializable]
public class ResponseJWE
{
    public string jwe;
}

public class RoutePoster : MonoBehaviour
{
    //string ParseURL = "https://play1apps.com:3001/b4a";//用letsencrypt建立的pem/key來host https server
    //string ParseURL = "https://playoneapps.com.tw:8443/b4a";//可以使用cloudflare的origin ca建立nodejs server, 但必須經由cloudflare允許的8443 port才會通
    //string ParseURL = "https://playoneapps.com.tw/b4a/";//app.post('/'必須搭配tw/b4a/，不能是tw/b4a。容易出錯，所以還是給路由名稱比較好
    private readonly string ParseURL = "https://playoneapps.com.tw/parsereq/api";

    void Start()
    {
        string url = ParseURL;
        var req = new RequestDataByUserID
        {
            method = "getobjbyuserid",
            typename = "Lb_eatall",
            userid = "mEgzD2XM3u",
        };
        string jsonData = JsonUtility.ToJson(req);
        //Debug.Log($"json={jsonData}");
        //StartCoroutine(Request(url, jsonData));//POST端口為/api
        //StartCoroutine(GetRequest("https://playoneapps.com.tw/parsereq/testGET"));//測試GET
        //TestSignInFunction(url);
    }
    // 測試 SignIn 的函數
    public void TestSignInFunction(string url)
    {
        UserData testData = new()
        {
            name = "testuser",
            avatar = 1,
            token = "example_token",
            login = 1
        };
        string dataJson = JsonUtility.ToJson(testData);
        //SignIn(url, dataJson);
    }

    // 修改後的 Request 函數
    public IEnumerator Request(string url, string jsonData, System.Action<string> onSuccess = null, System.Action<string> onError = null)
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest webRequest = new UnityWebRequest(url, "POST");
        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        webRequest.downloadHandler = new DownloadHandlerBuffer();

        // 設置必要的 headers
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("X-Requested-With", "XMLHttpRequest");

        Debug.Log("---request sending---");

        yield return webRequest.SendWebRequest();

        Debug.Log("---request completed---");

        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            string error = webRequest.downloadHandler.text;
            if (onError != null)
            {
                onError.Invoke(error);
            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
                Debug.LogError("Response Text: " + error);
            }
        }
        else
        {
            string response = webRequest.downloadHandler.text;
            if (onSuccess != null)
            {
                onSuccess.Invoke(response);
            }
            else
            {
                Debug.Log("Response: " + response);
            }
        }

        webRequest.Dispose();
    }

    IEnumerator GetRequest(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // 發送請求並等待回應
            yield return webRequest.SendWebRequest();

            // 處理回應
            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError("Connection Error: " + webRequest.error);
            }
            else if (webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Protocol Error: " + webRequest.error);
                Debug.LogError("Response Code: " + webRequest.responseCode);
                Debug.LogError("Response Text: " + webRequest.downloadHandler.text);
            }
            else
            {
                Debug.Log("Response: " + webRequest.downloadHandler.text);
            }
        }
    }
}