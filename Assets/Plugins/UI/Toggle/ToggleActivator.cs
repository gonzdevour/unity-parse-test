using UnityEngine;
using UnityEngine.UI;

public class ToggleActivator : MonoBehaviour
{
    public Toggle toggle;          // �s���� Toggle UI
    public GameObject targetObject; // �n�Q�������
    public bool OffOnStart = true;
    public bool OffChildrenToggles = false;

    private void OnEnable()
    {
        if (toggle == null || targetObject == null)
        {
            Debug.LogWarning("Toggle �� TargetObject ���]�w�I");
            return;
        }

        // ��ť Toggle ���ܧ�ƥ�
        toggle.onValueChanged.AddListener(SetActiveState);

        // ��l�Ʈ�����
        if (OffOnStart) toggle.isOn = false;
        // ��l�ƪ��A�A�ϥؼЪ���P Toggle ���A�P�B
        targetObject.SetActive(toggle.isOn);
    }

    private void SetActiveState(bool isActive)
    {
        if (targetObject != null)
        {
            targetObject.SetActive(isActive);

            // �p�G targetObject �Q�����A�h�����Ҧ��l���󤺪� Toggle
            if (!isActive && OffChildrenToggles)
            {
                DisableAllChildToggles(targetObject);
            }
        }
    }

    private void DisableAllChildToggles(GameObject parent)
    {
        Toggle[] toggles = parent.GetComponentsInChildren<Toggle>(true); // ���o�Ҧ��l���� Toggle�]�]�A�D�ҥΪ��A���^
        foreach (Toggle childToggle in toggles)
        {
            childToggle.isOn = false; // �]�� off
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
