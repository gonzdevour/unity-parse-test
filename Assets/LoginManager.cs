using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.Net;
using System;
using UnityEditor.PackageManager;

public class LoginManager : MonoBehaviour
{
    public ParsePoster parsePoster;
    private string myToken;
    private bool IsNewUser = false;
    private readonly string ParseURL = "https://playoneapps.com.tw/parsereq/api";
    private readonly string JoseDecodeURL = "https://playoneapps.com.tw/jose/decode";

    void Start()
    {
        //PlayerPrefs.DeleteAll();
        LoginMember();
    }

    public void LoginMember()
    {
        // �P�_�O�_�b Unity �s�边��
        string payload;
        if (Application.isEditor)
        {
            payload = "eyJhbGciOiJQQkVTMi1IUzI1NitBMTI4S1ciLCJlbmMiOiJBMTI4Q0JDLUhTMjU2IiwicDJjIjoxMDAwMCwicDJzIjoiMEp0TVZqV3pQNkwwSTd4eEg1aE9tdyJ9.k-TcDWZMVgOadOwYn6f3YcV2Q4UfohX-Nf6itn60Hg36wKufr6RQ-w.eViwtmHR6YEb5AGquxs_-g.kCCvwnVmgFG8or3641Ml_hJRtG4tjvLKasNdQMwVvxKaJwqADtYVzHXq8OsjGFlEBhYw2TSyxtj-2jTtqbsaQQyCBDj4sy1FjJdFJt3H0cQ.SFTOxqPeIPtsfBy9VpbPbw"; // �w�]��
        }
        else
        {
            payload = GetPayloadFromQueryString("user"); //����ɡA�qquerystring���ouser���ѼƤ��e
        }

        // �ˬd payload �O�_����
        if (!string.IsNullOrEmpty(payload))
        {
            StartCoroutine(RequestPlayerToken(payload));
        }
        else
        {
            ShowDialog("�s�u���~�A�еy��A�աC");
        }
    }

    private IEnumerator RequestPlayerToken(string payload)
    {
        // �]�w�ШD�� URL
        string url = JoseDecodeURL;

        // �ϥ� RequestToken �]�� payload�A���ഫ�� JSON
        RequestToken requestData = new(payload);
        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log($"jsonToSend={jsonData}");

        // �ϥγq�Ϊ� Request ��Ƶo�e�ШD�A�ô��Ѧ^��
        yield return parsePoster.Request(url, jsonData,
            onSuccess: (response) =>
            {
                // �ѪR�^�Ǫ� JSON�A���� memberId �M exp
                ResponseToken responseData = JsonUtility.FromJson<ResponseToken>(response);
                if (responseData != null)
                {
                    Debug.Log("myToken: " + responseData.memberId);
                    Debug.Log("Expiration: " + responseData.exp);
                    // �NmyToken�]�w�����^��memberId
                    myToken = responseData.memberId;
                    // �q PlayerPrefs ��� playerToken
                    string playerToken = PlayerPrefs.GetString("playerToken", "");

                    if (string.IsNullOrEmpty(playerToken) || playerToken != myToken)
                    {
                        // playerToken ���ũλP myToken ���P�A���� ParseRecoverIDByToken
                        Debug.Log("PlayerPrefs ���� playerToken ���� �λP myToken ���P�A���� RecoverByToken");
                        StartCoroutine(ParseRecoverIDByToken(myToken, "UserInfo", RecoverIdByTokenSuccess));
                    }
                    else
                    {
                        // playerToken �M myToken �ۦP�A��ܤw�s�b UserID
                        Debug.Log("PlayerPrefs ���w�s�b UserID�A���� parse_getobjbyid");
                        StartCoroutine(ParseGetObjById(myToken, "UserInfo", AssignMyUserInfo)); // �^�I��Ƭ� AssignMyUserInfo
                    }
                }
                else
                {
                    ShowDialog("�L�k���o���Ī� token ��ơA�еy��A�աC");
                }
            },
            onError: (error) =>
            {
                // �B�z�ШD����
                ShowDialog("�s�u���~�A�еy��A�աC");
                Debug.LogError("Error: " + error);
            });
    }

    private void AssignMyUserInfo(string response)
    {
        // �ѪR�^�Ǫ� JSON�A���� memberId �M exp
        ResponseUserInfo responseData = JsonUtility.FromJson<ResponseUserInfo>(response);
        if (responseData != null)
        {
            Debug.Log("name: " + responseData.name);
            Debug.Log("objectId: " + responseData.objectId);
            // �ˬd objectId �O�_�s�b
            bool hasObjectId = !string.IsNullOrEmpty(responseData.objectId);
            if (hasObjectId)
            {
                // �N����л\ PlayerPrefs
                UpdatePlayerPrefsFromJson(JsonUtility.ToJson(responseData));
                RecoverParseRequestEnergy();
                DataUpdateRequest();

                if (PlayerPrefs.GetString("myPlayerName").Length < 2)
                {
                    IsNewUser = true;
                }
            }
            else
            {
                Debug.Log($"���^��UserInfo����objectId���šA���� RecoverByToken");
                StartCoroutine(ParseRecoverIDByToken(myToken, "UserInfo", RecoverIdByTokenSuccess));
            }
        }
        else
        {
            ShowDialog("�L�k���o���Ī� user ��ơA�еy��A�աC");
        }
    }

    private void RecoverIdByTokenSuccess(string response)
    {
        // �ѪR�^�Ǫ� JSON�A���� memberId �M exp
        ResponseUserInfo responseData = JsonUtility.FromJson<ResponseUserInfo>(response);
        if (responseData != null)
        {
            Debug.Log("name: " + responseData.name);
            Debug.Log("objectId: " + responseData.objectId);
            // �ˬd objectId �O�_�s�b
            bool hasObjectId = !string.IsNullOrEmpty(responseData.objectId);
            if (hasObjectId)
            {
                string userid = responseData.objectId;
                // �N����л\ PlayerPrefs
                UpdatePlayerPrefsFromJson(JsonUtility.ToJson(responseData));
                RecoverParseRequestEnergy();
                StartCoroutine(ParseGetObjById(userid, "Lb_eatall", ParseGetMyRecord)); //���o�ۤv��leaderboard

                if (PlayerPrefs.GetString("myPlayerName").Length < 2)
                {
                    IsNewUser = true;
                }
            }
            else
            {
                Debug.Log($"Recover���^��UserInfo����objectId���šA���� SignIn");
                UserData testData = new()
                {
                    name = "gd_unity_01",
                    avatar = 1,
                    token = myToken,
                    login = 1
                };
                SignIn(ParseURL, JsonUtility.ToJson(testData));
            }
        }
        else
        {
            ShowDialog("�L�k���o���Ī� user ��ơA�еy��A�աC");
        }
    }

    // SignIn ��� (�եέ�l�� SignIn �N�X)
    public void SignIn(string url, string data)
    {
        RequestDataByDataJSON req = new()
        {
            method = "pushobj",
            typename = "UserInfo",
            datajson = data,
        };
        string jsonData = JsonUtility.ToJson(req);

        StartCoroutine(parsePoster.Request(url, jsonData,
            onSuccess: (response) =>
            {
                ResponseUserInfo responseData = JsonUtility.FromJson<ResponseUserInfo>(response);
                if (responseData != null)
                {
                    Debug.Log("Sign-in successful. Response: " + response);
                    // �N����л\ PlayerPrefs
                    UpdatePlayerPrefsFromJson(JsonUtility.ToJson(responseData));
                    IsNewUser = true;
                    RecoverParseRequestEnergy();
                    DataUpdateRequest();
                }
                else
                {
                    Debug.LogError("Sign-in successful, but Response is null");
                    ShowDialog("�s�u���~�A�еy��A�աC");
                }
            },
            onError: (error) =>
            {
                Debug.LogError("Sign-in failed. Error: " + error);
                // �b�o�̳B�z���~�޿�
                if (IsNetworkConnected())
                {
                    ShowDialog("�s�u���~�A�еy��A�աC");
                }
                else
                {
                    ShowDialog("���ˬd�����s�u�C");
                }
            }));
    }

    private IEnumerator ParseGetObjById(string objectId, string typename, System.Action<string> callback)
    {
        // �]�w�ШD�� URL
        string url = ParseURL;

        // �ϥ� RequestToken �]�� payload�A���ഫ�� JSON
        RequestDataByID requestData = new()
        {
            method = "getobjbyid",
            typename = typename,
            id = objectId,
        };
        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log($"jsonToSend={jsonData}");

        // �ϥγq�Ϊ� Request ��Ƶo�e�ШD�A�ô��Ѧ^��
        yield return parsePoster.Request(url, jsonData,
            onSuccess: (response) =>
            {
                callback(response);
            },
            onError: (error) =>
            {
                // �B�z�ШD����
                ShowDialog("�s�u���~�A�еy��A�աC");
                Debug.LogError("GetObjByID_Error: " + error);
            });
    }

    private IEnumerator ParseRecoverIDByToken(string token, string typename, System.Action<string> callback)
    {
        // �]�w�ШD�� URL
        string url = ParseURL;

        // �ϥ� RequestToken �]�� payload�A���ഫ�� JSON
        RequestDataByToken requestData = new()
        {
            method = "getobjbyid",
            typename = typename,
            token = token,
        };
        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log($"jsonToSend={jsonData}");

        // �ϥγq�Ϊ� Request ��Ƶo�e�ШD�A�ô��Ѧ^��
        yield return parsePoster.Request(url, jsonData,
            onSuccess: (response) =>
            {
                callback(response);
            },
            onError: (error) =>
            {
                // �B�z�ШD����
                ShowDialog("�s�u���~�A�еy��A�աC");
                Debug.LogError("RecoverIDByToken_Error: " + error);
            });
    }

    private void ParseGetMyRecord(string jsonResponse)
    {
        // �ˬd�^�Ǹ�ƬO�_����
        if (string.IsNullOrEmpty(jsonResponse))
        {
            PlayerPrefs.SetInt("score", 0);
        }
        else
        {
            // ��s����
            int score = ExtractScoreFromJson(jsonResponse);
            PlayerPrefs.SetInt("score", score);
        }

        if (PlayerPrefs.GetString("myPlayerName").Length < 2)
        {
            IsNewUser = true;
        }

        DataUpdateRequest();
    }

    private void DataUpdateRequest()
    {
        // �����ƾڧ�s�ШD�A���]���\
        if (IsNewUser)
        {
            PlayerPrefs.DeleteKey("playerName");
            GoToScene("selectScene");
        }
        else
        {
            GoToScene("titleScene");
        }
    }

    // �H�U�����U��k
    private string GetPayloadFromQueryString(string key)
    {
        // �������o URL Query String �����Ѽ�
        return "someQueryValue";
    }

    private void ShowDialog(string message)
    {
        Debug.Log("��ܰT��: " + message);
    }

    private bool CheckUserAgentForDcard()
    {
        // ���] UserAgent �ˬd
        return false;
    }

    private void UpdatePlayerPrefsFromJson(string json)
    {
        // ������s PlayerPrefs
        PlayerPrefs.SetString("playerData", json);
    }

    private int ExtractScoreFromJson(string json)
    {
        // �����q JSON ������ score
        return 100;
    }

    private void GoToScene(string sceneName)
    {
        Debug.Log("�e������: " + sceneName);
    }

    private bool IsNetworkConnected()
    {
        // ���������s�u���A
        return true;
    }

    private void RecoverParseRequestEnergy()
    {
        Debug.Log("�I�s RecoverParseRequestEnergy()");
    }

    private void ParseGetObjByUserId(string userId, System.Action<string> callback)
    {
        // �����ШD���o�Τ���
        callback.Invoke("{\"score\":100}");
    }
}
