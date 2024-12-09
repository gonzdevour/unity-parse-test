using UnityEngine;
using UnityEngine.UI;

public class BtnToScene : MonoBehaviour
{
    public Button button;
    public string targetScene; // ���w�n�����������W��

    void Start()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        button.interactable = false; // �T�Ϋ��s
        var sceneSwitcher = FindObjectOfType<SceneSwitcher>();
        if (sceneSwitcher != null)
        {
            sceneSwitcher.SwitchScene(targetScene);
        }
    }
}
