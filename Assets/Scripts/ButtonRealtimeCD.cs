using UnityEngine;
using UnityEngine.UI;

public class ButtonRealtimeCD : MonoBehaviour
{
    public Button myButton; // 当前按钮
    public Text cooldownText; // 冷却倒计时显示的文本
    public TimeAway timeAway; // 引用改进的 TimeAway 类
    public float cooldownDuration = 60f; // 冷却时间（秒）

    private void Start()
    {
        // 初始化按钮点击事件
        myButton.onClick.AddListener(OnButtonPressed);

        // 初始化按钮状态
        UpdateButtonInteractable();
    }

    private void Update()
    {
        // 实时检查按钮状态并更新剩余时间
        UpdateButtonInteractable();
        UpdateCooldownText();
    }

    private void OnButtonPressed()
    {
        float remainingTime;
        bool isCooldownComplete = timeAway.CheckCooldownRealtime(myButton.name, cooldownDuration, out remainingTime);

        if (isCooldownComplete)
        {
            Debug.Log($" {myButton.name} clicked");
            timeAway.StartCooldownRealtime(myButton.name);
        }
        else
        {
            Debug.Log($" {myButton.name} cooldown：{Mathf.Ceil(remainingTime)} sec");
        }
    }

    private void UpdateButtonInteractable()
    {
        // 动态更新按钮的交互状态
        float remainingTime;
        myButton.interactable = timeAway.CheckCooldownRealtime(myButton.name, cooldownDuration, out remainingTime);
    }

    private void UpdateCooldownText()
    {
        // 更新冷却时间显示
        float remainingTime;
        timeAway.CheckCooldownRealtime(myButton.name, cooldownDuration, out remainingTime);

        if (remainingTime > 0f)
        {
            cooldownText.text = $"CD: {Mathf.Ceil(remainingTime)} sec";
        }
        else
        {
            cooldownText.text = "Ready";
        }
    }
}
