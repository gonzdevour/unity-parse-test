using UnityEngine;
using UnityEngine.UI;

public class ToggleActivator : MonoBehaviour
{
    public Toggle toggle;          // 連結的 Toggle UI
    public GameObject targetObject; // 要被控制的物件
    public bool OffOnStart = true;
    public bool OffChildrenToggles = false;

    private void OnEnable()
    {
        if (toggle == null || targetObject == null)
        {
            Debug.LogWarning("Toggle 或 TargetObject 未設定！");
            return;
        }

        // 監聽 Toggle 的變更事件
        toggle.onValueChanged.AddListener(SetActiveState);

        // 初始化時關閉
        if (OffOnStart) toggle.isOn = false;
        // 初始化狀態，使目標物件與 Toggle 狀態同步
        targetObject.SetActive(toggle.isOn);
    }

    private void SetActiveState(bool isActive)
    {
        if (targetObject != null)
        {
            targetObject.SetActive(isActive);

            // 如果 targetObject 被關閉，則關閉所有子物件內的 Toggle
            if (!isActive && OffChildrenToggles)
            {
                DisableAllChildToggles(targetObject);
            }
        }
    }

    private void DisableAllChildToggles(GameObject parent)
    {
        Toggle[] toggles = parent.GetComponentsInChildren<Toggle>(true); // 取得所有子物件的 Toggle（包括非啟用狀態的）
        foreach (Toggle childToggle in toggles)
        {
            childToggle.isOn = false; // 設為 off
        }
    }

    private void OnDestroy()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(SetActiveState);
        }
    }
}
