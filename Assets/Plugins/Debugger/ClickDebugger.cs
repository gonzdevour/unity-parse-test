using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ClickDebugger : MonoBehaviour
{
    void Update()
    {
        // 偵測滑鼠左鍵按下
        if (Input.GetMouseButtonDown(0))
        {
            // 建立一個 PointerEventData，模擬目前的滑鼠位置
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            // 用來接收 Raycast 结果
            List<RaycastResult> results = new List<RaycastResult>();

            // 呼叫 RaycastAll，取得所有被擊中的 UI
            EventSystem.current.RaycastAll(pointerData, results);

            // 印出被擊中的所有物件
            if (results.Count == 0)
            {
                Debug.Log("No UI element is blocking the click.");
            }
            else
            {
                foreach (var result in results)
                {
                    Debug.Log($"UI element blocking click: {result.gameObject.name}");
                }
            }
        }
    }
}
