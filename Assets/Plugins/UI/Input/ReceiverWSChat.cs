using UnityEngine;

public class ReceiverWSChat : MonoBehaviour, IReceiver
{
    public void Take(string message)
    {
        Debug.Log("[WS]" + message);
    }
}
