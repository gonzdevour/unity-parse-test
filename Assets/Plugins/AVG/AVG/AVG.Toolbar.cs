using UnityEngine;
public partial class AVG
{
    private void OnAutoToggleChanged(bool isOn)
    {
        Debug.Log($"Auto Toggle Changed: {isOn}");
        // TODO: 在這裡處理自動播放的邏輯
        isAuto = isOn;
        if (isOn) CheckIfReadyToNext();
    }

    private void OnSkippingToggleChanged(bool isOn)
    {
        Debug.Log($"Skipping Toggle Changed: {isOn}");
        // TODO: 在這裡處理跳過已讀的邏輯
        isSkipping = isOn;
        if (isOn) CheckIfReadyToNext();
    }

    private void OnHideUIButtonClicked()
    {
        Debug.Log("Hide UI Button Clicked");
        // TODO: 介面 Alpha 變 0，並啟動等待
    }

    private void OnSaveButtonClicked()
    {
        Debug.Log("Save Button Clicked");
        // TODO: 顯示記錄面板
    }

    private void OnLoadButtonClicked()
    {
        Debug.Log("Load Button Clicked");
        // TODO: 顯示讀取面板
    }
}
