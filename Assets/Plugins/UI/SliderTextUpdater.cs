using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SliderTextUpdater : MonoBehaviour
{
    [SerializeField] private Text valueText; // �n��ܤ�r�� Text ����
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();

        // ���U Slider �� OnValueChanged �ƥ�
        slider.onValueChanged.AddListener(UpdateText);

        // ��l�� Text
        UpdateText(slider.value);
    }

    private void UpdateText(float value)
    {
        //Debug.Log("�ܤƮɪ�maxValue=" + slider.maxValue);
        if (valueText != null)
        {
            valueText.text = $"{value + 1} / {slider.maxValue + 1}";
        }
    }

    private void OnDestroy()
    {
        // �קK���s���|�A�����ƥ��ť
        slider.onValueChanged.RemoveListener(UpdateText);
    }
}
