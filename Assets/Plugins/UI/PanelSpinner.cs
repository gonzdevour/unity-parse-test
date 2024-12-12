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
}
