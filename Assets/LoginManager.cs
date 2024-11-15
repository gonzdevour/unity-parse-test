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
        // 判斷是否在 Unity 編輯器中
        string payload;
        if (Application.isEditor)
        {
            payload = "eyJhbGciOiJQQkVTMi1IUzI1NitBMTI4S1ciLCJlbmMiOiJBMTI4Q0JDLUhTMjU2IiwicDJjIjoxMDAwMCwicDJzIjoiMEp0TVZqV3pQNkwwSTd4eEg1aE9tdyJ9.k-TcDWZMVgOadOwYn6f3YcV2Q4UfohX-Nf6itn60Hg36wKufr6RQ-w.eViwtmHR6YEb5AGquxs_-g.kCCvwnVmgFG8or3641Ml_hJRtG4tjvLKasNdQMwVvxKaJwqADtYVzHXq8OsjGFlEBhYw2TSyxtj-2jTtqbsaQQyCBDj4sy1FjJdFJt3H0cQ.SFTOxqPeIPtsfBy9VpbPbw"; // 預設值
        }
        else
        {
            payload = GetPayloadFromQueryString("user"); //實機時，從querystring取得user的參數內容
        }

        // 檢查 payload 是否為空
        if (!string.IsNullOrEmpty(payload))
        {
            StartCoroutine(RequestPlayerToken(payload));
        }
        else
        {
            ShowDialog("連線錯誤，請稍後再試。");
        }
    }

    private IEnumerator RequestPlayerToken(string payload)
    {
        // 設定請求的 URL
        string url = JoseDecodeURL;

        // 使用 RequestToken 包裝 payload，並轉換為 JSON
        RequestToken requestData = new(payload);
        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log($"jsonToSend={jsonData}");

        // 使用通用的 Request 函數發送請求，並提供回調
        yield return parsePoster.Request(url, jsonData,
            onSuccess: (response) =>
            {
                // 解析回傳的 JSON，提取 memberId 和 exp
                ResponseToken responseData = JsonUtility.FromJson<ResponseToken>(response);
                if (responseData != null)
                {
                    Debug.Log("myToken: " + responseData.memberId);
                    Debug.Log("Expiration: " + responseData.exp);
                    // 將myToken設定為取回的memberId
                    myToken = responseData.memberId;
                    // 從 PlayerPrefs 獲取 playerToken
                    string playerToken = PlayerPrefs.GetString("playerToken", "");

                    if (string.IsNullOrEmpty(playerToken) || playerToken != myToken)
                    {
                        // playerToken 為空或與 myToken 不同，執行 ParseRecoverIDByToken
                        Debug.Log("PlayerPrefs 中的 playerToken 為空 或與 myToken 不同，執行 RecoverByToken");
                        StartCoroutine(ParseRecoverIDByToken(myToken, "UserInfo", RecoverIdByTokenSuccess));
                    }
                    else
                    {
                        // playerToken 和 myToken 相同，表示已存在 UserID
                        Debug.Log("PlayerPrefs 中已存在 UserID，執行 parse_getobjbyid");
                        StartCoroutine(ParseGetObjById(myToken, "UserInfo", AssignMyUserInfo)); // 回呼函數為 AssignMyUserInfo
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

    private void AssignMyUserInfo(string response)
    {
        // 解析回傳的 JSON，提取 memberId 和 exp
        ResponseUserInfo responseData = JsonUtility.FromJson<ResponseUserInfo>(response);
        if (responseData != null)
        {
            Debug.Log("name: " + responseData.name);
            Debug.Log("objectId: " + responseData.objectId);
            // 檢查 objectId 是否存在
            bool hasObjectId = !string.IsNullOrEmpty(responseData.objectId);
            if (hasObjectId)
            {
                // 將資料覆蓋 PlayerPrefs
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
                Debug.Log($"取回的UserInfo中的objectId為空，執行 RecoverByToken");
                StartCoroutine(ParseRecoverIDByToken(myToken, "UserInfo", RecoverIdByTokenSuccess));
            }
        }
        else
        {
            ShowDialog("無法取得有效的 user 資料，請稍後再試。");
        }
    }

    private void RecoverIdByTokenSuccess(string response)
    {
        // 解析回傳的 JSON，提取 memberId 和 exp
        ResponseUserInfo responseData = JsonUtility.FromJson<ResponseUserInfo>(response);
        if (responseData != null)
        {
            Debug.Log("name: " + responseData.name);
            Debug.Log("objectId: " + responseData.objectId);
            // 檢查 objectId 是否存在
            bool hasObjectId = !string.IsNullOrEmpty(responseData.objectId);
            if (hasObjectId)
            {
                string userid = responseData.objectId;
                // 將資料覆蓋 PlayerPrefs
                UpdatePlayerPrefsFromJson(JsonUtility.ToJson(responseData));
                RecoverParseRequestEnergy();
                StartCoroutine(ParseGetObjById(userid, "Lb_eatall", ParseGetMyRecord)); //取得自己的leaderboard

                if (PlayerPrefs.GetString("myPlayerName").Length < 2)
                {
                    IsNewUser = true;
                }
            }
            else
            {
                Debug.Log($"Recover取回的UserInfo中的objectId為空，執行 SignIn");
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

        StartCoroutine(parsePoster.Request(url, jsonData,
            onSuccess: (response) =>
            {
                ResponseUserInfo responseData = JsonUtility.FromJson<ResponseUserInfo>(response);
                if (responseData != null)
                {
                    Debug.Log("Sign-in successful. Response: " + response);
                    // 將資料覆蓋 PlayerPrefs
                    UpdatePlayerPrefsFromJson(JsonUtility.ToJson(responseData));
                    IsNewUser = true;
                    RecoverParseRequestEnergy();
                    DataUpdateRequest();
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
                if (IsNetworkConnected())
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

        // 使用 RequestToken 包裝 payload，並轉換為 JSON
        RequestDataByID requestData = new()
        {
            method = "getobjbyid",
            typename = typename,
            id = objectId,
        };
        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log($"jsonToSend={jsonData}");

        // 使用通用的 Request 函數發送請求，並提供回調
        yield return parsePoster.Request(url, jsonData,
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

    private IEnumerator ParseRecoverIDByToken(string token, string typename, System.Action<string> callback)
    {
        // 設定請求的 URL
        string url = ParseURL;

        // 使用 RequestToken 包裝 payload，並轉換為 JSON
        RequestDataByToken requestData = new()
        {
            method = "getobjbyid",
            typename = typename,
            token = token,
        };
        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log($"jsonToSend={jsonData}");

        // 使用通用的 Request 函數發送請求，並提供回調
        yield return parsePoster.Request(url, jsonData,
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
        // 檢查回傳資料是否為空
        if (string.IsNullOrEmpty(jsonResponse))
        {
            PlayerPrefs.SetInt("score", 0);
        }
        else
        {
            // 更新分數
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
        // 模擬數據更新請求，假設成功
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

    // 以下為輔助方法
    private string GetPayloadFromQueryString(string key)
    {
        // 模擬取得 URL Query String 中的參數
        return "someQueryValue";
    }

    private void ShowDialog(string message)
    {
        Debug.Log("顯示訊息: " + message);
    }

    private bool CheckUserAgentForDcard()
    {
        // 假設 UserAgent 檢查
        return false;
    }

    private void UpdatePlayerPrefsFromJson(string json)
    {
        // 模擬更新 PlayerPrefs
        PlayerPrefs.SetString("playerData", json);
    }

    private int ExtractScoreFromJson(string json)
    {
        // 模擬從 JSON 中提取 score
        return 100;
    }

    private void GoToScene(string sceneName)
    {
        Debug.Log("前往場景: " + sceneName);
    }

    private bool IsNetworkConnected()
    {
        // 模擬網路連線狀態
        return true;
    }

    private void RecoverParseRequestEnergy()
    {
        Debug.Log("呼叫 RecoverParseRequestEnergy()");
    }

    private void ParseGetObjByUserId(string userId, System.Action<string> callback)
    {
        // 模擬請求取得用戶資料
        callback.Invoke("{\"score\":100}");
    }
}
