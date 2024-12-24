using UnityEngine;
using UnityEngine.UI;

public class ToggleSelector : MonoBehaviour
{
    public string SelectorName = "Selector";
    public Toggle[] toggles;

    public virtual void Start()
    {
        toggles = GetComponentsInChildren<Toggle>();
        foreach (Toggle toggle in toggles)
        {
            // 為每個 Toggle 添加 OnValueChanged 監聽事件
            toggle.onValueChanged.AddListener((isOn) => OnToggleValueChanged(toggle, isOn));
            // 取得Toggle中Label的Text組件
            Text toggleTx = toggle.GetComponentInChildren<Text>();
            // 設定每個Toggle Label Text的文字與顏色
            ToggleVariables toggleVariables = toggle.GetComponent<ToggleVariables>();
            if (toggleVariables != null)
            {
                string colorCode = ColorUtility.ToHtmlStringRGB(toggleVariables.LabelColor); // 將Color轉換為16進制RGBA
                if (!string.IsNullOrEmpty(toggleVariables.LabelName)) //LabelName有值才換字上色，否則維持原字預設值
                {
                    toggleTx.text = $"<color=#{colorCode}>{toggleVariables.LabelName}</color>";
                }
            }
        }
    }

    public virtual void OnToggleValueChanged(Toggle toggle, bool isOn)
    {
        Text toggleTx = toggle.GetComponentInChildren<Text>();
        Debug.Log($"{(isOn ? "選擇" : "取消選擇")}{toggleTx.text}");
        Check();
    }

    public void Check()
    {
        if (CountSelectedToggles() > 0)
        {
            Debug.Log($"{SelectorName}已選擇：");
            foreach (var toggle in toggles)
            {
                if (toggle.isOn)
                {
                    Text toggleTx = toggle.GetComponentInChildren<Text>();
                    Debug.Log($"- {toggleTx.text} ");
                }
            }
        }
        else
        {
            Debug.Log($"{SelectorName}尚未選擇");
        }
    }

    int CountSelectedToggles()
    {
        int count = 0;
        foreach (Toggle toggle in toggles)
        {
            if (toggle.isOn)
            {
                count++;
            }
        }
        return count;
    }
}
