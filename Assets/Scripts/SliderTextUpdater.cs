using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SliderTextUpdater : MonoBehaviour
{
    [SerializeField] private Text valueText; // 要顯示文字的 Text 元件
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();

        // 註冊 Slider 的 OnValueChanged 事件
        slider.onValueChanged.AddListener(UpdateText);

        // 初始化 Text
        UpdateText(slider.value);
    }

    private void UpdateText(float value)
    {
        //Debug.Log("變化時的maxValue=" + slider.maxValue);
        if (valueText != null)
        {
            valueText.text = $"{value + 1} / {slider.maxValue + 1}";
        }
    }

    private void OnDestroy()
    {
        // 避免內存洩漏，移除事件監聽
        slider.onValueChanged.RemoveListener(UpdateText);
    }
}
