using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using System;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine.SocialPlatforms.Impl;
// using System.Web;//�쥻�ΨӸѪRUserAgent�A���o�O��ݥΪ�lib�A�����ϥ�webgl plugin�����qjs����

public class LoginManager : MonoBehaviour
{
    public bool CleanStorageAtStart;
    public UserAgentChecker userAgentChecker;
    public QueryStringParser queryStringParser;
    public UserData userDataManager;
    public RoutePoster routePoster;
    private int loginProgress;
    private string myToken;
    private bool IsNewUser = false;
    private readonly string GameName = "eatall";
    private readonly string ParseURL = "https://playoneapps.com.tw/parsereq/api";
    private readonly string JoseEncodeURL = "https://playoneapps.com.tw/jose/encode";
    private readonly string JoseDecodeURL = "https://playoneapps.com.tw/jose/decode";

    private bool IsOnWeb;
    private bool IsOnline
    {
        get
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return false;
            }
            return true;
        }
    }

    private void Awake()
    {
        IsOnWeb = !string.IsNullOrEmpty(Application.absoluteURL);

        if (CleanStorageAtStart)
        {
            PlayerPrefs.DeleteAll();
        }
    }

    void Start()
    {
        //StartCoroutine(RequestEncodeTokenToPayload("test-encode-ab1234"));//����jose encode
        //Debug.Log(GetPayloadFromQueryString("user"));//����System.Web�ѪRquerystring
        //Debug.Log("Login Start");
        //LoginCheckPayload();
    }

    public void LoginCheckPayload()
    {
        string payload = "";
        if (Application.isEditor) // �b Unity �s�边���A���խ�
        {
            payload = "eyJhbGciOiJQQkVTMi1IUzI1NitBMTI4S1ciLCJlbmMiOiJBMTI4Q0JDLUhTMjU2IiwicDJjIjoxMDAwMCwicDJzIjoiMEp0TVZqV3pQNkwwSTd4eEg1aE9tdyJ9.k-TcDWZMVgOadOwYn6f3YcV2Q4UfohX-Nf6itn60Hg36wKufr6RQ-w.eViwtmHR6YEb5AGquxs_-g.kCCvwnVmgFG8or3641Ml_hJRtG4tjvLKasNdQMwVvxKaJwqADtYVzHXq8OsjGFlEBhYw2TSyxtj-2jTtqbsaQQyCBDj4sy1FjJdFJt3H0cQ.SFTOxqPeIPtsfBy9VpbPbw"; // �w�]��
        }
        else if (IsOnWeb) // �bweb����A�qquerystring���ouser���ѼƤ��e
        {
            payload = queryStringParser.Get("user");
        }

        // �ˬd payload �O�_����
        if (!string.IsNullOrEmpty(payload))
        {
            UpdateDataLoadingTask(1);//1. �T�{payload����
            StartCoroutine(RequestDecodePayloadToToken(payload));
        }
        else
        {
            ShowDialog("�n�J�v�����`�A�еy��A�աC");
        }
    }

    private IEnumerator RequestEncodeTokenToPayload(string payload)
    {
        // �]�w�ШD�� URL
        string url = JoseEncodeURL;

        // �ϥ� RequestPayload �]�� payload�A���ഫ�� JSON
        RequestPayload requestData = new(payload);
        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log("RequestEncodeTokenToPayload Start");
        Debug.Log($"jsonToSend={jsonData}");

        // �ϥγq�Ϊ� Request ��Ƶo�e�ШD�A�ô��Ѧ^��
        yield return routePoster.Request(url, jsonData,
            onSuccess: (response) =>
            {
                ResponseJWE responseData = JsonUtility.FromJson<ResponseJWE>(response);
                if (responseData != null)
                {
                    Debug.Log("RequestEncodeTokenToPayload Success");
                    Debug.Log(responseData.jwe);
                    UpdateDataLoadingTask(1);//1. �T�{payload�G��token���^�Fpayload
                }
                else
                {
                    ShowDialog("�L�k���o�[�Ktoken�A�еy��A�աC");
                }
            },
            onError: (error) =>
            {
                // �B�z�ШD����
                ShowDialog("�s�u���~�A�еy��A�աC");
                Debug.LogError("Error: " + error);
            });
    }

    private IEnumerator RequestDecodePayloadToToken(string payload)
    {
        // �]�w�ШD�� URL
        string url = JoseDecodeURL;

        // �ϥ� RequestPayload �]�� payload�A���ഫ�� JSON
        RequestPayload requestData = new(payload);
        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log("RequestDecodePayloadToToken Start");
        Debug.Log($"jsonToSend={jsonData}");

        // �ϥγq�Ϊ� Request ��Ƶo�e�ШD�A�ô��Ѧ^��
        yield return routePoster.Request(url, jsonData,
            onSuccess: (response) =>
            {
                UpdateDataLoadingTask(1);//2. �T�{token:��payload���^�Ftoken
                // �ѪR�^�Ǫ� JSON�A���� memberId �M exp
                ResponseToken responseData = JsonUtility.FromJson<ResponseToken>(response);
                if (responseData != null)
                {
                    Debug.Log("RequestDecodePayloadToToken Success");
                    Debug.Log("token: " + responseData.memberId);
                    Debug.Log("Expiration: " + responseData.exp);
                    // �NmyToken�]�w�����^��memberId
                    myToken = responseData.memberId;

                    // �P�_PlayerPrefs��token�O�_�s�b
                    string playerToken = PlayerPrefs.GetString("token", "");
                    if (string.IsNullOrEmpty(playerToken) || playerToken != myToken)
                    {
                        // playerToken ���ũλP myToken ���P�A���� ParseRecoverIDByToken
                        Debug.Log("PlayerPrefs ���� playerToken ���� �λP myToken ���P�A���� RecoverByToken");
                        StartCoroutine(ParseRecoverIDByToken(myToken, "UserInfo", ParseRecoverIdByTokenSuccess));
                    }
                    else
                    {
                        // playerToken �M myToken �ۦP�A��ܤw�s�b userID�AuserID�Y��UserInfo����objectId�A�ҥH�ϥ�ParseGetObjById�ӫDParseGetObjByUserId
                        string userID = PlayerPrefs.GetString("userID", "");
                        Debug.Log($"PlayerPrefs���w�s�b userID={userID}�A����parse_getobjbyid");
                        StartCoroutine(ParseGetObjById(userID, "UserInfo", ParseGetUserInfoByIdSuccess)); // �^�I��Ƭ� ParseGetUserInfoByIdSuccess
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

    private void ParseGetUserInfoByIdSuccess(string response) //�Υ��auserID�n�J���\
    {
        // �ѪR�^�Ǫ� JSON�A���� memberId �M exp
        UserData responseData = JsonUtility.FromJson<UserData>(response);
        if (responseData != null)
        {
            Debug.Log("name: " + responseData.name);
            Debug.Log("objectId: " + responseData.objectId);
            // �ˬd objectId �O�_�s�b
            bool hasObjectId = !string.IsNullOrEmpty(responseData.objectId);
            if (hasObjectId)
            {
                UpdateDataLoadingTask(1);//3. �T�{�ϥΪ̸��: ��userID���^�FUserInfo�A���PlayerPref�����a��Ʀs�b
                UpdateDataLoadingTask(1);//4. �T�{�ϥΪ̦��Z: PlayerPref�����a��Ʀs�b�A�]��score��Ƥ]�s�b
                // �N����л\ PlayerPrefs
                userDataManager.Update(responseData);
                // �u��signIn��recoverId���\�~���\�L������է�sleaderboard�A���auserID�s��n�J�h���ӭn������
                RecoverParseRequestEnergy();
                // ��sleaderboard
                DataUpdateCheck();
            }
            else
            {
                Debug.Log($"���^��UserInfo����objectId���šA���� RecoverByToken");
                StartCoroutine(ParseRecoverIDByToken(myToken, "UserInfo", ParseRecoverIdByTokenSuccess));
            }
        }
        else
        {
            ShowDialog("�L�k���o���Ī� user ��ơA�еy��A�աC");
        }
    }

    private void ParseRecoverIdByTokenSuccess(string response)
    {
        // �ѪR�^�Ǫ� JSON�A���� memberId �M exp
        UserData responseData = JsonUtility.FromJson<UserData>(response);
        if (responseData != null)
        {
            Debug.Log("name: " + responseData.name);
            Debug.Log("objectId: " + responseData.objectId);
            // �ˬd objectId �O�_�s�b
            bool hasObjectId = !string.IsNullOrEmpty(responseData.objectId);
            if (hasObjectId)
            {
                UpdateDataLoadingTask(1);//3. �T�{�ϥΪ̸��: ��token���^�FUserInfo�A�N����л\ PlayerPrefs

                string userID = responseData.objectId;
                // �N����л\ PlayerPrefs
                userDataManager.Update(responseData);
                StartCoroutine(ParseGetObjByUserID(userID, "Lb_"+ GameName, ParseGetMyRecord)); //���o�ۤv��leaderboard
            }
            else
            {
                Debug.Log($"Recover���^��UserInfo����objectId���šA���� SignIn");
                UserData signInData = new()
                {
                    token = PlayerPrefs.GetString("token", myToken),
                    name = PlayerPrefs.GetString("name", ""),
                    avatar = PlayerPrefs.GetInt("avatar", 1),
                    login = PlayerPrefs.GetInt("login", 1),
                };
                SignIn(ParseURL, JsonUtility.ToJson(signInData));
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

        StartCoroutine(routePoster.Request(url, jsonData,
            onSuccess: (response) =>
            {
                UserData responseData = JsonUtility.FromJson<UserData>(response);
                if (responseData != null)
                {
                    Debug.Log("Sign-in successful. Response: " + response);
                    // signIn���\���w�O�s���a
                    IsNewUser = true;
                    // �N����л\ PlayerPrefs
                    userDataManager.Update(responseData);
                    // �u��signIn��recoverId���\�~���\�L������է�sleaderboard�A���auserID�s��n�J�h���ӭn������
                    RecoverParseRequestEnergy();
                    // ��sleaderboard
                    DataUpdateCheck();
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
                if (IsOnline)
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

        // �ϥ� RequestPayload �]�� payload�A���ഫ�� JSON
        RequestDataByID requestData = new()
        {
            method = "getobjbyid",
            typename = typename,
            id = objectId,
        };
        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log($"jsonToSend={jsonData}");

        // �ϥγq�Ϊ� Request ��Ƶo�e�ШD�A�ô��Ѧ^��
        yield return routePoster.Request(url, jsonData,
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

    private IEnumerator ParseGetObjByUserID(string userid, string typename, System.Action<string> callback)
    {
        // �]�w�ШD�� URL
        string url = ParseURL;

        // �ϥ� RequestPayload �]�� payload�A���ഫ�� JSON
        RequestDataByUserID requestData = new()
        {
            method = "getobjbyuserid",
            typename = typename,
            userid = userid,
        };
        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log($"jsonToSend={jsonData}");

        // �ϥγq�Ϊ� Request ��Ƶo�e�ШD�A�ô��Ѧ^��
        yield return routePoster.Request(url, jsonData,
            onSuccess: (response) =>
            {
                callback(response);
            },
            onError: (error) =>
            {
                // �B�z�ШD����
                ShowDialog("�s�u���~�A�еy��A�աC");
                Debug.LogError("GetObjByUserID_Error: " + error);
            });
    }

    private IEnumerator ParseRecoverIDByToken(string token, string typename, System.Action<string> callback)
    {
        // �]�w�ШD�� URL
        string url = ParseURL;

        // �ϥ� RequestPayload �]�� payload�A���ഫ�� JSON
        RequestDataByToken requestData = new()
        {
            method = "recoveridbytoken",
            typename = typename,
            token = token,
        };
        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log($"jsonToSend={jsonData}");

        // �ϥγq�Ϊ� Request ��Ƶo�e�ШD�A�ô��Ѧ^��
        yield return routePoster.Request(url, jsonData,
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
        UpdateDataLoadingTask(1);//4. �T�{�ϥΪ̦��Z: ��userID���^�ۤv�bLb�̪�score���(���^�ۤv�����Z�ӫD�ƦW)
        JObject data = JObject.Parse(jsonResponse);// �ѪR JSON �� JObject
        int score = data["score"]?.ToObject<int>() ?? 0;//data["key"]��JToken�A�n�A�ন���w���O
        Debug.Log($"��userID���^�ۤv�����Z:{score}");
        // �ˬd�^�Ǹ�ƬO�_����
        if (string.IsNullOrEmpty(jsonResponse))
        {
            PlayerPrefs.SetInt("score", 0);
        }
        else
        {
            // ��s����
            PlayerPrefs.SetInt("score", score);
        }

        // �u��signIn��recoverId���\�~���\�L������է�sleaderboard�A���auserID�s��n�J�h���ӭn������
        RecoverParseRequestEnergy();
        // ��sleaderboard
        DataUpdateCheck();
    }

    private void DataUpdateCheck()
    {
        // �ˬdenergy�O�_�����B���a�O�_��leaderboard����
        StartCoroutine(DataUpdate());
    }

    private IEnumerator DataUpdate()
    {
        // �M�w�n��s����data�A�Ҧp�Ʀ�]�B���N
        Debug.Log($"�}�l�ШD�Ʀ�]");
        yield return StartCoroutine(DataUpdate_Rank());// ��s�Ʀ�]
        // ��s����
        DataReady();
    }

    private IEnumerator DataUpdate_Rank() // ��s�Ʀ�]
    {
        // �]�w�ШD�� URL
        string url = ParseURL;

        // �ϥ� RequestPayload �]�� payload�A���ഫ�� JSON
        RequestRank requestData = new()
        {
            method = "ranktomyorder",
            typename = "Lb_" + GameName,
            descendingBy = "score",
            lines = 100,
            userID = PlayerPrefs.GetString("userID", ""), //�ΨӨ��o�ۤv��rank
        };
        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log($"jsonToSend={jsonData}");

        // �ϥγq�Ϊ� Request ��Ƶo�e�ШD�A�ô��Ѧ^��
        yield return routePoster.Request(url, jsonData,
            onSuccess: (response) =>
            {
                Debug.Log($"���^�Ʀ�]={response}");
                PlayerPrefs.SetString("leaderboard", response);
            },
            onError: (error) =>
            {
                // �B�z�ШD����
                //ShowDialog("�s�u���~�A�еy��A�աC");//rank���^���Ѥ��v�T�C���A���B�zerror
                Debug.LogError("DataUpdate_Rank Error: " + error);
            });
    }

    private void DataReady()
    {
        //���]�wname�����s���a
        if (PlayerPrefs.GetString("name").Length < 2)
        {
            IsNewUser = true;
        }
        if (IsNewUser)
        {
            GoToScene("selectScene");
        }
        else
        {
            GoToScene("titleScene");
        }
    }

    // �H�U�����U��k

    //private string GetPayloadFromQueryString(string key) //�o�̥Ψ쪺System.Web�ä��O�e�ݤ�k�Awebgl�i�Φ�����ĳ
    //{
    //    //string url = Application.absoluteURL;// �����e���}
    //    string url = "https://example.com/game?payload=myPayload&user=Player1";
    //    if (string.IsNullOrEmpty(url))// �T�O URL �D��
    //    {
    //        Debug.LogWarning("URL is empty or null.");
    //        return null;
    //    }

    //    try
    //    {
    //        var uri = new Uri(url);
    //        var queryParams = HttpUtility.ParseQueryString(uri.Query); //�ѪR Query String�A�nusing System.Web
    //        return queryParams[key];
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.LogError($"Error parsing URL: {ex.Rank}");
    //        return null;
    //    }
    //}

    private void ShowDialog(string message)
    {
        Debug.Log("��ܰT��: " + message);
    }

    private void GoToScene(string sceneName)
    {
        Debug.Log("�e������: " + sceneName);
    }

    private void RecoverParseRequestEnergy()
    {
        Debug.Log("�I�s RecoverParseRequestEnergy()");
    }

    private void UpdateDataLoadingTask(int progressToAdd)
    {
        loginProgress += progressToAdd;
    }
}
