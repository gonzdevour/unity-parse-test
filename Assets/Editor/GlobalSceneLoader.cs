#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class AutoGlobalSceneLoader
{
    private const string GlobalSceneName = "Global";

    static AutoGlobalSceneLoader()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            if (!SceneManager.GetSceneByName(GlobalSceneName).isLoaded)
            {
                Debug.Log($"Auto-loading Global Scene: {GlobalSceneName}");
                SceneManager.LoadScene(GlobalSceneName, LoadSceneMode.Additive);
            }
        }
    }
}
#endif