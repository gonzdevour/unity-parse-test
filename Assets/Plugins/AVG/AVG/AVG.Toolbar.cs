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

    private void OnHideUIButtonClicked()
    {
        Debug.Log("Hide UI Button Clicked");
        // TODO: ���� Alpha �� 0�A�ñҰʵ���
    }

    private void OnSaveButtonClicked()
    {
        Debug.Log("Save Button Clicked");
        // TODO: ��ܰO�����O
    }

    private void OnLoadButtonClicked()
    {
        Debug.Log("Load Button Clicked");
        // TODO: ���Ū�����O
    }
}
