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

    public GameObject Modal;
    public GameObject Board;
    public Button BtnY;
    public Button BtnN;
    public Text TxTitle;
    public Text TxContent;
    private Action cbkY;
    private Action cbkN;

    private void OnEnable()
    {
        BtnY.onClick.AddListener(OnYesClicked);
        BtnN.onClick.AddListener(OnNoClicked);
    }

    private void OnDisable()
    {
        BtnY.onClick.RemoveListener(OnYesClicked);
        BtnN.onClick.RemoveListener(OnNoClicked);
    }

    public void YN(string Title = "", string Content = "", Action CallbackY = null, Action CallbackN = null)
    {
        Modal.SetActive(true);
        TxTitle.gameObject.SetActive(!string.IsNullOrEmpty(Title));
        TxContent.gameObject.SetActive(!string.IsNullOrEmpty(Content));
        BtnY.gameObject.SetActive(true);
        BtnN.gameObject.SetActive(true);

        TxTitle.text = Title;
        TxContent.text = Content;

        cbkY = CallbackY;
        cbkN = CallbackN;
    }

    public void Y(string Title = "", string Content = "", Action CallbackY = null)
    {
        Modal.SetActive(true);
        TxTitle.gameObject.SetActive(!string.IsNullOrEmpty(Title));
        TxContent.gameObject.SetActive(!string.IsNullOrEmpty(Content));
        BtnY.gameObject.SetActive(true);
        BtnN.gameObject.SetActive(false);

        TxTitle.text = Title;
        TxContent.text = Content;

        cbkY = CallbackY;
    }

    private void OnYesClicked()
    {
        cbkY?.Invoke();
        Close();
    }

    private void OnNoClicked()
    {
        cbkN?.Invoke();
        Close();
    }

    public void Close()
    {
        Modal.SetActive(false);
    }
}

