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

//�H�N��user��Payload�VJose�ШD�ѽX�u����token
[System.Serializable]
public class RequestPayload
{
    public string payload;
    public RequestPayload(string payload) //�c�y��ơG���Φb�ŧi�ɹ��]��ȡA�ӥi�H�ǤJ�Ѽ������b��Ƥ���ȧ��A�^��
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

//���^��jwe
[System.Serializable]
public class ResponseJWE
{
    public string jwe;
}

public class RoutePoster : MonoBehaviour
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
        //Debug.Log($"json={jsonData}");
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
        //SignIn(url, dataJson);
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