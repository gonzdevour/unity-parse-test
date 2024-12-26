using UnityEngine;
using UnityEngine.UI;

public class InputSubmitter : MonoBehaviour
{
    public InputField inputField; // 指向 InputField
    public Button lnputButton;   // 指向 Button

    void Start()
    {
        // 確保按鈕有綁定事件
        if (lnputButton != null)
        {
            lnputButton.onClick.AddListener(OnButtonClick);
        }
    }

    void OnButtonClick()
    {
        // 確保 InputField 不為空
        if (inputField != null)
        {
            var reciever = LogPanelController.Inst.gameObject;
            // 嘗試從 reciever 中獲取 IReceiver 的組件
            var dataTaker = reciever.GetComponent<IReceiver>();
            if (dataTaker != null)
            {
                //Debug.Log(inputField.text); // 印出 InputField 的文字
                dataTaker.Take(inputField.text); // 調用 Take 方法
            }
            else
            {
                Debug.LogWarning("Reciever 上沒有實作 ITakeable 的組件！");
            }
        }
        else
        {
            Debug.LogWarning("InputField 未設定！");
        }
    }
}
