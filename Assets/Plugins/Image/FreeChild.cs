using UnityEngine;

public class FreeChild : MonoBehaviour
{
    private Vector3 initialBackgroundPosition;  // �I������l�@�ɮy��
    private Quaternion initialBackgroundRotation;  // �I������l����

    void Start()
    {
        // �O�s�I������l�@�ɮy�ЩM����
        initialBackgroundPosition = transform.position;
        initialBackgroundRotation = transform.rotation;
    }

    void LateUpdate()
    {
        // �C�V��s�I�����@�ɮy�ЩM����A�O���T�w
        transform.position = initialBackgroundPosition;
        transform.rotation = initialBackgroundRotation;
    }
}
