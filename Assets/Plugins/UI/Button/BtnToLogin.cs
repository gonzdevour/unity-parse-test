using UnityEngine;
using UnityEngine.UI;

public class BtnToLogin : MonoBehaviour
{
    public Button button;

    void Start()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        button.interactable = false; // �T�Ϋ��s
        var loginManager = LoginManager.Inst;
        if (loginManager != null)
        {
            loginManager.LoginFromPlayerPref();
            //loginManager.LoginFromCheckingPayload();
        }
    }
}
