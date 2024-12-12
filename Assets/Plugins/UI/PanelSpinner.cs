using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // 引入 DOTween
using System.Collections; // 引入協程

public class PanelSpinner : MonoBehaviour
{
    public Text spTitle;      // 顯示標題的文字
    public Text spMsg;        // 顯示訊息的文字

    public void SetTitle(string title)
    {
        spTitle.text = title;
    }

    public void SetMessage(string msg)
    {
        spMsg.text = msg;
    }
}
