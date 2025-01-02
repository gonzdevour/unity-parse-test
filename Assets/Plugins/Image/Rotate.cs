using UnityEngine;

public class Rotate : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Vector3 rotationAxis = Vector3.up; // 旋轉軸，默認為 Y 軸
    public float rotationSpeed = 100f; // 旋轉速度 (度/秒)

    void Update()
    {
        // 持續旋轉物件
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}
