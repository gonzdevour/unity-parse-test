using UnityEngine;
using UnityEngine.UI;

public class InputSubmitter : MonoBehaviour
{
    public InputField inputField; // 指向 InputField
    public Button lnputButton;      // 指向 Button

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
            // 印出 InputField 的文字
            Debug.Log(inputField.text);
        }
        else
        {
            Debug.LogWarning("InputField 未設定！");
        }
    }
}
