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
        globalCanvas.sortingOrder = 100; // �T�O�ƧǦb��L Canvas ���W

        // �w�]�ϥβH�J�H�X�S��
        transitionEffect = new FadeTransitionEffect(ScreenImg, Duration);
    }

    void Start()
    {
        currentScene = null;
        if (SceneManager.sceneCount == 1 && SceneManager.GetSceneAt(0).name == "Global")
        {
            // �qGlobal�ҰʮɡA�e��firstScene
            SwitchScene(firstScene);
        }
        else
        {
            // ���O�qGlobal�ҰʮɡA���Ұʪ�scene�AFadeIn
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
        // �Q�������ؼг����w�g�O��e�����A������
        if (currentScene == targetScene)
        {
            return;
        }

        // �Q�������ؼг������O��e�����A�}�l����
        if (!string.IsNullOrEmpty(currentScene) && SceneManager.GetSceneByName(currentScene).isLoaded)
        {
            // ��e�����r��D�ťB��e�����s�b�A����FadeOut�������e�����å[���ؼг���
            transitionEffect.FadeOut(() =>
            {
                SceneManager.UnloadSceneAsync(currentScene).completed += (op) =>
                {
                    Debug.Log($"�������� {currentScene} �����C");
                    LoadTargetScene(targetScene);
                };
            });
        }
        else
        {
            // ��e�����r�ꬰ�ũη�e�������s�b�A������FadeOut�A����Ū���ؼг���
            LoadTargetScene(targetScene);
        }
    }

    private void LoadTargetScene(string targetScene)
    {
        // �p�G�ؼг����w�s�b�A�ߧY����Fade�A�N�ؼг����]�w���ثe����
        if (SceneManager.GetSceneByName(targetScene).isLoaded)
        {
            ScreenImg.DOFade(0, 0).SetEase(Ease.InQuad);

            lastScene = currentScene;
            currentScene = targetScene;
            Debug.LogWarning($"���� {targetScene} �w�[���A�L�ݭ��ƥ[���C");
            return;
        }
        // �p�G�ؼг������s�b�A�HAdditive�Ҧ��[�������A���������FadeIn
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
            Debug.LogError("FadeIn �L�k����A�]�� Cover �����T�]�m�I");
            return;
        }
        ScreenImg.DOFade(0, duration).SetEase(Ease.InQuad);
    }

    public void FadeOut(System.Action onComplete)
    {
        if (ScreenImg == null)
        {
            Debug.LogError("FadeOut �L�k����A�]�� Cover �����T�]�m�I");
            return;
        }

        ScreenImg.DOFade(1, duration).SetEase(Ease.OutQuad).OnComplete(() => onComplete?.Invoke());
    }
}

// ���ӥi�X�i��L�S�ġA�p�Y��B���൥
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
