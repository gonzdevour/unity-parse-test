using UnityEngine;
using UnityEngine.UI;
using NativeWebSocket;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;

public class WebsocketChat : MonoBehaviour
{
    public Button sendBtn;
    public Button joinRoomBtn;
    public Button leaveRoomBtn;
    public Button getRoomsBtn;
    public Button getClientsTreeBtn;
    public InputField messageInput;
    public InputField roomNameInput;
    public InputField userNameInput;

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

        ws.OnOpen += () => Debug.Log("WebSocket connected.");
        ws.OnError += (e) => Debug.LogError("WebSocket error: " + e);
        ws.OnClose += (e) => Debug.Log("WebSocket closed.");
        ws.OnMessage += (bytes) =>
        {
            Debug.Log("got msg");
            var message = Encoding.UTF8.GetString(bytes);
            HandleServerMessage(message);
        };

        // 嘗試連接 WebSocket
        ConnectWebSocket();

        // 綁定按鈕事件
        sendBtn.onClick.AddListener(() => SendPublicMessage(messageInput.text.Trim()));
        joinRoomBtn.onClick.AddListener(() => JoinRoom(userNameInput.text.Trim(), roomNameInput.text.Trim()));
        leaveRoomBtn.onClick.AddListener(LeaveRoom);
        getRoomsBtn.onClick.AddListener(GetRoomList);
        getClientsTreeBtn.onClick.AddListener(GetClientsTree);
    }

    private async void ConnectWebSocket()
    {
        await ws.Connect();
    }

    private async void SendPublicMessage(string message)
    {
        if (ws.State == WebSocketState.Open && !string.IsNullOrEmpty(message))
        {
            string payload = JsonConvert.SerializeObject(new { type = "public_message", message });
            await ws.SendText(payload);
            Debug.Log($"You: {message}");
        }
        else
        {
            Debug.Log("WebSocket is not open or message is empty.");
        }
    }

    private async void JoinRoom(string userName, string roomName)
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

    private async void LeaveRoom()
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

    private async void GetRoomList()
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

    private async void GetClientsTree()
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
                    Debug.Log("Room List: " + JsonConvert.SerializeObject(serverMessage["rooms"]));
                    break;
                case "clients_tree":
                    Debug.Log("Clients Tree: " + JsonConvert.SerializeObject(serverMessage["data"]));
                    break;
                case "public_message":
                    Debug.Log($"[{serverMessage["userName"]}] {serverMessage["message"]}");
                    break;
                case "user_joined":
                    Debug.Log($"User joined: {serverMessage["message"]}");
                    break;
                case "user_left":
                    Debug.Log($"User left: {serverMessage["message"]}");
                    break;
                case "roommates_update":
                    Debug.Log("Updated roommates: " + JsonConvert.SerializeObject(serverMessage["roomMates"]));
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

    private async void OnApplicationQuit()
    {
        if (ws != null)
        {
            await ws.Close();
        }
    }
}
