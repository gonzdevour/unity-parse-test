using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // �ޤJ DOTween
using System.Collections; // �ޤJ��{

public class PanelSpinner : MonoBehaviour
{
    public Text spTitle;      // ��ܼ��D����r
    public Text spMsg;        // ��ܰT������r

    public void SetTitle(string title)
    {
        spTitle.text = title;
    }

    public void SetMessage(string msg)
    {
        spMsg.text = msg;
    }

    public void On(string title = "", string message = "")
    {
        Clear();
        if (!string.IsNullOrEmpty(title))
        {
            spTitle.text = title;
        }
        if (!string.IsNullOrEmpty(message))
        {
            spMsg.text = message;
        }
        gameObject.SetActive(true);
    }

    public void Off()
    {
        Clear();
        gameObject.SetActive(false);
    }

    public void Clear()
    {
        spTitle.text = "";
        spMsg.text = "";
    }
}
