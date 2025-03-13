using UnityEngine;

public partial class AVG
{
    private void OnAutoToggleChanged(bool isOn)
    {
        Debug.Log($"Auto Toggle Changed: {isOn}");
        // TODO: �b�o�̳B�z�۰ʼ����޿�
        isAuto = isOn;
        if (isOn) CheckIfReadyToNext();
    }

    private void OnSkippingToggleChanged(bool isOn)
    {
        Debug.Log($"Skipping Toggle Changed: {isOn}");
        // TODO: �b�o�̳B�z���L�wŪ���޿�
        isSkipping = isOn;
        if (isOn) CheckIfReadyToNext();
    }

    private void OnSaveButtonClicked()
    {
        Dialog.Inst.Save("�x�s�ɮ�");
    }

    private void OnLoadButtonClicked()
    {
        Dialog.Inst.Load("Ū���ɮ�");
    }

    private void OnHideUIButtonClicked()
    {
        Debug.Log("Hide UI Button Clicked");
        UIHide();
    }

    private void UIHide()
    {
        StoryPlayerObj.SetActive(false);
        Toolbar.SetActive(false);
    }

    private void UIUnhide()
    {
        StoryPlayerObj.SetActive(true);
        Toolbar.SetActive(true);
    }
}
