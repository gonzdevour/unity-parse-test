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

    private void OnSaveButtonClicked()
    {
        Dialog.Inst.Save("儲存檔案");
    }

    private void OnLoadButtonClicked()
    {
        Dialog.Inst.Load("讀取檔案");
    }

    private void OnHideUIButtonClicked()
    {
        Debug.Log("Hide UI Button Clicked");
        UIHide();
    }

    private void UIHide()
    {
        if (CGMode) StoryCG.SetActive(false);
        if (DisplayBubble) StoryBubble.SetActive(false);
        if (DisplayStoryBox) StoryBox.SetActive(false);
        Toolbar.SetActive(false);
    }

    private void UIUnhide()
    {
        if (CGMode) StoryCG.SetActive(true);
        if (DisplayBubble) StoryBubble.SetActive(true);
        if (DisplayStoryBox) StoryBox.SetActive(true);
        Toolbar.SetActive(true);
    }
}
