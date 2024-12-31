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
            // ���C�� Toggle �K�[ OnValueChanged ��ť�ƥ�
            toggle.onValueChanged.AddListener((isOn) => OnToggleValueChanged(toggle, isOn));
            // ���oToggle��Label��Text�ե�
            Text toggleTx = toggle.GetComponentInChildren<Text>();
            // �]�w�C��Toggle Label Text����r�P�C��
            ToggleVariables toggleVariables = toggle.GetComponent<ToggleVariables>();
            if (toggleVariables != null)
            {
                string colorCode = ColorUtility.ToHtmlStringRGB(toggleVariables.LabelColor); // �NColor�ഫ��16�i��RGBA
                if (!string.IsNullOrEmpty(toggleVariables.LabelName)) //LabelName���Ȥ~���r�W��A�_�h������r�w�]��
                {
                    toggleTx.text = $"<color=#{colorCode}>{toggleVariables.LabelName}</color>";
                }
            }
        }
    }

    public virtual void OnToggleValueChanged(Toggle toggle, bool isOn)
    {
        Text toggleTx = toggle.GetComponentInChildren<Text>();
        //Debug.Log($"{(isOn ? "���" : "�������")}{toggleTx.text}");
        if (isOn)
        {
            Debug.Log("�襤�� LabelName: " + string.Join(", ", SelectedToggleNames));
        }
    }

    private string[] GetSelectedLabelNames()
    {
        // �^�ǥثe�Ҧ��襤 Toggle �� LabelName
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
