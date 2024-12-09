using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using System;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine.SocialPlatforms.Impl;
// using System.Web;//原本用來解析UserAgent，但這是後端用的lib，換成使用webgl plugin直接從js取值

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
        //StartCoroutine(RequestEncodeTokenToPayload("test-encode-ab1234"));//測試jose encode
        //Debug.Log(GetPayloadFromQueryString("user"));//測試System.Web解析querystring
        //Debug.Log("Login Start");
        //LoginCheckPayload();
    }

    public void LoginCheckPayload()
    {
        string payload = "";
        if (Application.isEditor) // 在 Unity 編輯器中，測試值
        {
            payload = "eyJhbGciOiJQQkVTMi1IUzI1NitBMTI4S1ciLCJlbmMiOiJBMTI4Q0JDLUhTMjU2IiwicDJjIjoxMDAwMCwicDJzIjoiMEp0TVZqV3pQNkwwSTd4eEg1aE9tdyJ9.k-TcDWZMVgOadOwYn6f3YcV2Q4UfohX-Nf6itn60Hg36wKufr6RQ-w.eViwtmHR6YEb5AGquxs_-g.kCCvwnVmgFG8or3641Ml_hJRtG4tjvLKasNdQMwVvxKaJwqADtYVzHXq8OsjGFlEBhYw2TSyxtj-2jTtqbsaQQyCBDj4sy1FjJdFJt3H0cQ.SFTOxqPeIPtsfBy9VpbPbw"; // 預設值
        }
        else if (IsOnWeb) // 在web實機，從querystring取得user的參數內容
        {
            payload = queryStringParser.Get("user");
        }

        // 檢查 payload 是否為空
        if (!string.IsNullOrEmpty(payload))
        {
            UpdateDataLoadingTask(1);//1. 確認payload有值
            StartCoroutine(RequestDecodePayloadToToken(payload));
        }
        else
        {
            ShowDialog("登入權限異常，請稍後再試。");
        }
    }

    private IEnumerator RequestEncodeTokenToPayload(string payload)
    {
        // 設定請求的 URL
        string url = JoseEncodeURL;

        // 使用 RequestPayload 包裝 payload，並轉換為 JSON
        RequestPayload requestData = new(payload);
        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log("RequestEncodeTokenToPayload Start");
        Debug.Log($"jsonToSend={jsonData}");

        // 使用通用的 Request 函數發送請求，並提供回調
        yield return routePoster.Request(url, jsonData,
            onSuccess: (response) =>
            {
                ResponseJWE responseData = JsonUtility.FromJson<ResponseJWE>(response);
                if (responseData != null)
                {
                    Debug.Log("RequestEncodeTokenToPayload Success");
                    Debug.Log(responseData.jwe);
                    UpdateDataLoadingTask(1);//1. 確認payload：用token取回了payload
                }
                else
                {
                    ShowDialog("無法取得加密token，請稍後再試。");
                }
            },
            onError: (error) =>
            {
                // 處理請求失敗
                ShowDialog("連線錯誤，請稍後再試。");
                Debug.LogError("Error: " + error);
            });
    }

    private IEnumerator RequestDecodePayloadToToken(string payload)
    {
        // 設定請求的 URL
        string url = JoseDecodeURL;

        // 使用 RequestPayload 包裝 payload，並轉換為 JSON
        RequestPayload requestData = new(payload);
        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log("RequestDecodePayloadToToken Start");
        Debug.Log($"jsonToSend={jsonData}");

        // 使用通用的 Request 函數發送請求，並提供回調
        yield return routePoster.Request(url, jsonData,
            onSuccess: (response) =>
            {
                UpdateDataLoadingTask(1);//2. 確認token:用payload取回了token
                // 解析回傳的 JSON，提取 memberId 和 exp
                ResponseToken responseData = JsonUtility.FromJson<ResponseToken>(response);
                if (responseData != null)
                {
                    Debug.Log("RequestDecodePayloadToToken Success");
                    Debug.Log("token: " + responseData.memberId);
                    Debug.Log("Expiration: " + responseData.exp);
                    // 將myToken設定為取回的memberId
                    myToken = responseData.memberId;

                    // 判斷PlayerPrefs中token是否存在
                    string playerToken = PlayerPrefs.GetString("token", "");
                    if (string.IsNullOrEmpty(playerToken) || playerToken != myToken)
                    {
                        // playerToken 為空或與 myToken 不同，執行 ParseRecoverIDByToken
                        Debug.Log("PlayerPrefs 中的 playerToken 為空 或與 myToken 不同，執行 RecoverByToken");
                        StartCoroutine(ParseRecoverIDByToken(myToken, "UserInfo", ParseRecoverIdByTokenSuccess));
                    }
                    else
                    {
                        // playerToken 和 myToken 相同，表示已存在 userID，userID即為UserInfo中的objectId，所以使用ParseGetObjById而非ParseGetObjByUserId
                        string userID = PlayerPrefs.GetString("userID", "");
                        Debug.Log($"PlayerPrefs中已存在 userID={userID}，執行parse_getobjbyid");
                        StartCoroutine(ParseGetObjById(userID, "UserInfo", ParseGetUserInfoByIdSuccess)); // 回呼函數為 ParseGetUserInfoByIdSuccess
                    }
                }
                else
                {
                    ShowDialog("無法取得有效的 token 資料，請稍後再試。");
                }
            },
            onError: (error) =>
            {
                // 處理請求失敗
                ShowDialog("連線錯誤，請稍後再試。");
                Debug.LogError("Error: " + error);
            });
    }

    private void ParseGetUserInfoByIdSuccess(string response) //用本地userID登入成功
    {
        // 解析回傳的 JSON，提取 memberId 和 exp
        UserData responseData = JsonUtility.FromJson<UserData>(response);
        if (responseData != null)
        {
            Debug.Log("name: " + responseData.name);
            Debug.Log("objectId: " + responseData.objectId);
            // 檢查 objectId 是否存在
            bool hasObjectId = !string.IsNullOrEmpty(responseData.objectId);
            if (hasObjectId)
            {
                UpdateDataLoadingTask(1);//3. 確認使用者資料: 用userID取回了UserInfo，表示PlayerPref的本地資料存在
                UpdateDataLoadingTask(1);//4. 確認使用者成績: PlayerPref的本地資料存在，因此score資料也存在
                // 將資料覆蓋 PlayerPrefs
                userDataManager.Update(responseData);
                // 只有signIn或recoverId成功才允許無限制嘗試更新leaderboard，本地userID連續登入則應該要有限制
                RecoverParseRequestEnergy();
                // 更新leaderboard
                DataUpdateCheck();
            }
            else
            {
                Debug.Log($"取回的UserInfo中的objectId為空，執行 RecoverByToken");
                StartCoroutine(ParseRecoverIDByToken(myToken, "UserInfo", ParseRecoverIdByTokenSuccess));
            }
        }
        else
        {
            ShowDialog("無法取得有效的 user 資料，請稍後再試。");
        }
    }

    private void ParseRecoverIdByTokenSuccess(string response)
    {
        // 解析回傳的 JSON，提取 memberId 和 exp
        UserData responseData = JsonUtility.FromJson<UserData>(response);
        if (responseData != null)
        {
            Debug.Log("name: " + responseData.name);
            Debug.Log("objectId: " + responseData.objectId);
            // 檢查 objectId 是否存在
            bool hasObjectId = !string.IsNullOrEmpty(responseData.objectId);
            if (hasObjectId)
            {
                UpdateDataLoadingTask(1);//3. 確認使用者資料: 用token取回了UserInfo，將資料覆蓋 PlayerPrefs

                string userID = responseData.objectId;
                // 將資料覆蓋 PlayerPrefs
                userDataManager.Update(responseData);
                StartCoroutine(ParseGetObjByUserID(userID, "Lb_"+ GameName, ParseGetMyRecord)); //取得自己的leaderboard
            }
            else
            {
                Debug.Log($"Recover取回的UserInfo中的objectId為空，執行 SignIn");
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
            ShowDialog("無法取得有效的 user 資料，請稍後再試。");
        }
    }

    // SignIn 函數 (調用原始的 SignIn 代碼)
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
                    // signIn成功必定是新玩家
                    IsNewUser = true;
                    // 將資料覆蓋 PlayerPrefs
                    userDataManager.Update(responseData);
                    // 只有signIn或recoverId成功才允許無限制嘗試更新leaderboard，本地userID連續登入則應該要有限制
                    RecoverParseRequestEnergy();
                    // 更新leaderboard
                    DataUpdateCheck();
                }
                else
                {
                    Debug.LogError("Sign-in successful, but Response is null");
                    ShowDialog("連線錯誤，請稍後再試。");
                }
            },
            onError: (error) =>
            {
                Debug.LogError("Sign-in failed. Error: " + error);
                // 在這裡處理錯誤邏輯
                if (IsOnline)
                {
                    ShowDialog("連線錯誤，請稍後再試。");
                }
                else
                {
                    ShowDialog("請檢查網路連線。");
                }
            }));
    }

    private IEnumerator ParseGetObjById(string objectId, string typename, System.Action<string> callback)
    {
        // 設定請求的 URL
        string url = ParseURL;

        // 使用 RequestPayload 包裝 payload，並轉換為 JSON
        RequestDataByID requestData = new()
        {
            method = "getobjbyid",
            typename = typename,
            id = objectId,
        };
        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log($"jsonToSend={jsonData}");

        // 使用通用的 Request 函數發送請求，並提供回調
        yield return routePoster.Request(url, jsonData,
            onSuccess: (response) =>
            {
                callback(response);
            },
            onError: (error) =>
            {
                // 處理請求失敗
                ShowDialog("連線錯誤，請稍後再試。");
                Debug.LogError("GetObjByID_Error: " + error);
            });
    }

    private IEnumerator ParseGetObjByUserID(string userid, string typename, System.Action<string> callback)
    {
        // 設定請求的 URL
        string url = ParseURL;

        // 使用 RequestPayload 包裝 payload，並轉換為 JSON
        RequestDataByUserID requestData = new()
        {
            method = "getobjbyuserid",
            typename = typename,
            userid = userid,
        };
        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log($"jsonToSend={jsonData}");

        // 使用通用的 Request 函數發送請求，並提供回調
        yield return routePoster.Request(url, jsonData,
            onSuccess: (response) =>
            {
                callback(response);
            },
            onError: (error) =>
            {
                // 處理請求失敗
                ShowDialog("連線錯誤，請稍後再試。");
                Debug.LogError("GetObjByUserID_Error: " + error);
            });
    }

    private IEnumerator ParseRecoverIDByToken(string token, string typename, System.Action<string> callback)
    {
        // 設定請求的 URL
        string url = ParseURL;

        // 使用 RequestPayload 包裝 payload，並轉換為 JSON
        RequestDataByToken requestData = new()
        {
            method = "recoveridbytoken",
            typename = typename,
            token = token,
        };
        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log($"jsonToSend={jsonData}");

        // 使用通用的 Request 函數發送請求，並提供回調
        yield return routePoster.Request(url, jsonData,
            onSuccess: (response) =>
            {
                callback(response);
            },
            onError: (error) =>
            {
                // 處理請求失敗
                ShowDialog("連線錯誤，請稍後再試。");
                Debug.LogError("RecoverIDByToken_Error: " + error);
            });
    }

    private void ParseGetMyRecord(string jsonResponse)
    {
        UpdateDataLoadingTask(1);//4. 確認使用者成績: 用userID取回自己在Lb裡的score資料(取回自己的成績而非排名)
        JObject data = JObject.Parse(jsonResponse);// 解析 JSON 為 JObject
        int score = data["score"]?.ToObject<int>() ?? 0;//data["key"]為JToken，要再轉成指定型別
        Debug.Log($"用userID取回自己的成績:{score}");
        // 檢查回傳資料是否為空
        if (string.IsNullOrEmpty(jsonResponse))
        {
            PlayerPrefs.SetInt("score", 0);
        }
        else
        {
            // 更新分數
            PlayerPrefs.SetInt("score", score);
        }

        // 只有signIn或recoverId成功才允許無限制嘗試更新leaderboard，本地userID連續登入則應該要有限制
        RecoverParseRequestEnergy();
        // 更新leaderboard
        DataUpdateCheck();
    }

    private void DataUpdateCheck()
    {
        // 檢查energy是否足夠、本地是否有leaderboard的值
        StartCoroutine(DataUpdate());
    }

    private IEnumerator DataUpdate()
    {
        // 決定要更新哪些data，例如排行榜、成就
        Debug.Log($"開始請求排行榜");
        yield return StartCoroutine(DataUpdate_Rank());// 更新排行榜
        // 更新完成
        DataReady();
    }

    private IEnumerator DataUpdate_Rank() // 更新排行榜
    {
        // 設定請求的 URL
        string url = ParseURL;

        // 使用 RequestPayload 包裝 payload，並轉換為 JSON
        RequestRank requestData = new()
        {
            method = "ranktomyorder",
            typename = "Lb_" + GameName,
            descendingBy = "score",
            lines = 100,
            userID = PlayerPrefs.GetString("userID", ""), //用來取得自己的rank
        };
        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log($"jsonToSend={jsonData}");

        // 使用通用的 Request 函數發送請求，並提供回調
        yield return routePoster.Request(url, jsonData,
            onSuccess: (response) =>
            {
                Debug.Log($"取回排行榜={response}");
                PlayerPrefs.SetString("leaderboard", response);
            },
            onError: (error) =>
            {
                // 處理請求失敗
                //ShowDialog("連線錯誤，請稍後再試。");//rank取回失敗不影響遊戲，不處理error
                Debug.LogError("DataUpdate_Rank Error: " + error);
            });
    }

    private void DataReady()
    {
        //未設定name視為新玩家
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

    // 以下為輔助方法

    //private string GetPayloadFromQueryString(string key) //這裡用到的System.Web並不是前端方法，webgl可用但不建議
    //{
    //    //string url = Application.absoluteURL;// 獲取當前網址
    //    string url = "https://example.com/game?payload=myPayload&user=Player1";
    //    if (string.IsNullOrEmpty(url))// 確保 URL 非空
    //    {
    //        Debug.LogWarning("URL is empty or null.");
    //        return null;
    //    }

    //    try
    //    {
    //        var uri = new Uri(url);
    //        var queryParams = HttpUtility.ParseQueryString(uri.Query); //解析 Query String，要using System.Web
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
        Debug.Log("顯示訊息: " + message);
    }

    private void GoToScene(string sceneName)
    {
        Debug.Log("前往場景: " + sceneName);
    }

    private void RecoverParseRequestEnergy()
    {
        Debug.Log("呼叫 RecoverParseRequestEnergy()");
    }

    private void UpdateDataLoadingTask(int progressToAdd)
    {
        loginProgress += progressToAdd;
    }
}
