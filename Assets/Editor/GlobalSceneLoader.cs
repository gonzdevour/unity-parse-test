#if UNITY_EDITOR
using UnityEditor; // 提供 Unity 編輯器相關功能的命名空間
using UnityEngine; // 提供 Unity 引擎核心功能的命名空間
using UnityEngine.SceneManagement; // 提供場景管理相關功能的命名空間

// 標記類在編輯器模式下初始化的屬性
[InitializeOnLoad]
public class AutoGlobalSceneLoader
{
    // 常數，用於指定全局場景的名稱
    private const string GlobalSceneName = "Global";

    // 靜態構造函數，當編輯器加載此類時會自動執行
    static AutoGlobalSceneLoader()
    {
        // 訂閱編輯器的遊戲模式狀態變更事件
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    // 當遊戲模式狀態發生變化時觸發的回調函數
    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // 判斷是否進入遊戲模式
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            // 檢查全局場景是否已加載
            if (!SceneManager.GetSceneByName(GlobalSceneName).isLoaded)
            {
                // 如果未加載，輸出日誌信息
                Debug.Log($"Auto-loading Global Scene: {GlobalSceneName}");

                // 加載全局場景，使用附加模式（保留當前場景）
                SceneManager.LoadScene(GlobalSceneName, LoadSceneMode.Additive);
            }
        }
    }
}
#endif
