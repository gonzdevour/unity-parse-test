using UnityEngine;
using UnityEngine.UI;

public class InputSubmitter : MonoBehaviour
{
    public InputField inputField; // ���V InputField
    public Button lnputButton;   // ���V Button

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
            var reciever = LogPanelController.Inst.gameObject;
            // ���ձq reciever ����� IReceiver ���ե�
            var dataTaker = reciever.GetComponent<IReceiver>();
            if (dataTaker != null)
            {
                //Debug.Log(inputField.text); // �L�X InputField ����r
                dataTaker.Take(inputField.text); // �ե� Take ��k
            }
            else
            {
                Debug.LogWarning("Reciever �W�S����@ ITakeable ���ե�I");
            }
        }
        else
        {
            Debug.LogWarning("InputField ���]�w�I");
        }
    }
}
