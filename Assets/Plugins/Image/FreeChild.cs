using UnityEngine;

public class FreeChild : MonoBehaviour
{
    private Vector3 initialBackgroundPosition;  // 背景的初始世界座標
    private Quaternion initialBackgroundRotation;  // 背景的初始旋轉

    void Start()
    {
        // 保存背景的初始世界座標和旋轉
        initialBackgroundPosition = transform.position;
        initialBackgroundRotation = transform.rotation;
    }

    void LateUpdate()
    {
        // 每幀更新背景的世界座標和旋轉，保持固定
        transform.position = initialBackgroundPosition;
        transform.rotation = initialBackgroundRotation;
    }
}
