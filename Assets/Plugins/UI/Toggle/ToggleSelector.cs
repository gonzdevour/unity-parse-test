using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ToggleSelector : MonoBehaviour
{
    public string SelectorName = "Selector";
    public Toggle[] toggles;

    public string[] SelectedToggleNames
    {
        get { return GetSelectedLabelNames(); }
    }

    public string SelectedToggleNamesString
    {
        get { return string.Join(",", SelectedToggleNames); }
    }

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
        //Debug.Log($"{(isOn ? "選擇" : "取消選擇")}{toggleTx.text}");
        if (isOn)
        {
            Debug.Log("選中的 LabelName: " + string.Join(", ", SelectedToggleNames));
        }
    }

    private string[] GetSelectedLabelNames()
    {
        // 回傳目前所有選中 Toggle 的 LabelName
        var selectedNames = new List<string>();

        foreach (Toggle toggle in toggles)
        {
            if (toggle.isOn)
            {
                ToggleVariables toggleVariables = toggle.GetComponent<ToggleVariables>();
                if (toggleVariables != null && !string.IsNullOrEmpty(toggleVariables.LabelName))
                {
                    selectedNames.Add(toggleVariables.LabelName);
                }
            }
        }
        return selectedNames.ToArray();
    }
}
