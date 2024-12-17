using UnityEngine;

public class ReceiverWSChat : MonoBehaviour, IReceiver
{
    public void Take(string message)
    {
        WebsocketChat websocketChat = gameObject.GetComponent<WebsocketChat>();
        websocketChat.SendPublicMessage(message);
    }
}
