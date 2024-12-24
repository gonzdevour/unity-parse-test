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
        Debug.Log($"{(isOn ? "���" : "�������")}{toggleTx.text}");
        Check();
    }

    public void Check()
    {
        if (CountSelectedToggles() > 0)
        {
            Debug.Log($"{SelectorName}�w��ܡG");
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
            Debug.Log($"{SelectorName}�|�����");
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
