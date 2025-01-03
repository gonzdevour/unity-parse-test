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
        globalCanvas.sortingOrder = 100; // �T�O�ƧǦb��L Canvas ���W

        // �w�]�ϥβH�J�H�X�S��
        transitionEffect = new TransitionEffectCircle(UnmaskCtnr, ScreenImg, DurOut, DurIn, EaseOut, EaseIn);
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
            transitionEffect.Stop();//�ߧY�����ð���fade

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
