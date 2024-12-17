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
        // ��l�� WebSocket
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

        // ���ճs�� WebSocket
        ConnectWebSocket();

        // �j�wdropdown�ƥ�
        RoomList.onValueChanged.AddListener(OnRoomSelected);

        // �j�w���s�ƥ�
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
                    // ���oroommates_update��ƨéI�sUpdateRoommateList
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

        // �O�d�Ĥ@�ӿﶵ
        var firstOption = RoomList.options[0];
        Debug.Log($"firstOption: {firstOption.text}");

        // �M���즳�ﶵ�A�����O�d�Ĥ@��
        RoomList.options.Clear();
        RoomList.options.Add(firstOption);

        // �N�ж���ƶ�J����ﶵ
        foreach (var room in rooms)
        {
            string roomName = room["roomName"].ToString();
            int userCount = Convert.ToInt32(room["userCount"]); // �ϥ� Convert.ToInt32 �T�O�i�檺�ഫ

            string displayName = $"{roomName}({userCount})";
            //Debug.Log($"displayName: {displayName}");
            RoomList.options.Add(new Dropdown.OptionData(displayName));
        }

        // ��s Dropdown ���
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
            // �p�G�b�ϧǦC�ƩΧ�s�L�{���ߥX�ҥ~�A�i�H�b�o�̦L�X���~�T��
            Debug.LogError("[UpdateRoommateList] Exception occurred: " + ex.Message);
        }
    }


    void OnRoomSelected(int index)
    {
        Debug.Log($"index: {index}");
        // index = 0 ���ɭԡA�����ܧ� (�O�d�Ĥ@�ӿﶵ����N)
        if (index <= 0)
        {
            return;
        }

        // Dropdown.OptionData �� text �O���e UpdateRoomList �ɳ]�w���r��榡: $"{roomName}({userCount})"
        var selectedText = RoomList.options[index].text;
        Debug.Log($"selectedText: {selectedText}");
        // ���]�Ӧr��榡�T�w�� "�ж��W(�H��)"�A�Q�n�u���ж��W
        // �i�H�z�L²�檺�r����Ωί��ީw����^���G
        int parenthesisIndex = selectedText.IndexOf('(');
        string roomName = selectedText;
        if (parenthesisIndex > 0)
        {
            // roomName �O�A���e����r
            roomName = selectedText.Substring(0, parenthesisIndex);
        }
        Debug.Log($"roomName: {roomName}");
        // �N roomName �]�w�� roomNameInput
        roomNameInput.text = roomName;
    }
}
