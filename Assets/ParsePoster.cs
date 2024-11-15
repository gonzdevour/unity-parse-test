using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;
using Newtonsoft.Json;

[Serializable]
public class ResponseUserInfo
{
    public string objectId;
    public string name;
    public string token;
    public int avatar;
    public int login;
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
public class RequestDataByDataJSON
{
    public string method;
    public string typename;
    public string datajson;
}

[System.Serializable]
public class UserData
{
    public string name;
    public int avatar;
    public string token;
    public int login;
}

//�H�N��user��Payload�VJose�ШD�ѽX�u����token
[System.Serializable]
public class RequestToken
{
    public string payload;
    public RequestToken(string payload)
    {
        this.payload = payload;
    }
}
//���^��token�]�tmemberId�Pexp�A�HmemberId���̫᪺token��
[System.Serializable]
public class ResponseToken
{
    public string memberId;
    public int exp;
}

public class ParsePoster : MonoBehaviour
{
    //string ParseURL = "https://play1apps.com:3001/b4a";//��letsencrypt�إߪ�pem/key��host https server
    //string ParseURL = "https://playoneapps.com.tw:8443/b4a";//�i�H�ϥ�cloudflare��origin ca�إ�nodejs server, �������g��cloudflare���\��8443 port�~�|�q
    //string ParseURL = "https://playoneapps.com.tw/b4a/";//app.post('/'�����f�ttw/b4a/�A����Otw/b4a�C�e���X���A�ҥH�٬O�����ѦW�٤���n
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
        Debug.Log($"json={jsonData}");
        //StartCoroutine(Request(url, jsonData));//POST�ݤf��/api
        //StartCoroutine(GetRequest("https://playoneapps.com.tw/parsereq/testGET"));//����GET
        //TestSignInFunction(url);
    }
    // ���� SignIn �����
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
        SignIn(url, dataJson);
    }

    // SignIn ��� (�եέ�l�� SignIn �N�X)
    public void SignIn(string url, string data)
    {
        var req = new RequestDataByDataJSON
        {
            method = "pushobj",
            typename = "UserInfo",
            datajson = data,
        };
        string jsonData = JsonUtility.ToJson(req);

        StartCoroutine(Request(url, jsonData,
            onSuccess: (response) =>
            {
                Debug.Log("Sign-in successful. Response: " + response);
                // �b�o�̳B�z���\���n�J�޿�
            },
            onError: (error) =>
            {
                Debug.LogError("Sign-in failed. Error: " + error);
                // �b�o�̳B�z���~�޿�
            }));
    }

    // �ק�᪺ Request ���
    public IEnumerator Request(string url, string jsonData, System.Action<string> onSuccess = null, System.Action<string> onError = null)
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest webRequest = new UnityWebRequest(url, "POST");
        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        webRequest.downloadHandler = new DownloadHandlerBuffer();

        // �]�m���n�� headers
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("X-Requested-With", "XMLHttpRequest");

        Debug.Log("Sending request...");

        yield return webRequest.SendWebRequest();

        Debug.Log("Request completed");

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
            // �o�e�ШD�õ��ݦ^��
            yield return webRequest.SendWebRequest();

            // �B�z�^��
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