using System;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    public static Dialog Inst { get; private set; }
    private void Awake()
    {
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public Transform DialogLayer;
    public GameObject DialogYPrefab;
    public GameObject DialogYNPrefab;

    public void YN(string Title = "", string Content = "", Action CallbackY = null, Action CallbackN = null)
    {
        DialogYN dialog = Instantiate(DialogYNPrefab, DialogLayer).GetComponent<DialogYN>();
        dialog.Open(Title, Content, CallbackY, CallbackN);
    }

    public void Y(string Title = "", string Content = "", Action CallbackY = null)
    {
        DialogY dialog = Instantiate(DialogYNPrefab, DialogLayer).GetComponent<DialogY>();
        dialog.Open(Title, Content, CallbackY);
    }
}

