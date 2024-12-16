using UnityEngine;

public class ReceiverDebug : MonoBehaviour, IReceiver
{
    public void Take(string message)
    {
        Debug.Log("[Receiver]" + message);
    }
}
