using System;
using UnityEngine;
using UnityEngine.UI;

public class DialogY : MonoBehaviour
{
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

    public void Open(string Title = "", string Content = "", Action CallbackY = null)
    {
        TxTitle.gameObject.SetActive(!string.IsNullOrEmpty(Title));
        TxContent.gameObject.SetActive(!string.IsNullOrEmpty(Content));
        BtnY.gameObject.SetActive(true);
        BtnN.gameObject.SetActive(false);

        TxTitle.text = Title;
        TxContent.text = Content;

        cbkY = CallbackY;
    }

    public void Close()
    {
        Destroy(gameObject);
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
}

