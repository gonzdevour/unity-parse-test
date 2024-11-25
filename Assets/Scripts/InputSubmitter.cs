using UnityEngine;
using UnityEngine.UI;

public class InputSubmitter : MonoBehaviour
{
    public InputField inputField; // ���V InputField
    public Button lnputButton;      // ���V Button

    void Start()
    {
        // �T�O���s���j�w�ƥ�
        if (lnputButton != null)
        {
            lnputButton.onClick.AddListener(OnButtonClick);
        }
    }

    void OnButtonClick()
    {
        // �T�O InputField ������
        if (inputField != null)
        {
            // �L�X InputField ����r
            Debug.Log(inputField.text);
        }
        else
        {
            Debug.LogWarning("InputField ���]�w�I");
        }
    }
}
