using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
    public Image Cover;
    private readonly float Duration = 0.5f;
    public string[] sceneNames;
    private string currentScene;

    void Start()
    {
        // Global Canvas 設置
        Canvas globalCanvas = FindObjectOfType<Canvas>();
        globalCanvas.overrideSorting = true;
        globalCanvas.sortingOrder = 100; // 確保排序在其他 Canvas 之上

        currentScene = null;
        SwitchScene("Menu");
    }

    public void SwitchScene(string targetScene)
    {
        if (currentScene == targetScene)
        {
            Debug.Log($"目標場景 {targetScene} 已是當前場景，無需切換。");
            return;
        }

        if (!string.IsNullOrEmpty(currentScene) && SceneManager.GetSceneByName(currentScene).isLoaded)
        {
            FadeOut(targetScene);
        }
        else
        {
            LoadTargetScene(targetScene);
        }
    }

    private void LoadTargetScene(string targetScene)
    {
        if (SceneManager.GetSceneByName(targetScene).isLoaded)
        {
            Debug.LogWarning($"場景 {targetScene} 已加載，無需重複加載。");
            return;
        }

        SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive).completed += (op) =>
        {
            Debug.Log($"場景 {targetScene} 加載完成。");
            currentScene = targetScene;
            FadeIn();
        };
    }

    public void FadeIn()
    {
        if (Cover == null)
        {
            Debug.LogError("FadeIn 無法執行，因為 Cover 未正確設置！");
            return;
        }

        Debug.Log($"FadeIn Dur:{Duration}");
        Cover.DOFade(0, Duration).SetEase(Ease.InQuad);
    }

    public void FadeOut(string targetScene)
    {
        if (Cover == null)
        {
            Debug.LogError("FadeOut 無法執行，因為 Cover 未正確設置！");
            return;
        }

        Cover.DOFade(1, Duration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            SceneManager.UnloadSceneAsync(currentScene).completed += (op) =>
            {
                Debug.Log($"卸載{currentScene}，讀取{targetScene}");
                LoadTargetScene(targetScene);
            };
        });
    }
}
