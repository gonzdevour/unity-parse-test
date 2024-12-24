using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
    public GameObject Cover;
    private Image coverImg;
    private readonly float Duration = 0.2f;
    public string[] sceneNames;
    private string currentScene;
    private string lastScene;

    public static SceneSwitcher Inst { get; private set; }
    private void Awake()
    {
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        coverImg = Cover.GetComponent<Image>();
        Canvas globalCanvas = coverImg.GetComponentInParent<Canvas>();
        globalCanvas.overrideSorting = true;
        globalCanvas.sortingOrder = 100; // 確保排序在其他 Canvas 之上
    }

    void Start()
    {
        currentScene = null;
        // 確認目前已加載的場景數量，只有從global啟動時才會自動前往Menu，否則只需要FadeIn coverImg即可
        if (SceneManager.sceneCount == 1 && SceneManager.GetSceneAt(0).name == "Global")
        {
            SwitchScene("Menu");
        }
        else
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name != "Global")
                {
                    currentScene = scene.name; // 返回第一個不是 Global 的場景名稱
                    lastScene = scene.name; //current和last是同個scene，back button不起作用
                    FadeIn();
                }
            }
        }
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
            //Debug.Log($"當前場景{currentScene}非空且已經載入，執行FadeOut。");
            FadeOut(targetScene);
        }
        else
        {
            //Debug.Log($"當前場景{currentScene}為空或尚未載入。直接讀入目標場景{targetScene}");
            LoadTargetScene(targetScene);
        }
    }

    private void LoadTargetScene(string targetScene)
    {
        if (SceneManager.GetSceneByName(targetScene).isLoaded)
        {
            coverImg.DOFade(0, 0).SetEase(Ease.InQuad);

            //上一個scene=目前的scene，目前的scene=目標的scene
            lastScene = currentScene;
            currentScene = targetScene;

            Debug.LogWarning($"場景 {targetScene} 已加載，無需重複加載。");
            return;
        }

        SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive).completed += (op) =>
        {
            //Debug.Log($"場景 {targetScene} 加載完成。");
            //上一個scene=目前的scene，目前的scene=目標的scene
            lastScene = currentScene;
            currentScene = targetScene;
            FadeIn();
        };
    }

    public void FadeIn()
    {
        if (coverImg == null)
        {
            Debug.LogError("FadeIn 無法執行，因為 Cover 未正確設置！");
            return;
        }
        coverImg.DOFade(0, Duration).SetEase(Ease.InQuad);
    }

    public void FadeOut(string targetScene)
    {
        if (coverImg == null)
        {
            Debug.LogError("FadeOut 無法執行，因為 Cover 未正確設置！");
            return;
        }

        coverImg.DOFade(1, Duration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            SceneManager.UnloadSceneAsync(currentScene).completed += (op) =>
            {
                Debug.Log($"卸載{currentScene}場景，讀取{targetScene}場景");
                LoadTargetScene(targetScene);
            };
        });
    }
}
