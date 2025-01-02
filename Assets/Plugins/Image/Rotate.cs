using UnityEngine;

public class Rotate : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Vector3 rotationAxis = Vector3.up; // ����b�A�q�{�� Y �b
    public float rotationSpeed = 100f; // ����t�� (��/��)

    void Update()
    {
        // ������ફ��
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}
