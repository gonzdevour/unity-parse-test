using UnityEngine;
using UnityEngine.UI;
using NativeWebSocket;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using System;
using FancyScrollView.Leaderboard;

public class WebsocketChat : MonoBehaviour
{
    public Button sendBtn;
    public Button joinRoomBtn;
    public Button leaveRoomBtn;
    public Button getRoomsBtn;
    //public Button getClientsTreeBtn;
    public InputField messageInput;
    public InputField roomNameInput;
    public InputField userNameInput;
    public Dropdown RoomList;
    [SerializeField] private ScrollView_ChatUserList roommateListView;
    [SerializeField] private ScrollView_Chatbox chatBoxView;


    private WebSocket ws;
    private string currentRoom = string.Empty;

    private void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        ws.DispatchMessageQueue();
#endif
    }

    void Start()
    {
        // 初始化 WebSocket
        ws = new WebSocket("wss://playoneapps.com.tw/ws/");

        ws.OnOpen += () =>
        {
            Debug.Log("WebSocket connected.");
            //GetRoomList();
        };
        ws.OnError += (e) => Debug.LogError("WebSocket error: " + e);
        ws.OnClose += (e) => Debug.Log("WebSocket closed.");
        ws.OnMessage += (bytes) =>
        {
            //Debug.Log("got msg");
            var message = Encoding.UTF8.GetString(bytes);
            HandleServerMessage(message);
        };

        // 嘗試連接 WebSocket
        ConnectWebSocket();

        // 綁定dropdown事件
        RoomList.onValueChanged.AddListener(OnRoomSelected);

        // 綁定按鈕事件
        sendBtn.onClick.AddListener(() => SendPublicMessage(messageInput.text.Trim()));
        joinRoomBtn.onClick.AddListener(() => JoinRoom(userNameInput.text.Trim(), roomNameInput.text.Trim()));
        leaveRoomBtn.onClick.AddListener(LeaveRoom);
        getRoomsBtn.onClick.AddListener(GetRoomList);
        //getClientsTreeBtn.onClick.AddListener(GetClientsTree);
    }

    private async void OnApplicationQuit()
    {
        if (ws != null)
        {
            await ws.Close();
        }
    }

    public async void ConnectWebSocket()
    {
        await ws.Connect();
    }

    public async void SendPublicMessage(string message)
    {
        if (ws.State == WebSocketState.Open && !string.IsNullOrEmpty(message))
        {
            string payload = JsonConvert.SerializeObject(new { type = "public_message", message });
            await ws.SendText(payload);
            //Debug.Log($"You: {message}");
        }
        else
        {
            Debug.Log("WebSocket is not open or message is empty.");
        }
    }

    public async void JoinRoom(string userName, string roomName)
    {
        if (ws.State == WebSocketState.Open && !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(roomName))
        {
            string payload = JsonConvert.SerializeObject(new { type = "join_room", userName, roomName });
            await ws.SendText(payload);
            currentRoom = roomName;
            Debug.Log($"Joining room: {roomName} as {userName}");
        }
        else
        {
            Debug.Log("WebSocket is not open or userName/roomName is empty.");
        }
    }

    public async void LeaveRoom()
    {
        if (ws.State == WebSocketState.Open)
        {
            string payload = JsonConvert.SerializeObject(new { type = "leave_room" });
            await ws.SendText(payload);
            Debug.Log("Leaving room.");
            currentRoom = string.Empty;
        }
        else
        {
            Debug.Log("WebSocket is not open.");
        }
    }

    public async void GetRoomList()
    {
        if (ws.State == WebSocketState.Open)
        {
            string payload = JsonConvert.SerializeObject(new { type = "get_rooms" });
            await ws.SendText(payload);
            Debug.Log("Requesting room list.");
        }
        else
        {
            Debug.Log("WebSocket is not open.");
        }
    }

    public async void GetClientsTree()
    {
        if (ws.State == WebSocketState.Open)
        {
            string payload = JsonConvert.SerializeObject(new { type = "get_clients_tree" });
            await ws.SendText(payload);
            Debug.Log("Requesting clients tree.");
        }
        else
        {
            Debug.Log("WebSocket is not open.");
        }
    }

    private void HandleServerMessage(string message)
    {
        try
        {
            var serverMessage = JsonConvert.DeserializeObject<Dictionary<string, object>>(message);
            string type = serverMessage.ContainsKey("type") ? serverMessage["type"].ToString() : "unknown";

            switch (type)
            {
                case "welcome":
                    Debug.Log(serverMessage["message"]);
                    break;
                case "room_list":
                    string jsonString = JsonConvert.SerializeObject(serverMessage["rooms"]);
                    Debug.Log($"Room List: {jsonString}");
                    UpdateRoomList(jsonString);
                    break;
                case "clients_tree":
                    Debug.Log("Clients Tree: " + JsonConvert.SerializeObject(serverMessage["data"]));
                    break;
                case "public_message":
                    string msgString = JsonConvert.SerializeObject(serverMessage["message"]);
                    chatBoxView.UpdateChatBox(msgString);
                    Debug.Log($"[{serverMessage["userName"]}] {serverMessage["message"]}");
                    break;
                case "user_joined":
                    Debug.Log($"User joined: {serverMessage["message"]}");
                    break;
                case "user_left":
                    Debug.Log($"User left: {serverMessage["message"]}");
                    break;
                case "roommates_update":
                    // 取得roommates_update資料並呼叫UpdateRoommateList
                    string roommatesJson = JsonConvert.SerializeObject(serverMessage["roomMates"]);
                    Debug.Log($"Roommates Update: {roommatesJson}");
                    UpdateRoommateList(roommatesJson);
                    break;
                default:
                    Debug.Log("Unknown message type received: " + message);
                    break;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error parsing server message: " + ex.Message);
        }
    }

    public void UpdateRoomList(string roomsJson)
    {
        Debug.Log($"roomJson: {roomsJson}");

        var rooms = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(roomsJson);
        Debug.Log($"rooms: {rooms}");

        // 保留第一個選項
        var firstOption = RoomList.options[0];
        Debug.Log($"firstOption: {firstOption.text}");

        // 清除原有選項，但先保留第一個
        RoomList.options.Clear();
        RoomList.options.Add(firstOption);

        // 將房間資料填入後續選項
        foreach (var room in rooms)
        {
            string roomName = room["roomName"].ToString();
            int userCount = Convert.ToInt32(room["userCount"]); // 使用 Convert.ToInt32 確保可行的轉換

            string displayName = $"{roomName}({userCount})";
            //Debug.Log($"displayName: {displayName}");
            RoomList.options.Add(new Dropdown.OptionData(displayName));
        }

        // 刷新 Dropdown 顯示
        RoomList.RefreshShownValue();
    }

    public void UpdateRoommateList(string roommatesJson)
    {
        try
        {
            var roommates = JsonConvert.DeserializeObject<List<ChatUserData>>(roommatesJson);
            //Debug.Log($"[UpdateRoommateList] Successfully deserialized into {roommates.Count} roommates.");
            roommateListView.UpdateData(roommates);
            //Debug.Log("[UpdateRoommateList] roommateListView.UpdateData completed.");
        }
        catch (Exception ex)
        {
            // 如果在反序列化或更新過程中拋出例外，可以在這裡印出錯誤訊息
            Debug.LogError("[UpdateRoommateList] Exception occurred: " + ex.Message);
        }
    }


    void OnRoomSelected(int index)
    {
        Debug.Log($"index: {index}");
        // index = 0 的時候，不做變更 (保留第一個選項的原意)
        if (index <= 0)
        {
            return;
        }

        // Dropdown.OptionData 的 text 是先前 UpdateRoomList 時設定的字串格式: $"{roomName}({userCount})"
        var selectedText = RoomList.options[index].text;
        Debug.Log($"selectedText: {selectedText}");
        // 假設該字串格式固定為 "房間名(人數)"，想要只取房間名
        // 可以透過簡單的字串分割或索引定位來擷取：
        int parenthesisIndex = selectedText.IndexOf('(');
        string roomName = selectedText;
        if (parenthesisIndex > 0)
        {
            // roomName 是括號前的文字
            roomName = selectedText.Substring(0, parenthesisIndex);
        }
        Debug.Log($"roomName: {roomName}");
        // 將 roomName 設定到 roomNameInput
        roomNameInput.text = roomName;
    }
}
