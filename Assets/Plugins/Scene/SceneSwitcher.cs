using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
    public Transform UnmaskTransform;
    public Image ScreenImg;
    private readonly float Duration = 0.2f;
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
        transitionEffect = new FadeTransitionEffect(ScreenImg, Duration);
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
            ScreenImg.DOFade(0, 0).SetEase(Ease.InQuad);

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

public interface ITransitionEffect
{
    void FadeIn();
    void FadeOut(System.Action onComplete);
}

public class FadeTransitionEffect : ITransitionEffect
{
    private Image ScreenImg;
    private float duration;

    public FadeTransitionEffect(Image ScreenImg, float duration)
    {
        this.ScreenImg = ScreenImg;
        this.duration = duration;
    }

    public void FadeIn()
    {
        if (ScreenImg == null)
        {
            Debug.LogError("FadeIn 無法執行，因為 Cover 未正確設置！");
            return;
        }
        ScreenImg.DOFade(0, duration).SetEase(Ease.InQuad);
    }

    public void FadeOut(System.Action onComplete)
    {
        if (ScreenImg == null)
        {
            Debug.LogError("FadeOut 無法執行，因為 Cover 未正確設置！");
            return;
        }

        ScreenImg.DOFade(1, duration).SetEase(Ease.OutQuad).OnComplete(() => onComplete?.Invoke());
    }
}

// 未來可擴展其他特效，如縮放、旋轉等
public class ScaleTransitionEffect : ITransitionEffect
{
    private Transform coverTransform;
    private float duration;

    public ScaleTransitionEffect(Image coverImage, float duration)
    {
        this.coverTransform = coverImage.transform;
        this.duration = duration;
    }

    public void FadeIn()
    {
        coverTransform.localScale = Vector3.one * 1.5f;
        coverTransform.DOScale(Vector3.one, duration).SetEase(Ease.OutQuad);
    }

    public void FadeOut(System.Action onComplete)
    {
        coverTransform.DOScale(Vector3.one * 1.5f, duration).SetEase(Ease.InQuad).OnComplete(() => onComplete?.Invoke());
    }
}
