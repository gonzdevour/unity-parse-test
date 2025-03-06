using System;
using UnityEngine;

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
    public GameObject DialogNamePrefab;
    public GameObject DialogChoicesPrefab;
    public GameObject DialogSavePrefab;
    public GameObject DialogLoadPrefab;

    public void YN(string Title = "", string Content = "", Action CallbackY = null, Action CallbackN = null, Transform Layer = null)
    {
        if (Layer == null) { Layer = DialogLayer; };
        DialogYN dialog = Instantiate(DialogYNPrefab, Layer).GetComponent<DialogYN>();
        dialog.Open(Title, Content, CallbackY, CallbackN);
    }

    public void Y(string Title = "", string Content = "", Action CallbackY = null, Transform Layer = null)
    {
        if (Layer == null) { Layer = DialogLayer; };
        DialogY dialog = Instantiate(DialogYPrefab, Layer).GetComponent<DialogY>();
        dialog.Open(Title, Content, CallbackY);
    }

    public void Name(string Name0, string Name1, string Title = "", string Content = "", Action<string> CallbackY = null, Transform Layer = null)
    {
        if (Layer == null) { Layer = DialogLayer; };
        DialogName dialog = Instantiate(DialogNamePrefab, Layer).GetComponent<DialogName>();
        dialog.Open(Name0, Name1, Title, Content, CallbackY);
    }

    public void Choices(string[] options, string[] results, string Title = "", string Content = "", Action<string, GameObject> CallbackY = null, Transform Layer = null)
    {
        if (Layer == null) { Layer = DialogLayer; };
        DialogChoices dialog = Instantiate(DialogChoicesPrefab, Layer).GetComponent<DialogChoices>();
        dialog.Open(options, results, Title, Content, CallbackY);
    }

    public void Save(string Title = "", Action<string, GameObject> CallbackY = null, Transform Layer = null)
    {
        if (Layer == null) { Layer = DialogLayer; };
        DialogSave dialog = Instantiate(DialogSavePrefab, Layer).GetComponent<DialogSave>();
        dialog.Open(Title, CallbackY);
    }

    public void Load(string Title = "", Action<string, GameObject> CallbackY = null, Transform Layer = null)
    {
        if (Layer == null) { Layer = DialogLayer; };
        DialogLoad dialog = Instantiate(DialogLoadPrefab, Layer).GetComponent<DialogLoad>();
        dialog.Open(Title, CallbackY);
    }
}

