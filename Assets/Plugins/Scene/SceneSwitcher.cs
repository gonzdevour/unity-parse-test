using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
    public GameObject UnmaskCtnr;
    public Image ScreenImg;
    public float DurOut = 0.5f;
    public float DurIn = 0.5f;
    public Ease EaseOut = Ease.OutQuad;
    public Ease EaseIn = Ease.InQuad;
    public string[] sceneNames;
    private string firstScene = "Menu";
    private string currentScene;
    private string lastScene;

    private ITransitionEffect transitionEffect;

    public static SceneSwitcher Inst { get; private set; }

    private void Awake()
    {
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        Canvas globalCanvas = ScreenImg.GetComponentInParent<Canvas>();
        globalCanvas.overrideSorting = true;
        globalCanvas.sortingOrder = 100; // 確保排序在其他 Canvas 之上

        // 預設使用淡入淡出特效
        transitionEffect = new TransitionEffectCircle(UnmaskCtnr, ScreenImg, DurOut, DurIn, EaseOut, EaseIn);
    }

    void Start()
    {
        currentScene = null;
        if (SceneManager.sceneCount == 1 && SceneManager.GetSceneAt(0).name == "Global")
        {
            // 從Global啟動時，前往firstScene
            SwitchScene(firstScene);
        }
        else
        {
            // 不是從Global啟動時，找到啟動的scene，FadeIn
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name != "Global")
                {
                    currentScene = scene.name;
                    lastScene = scene.name;
                    transitionEffect.FadeIn();
                }
            }
        }
    }

    public void SwitchScene(string targetScene)
    {
        // 想切換的目標場景已經是當前場景，不切換
        if (currentScene == targetScene)
        {
            return;
        }

        // 想切換的目標場景不是當前場景，開始切換
        if (!string.IsNullOrEmpty(currentScene) && SceneManager.GetSceneByName(currentScene).isLoaded)
        {
            // 當前場景字串非空且當前場景存在，執行FadeOut後卸載當前場景並加載目標場景
            transitionEffect.FadeOut(() =>
            {
                SceneManager.UnloadSceneAsync(currentScene).completed += (op) =>
                {
                    Debug.Log($"卸載場景 {currentScene} 完成。");
                    LoadTargetScene(targetScene);
                };
            });
        }
        else
        {
            // 當前場景字串為空或當前場景不存在，不執行FadeOut，直接讀取目標場景
            LoadTargetScene(targetScene);
        }
    }

    private void LoadTargetScene(string targetScene)
    {
        // 如果目標場景已存在，立即停止Fade，將目標場景設定為目前場景
        if (SceneManager.GetSceneByName(targetScene).isLoaded)
        {
            transitionEffect.Stop();//立即完成並停止fade

            lastScene = currentScene;
            currentScene = targetScene;
            Debug.LogWarning($"場景 {targetScene} 已加載，無需重複加載。");
            return;
        }
        // 如果目標場景不存在，以Additive模式加載場景，完成後執行FadeIn
        SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive).completed += (op) =>
        {
            lastScene = currentScene;
            currentScene = targetScene;
            transitionEffect.FadeIn();
        };
    }

    public void SetTransitionEffect(ITransitionEffect effect)
    {
        transitionEffect = effect;
    }
}
