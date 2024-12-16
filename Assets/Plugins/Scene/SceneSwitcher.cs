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

    private void Awake()
    {
        coverImg = GetComponent<Image>();
        Canvas globalCanvas = coverImg.GetComponentInParent<Canvas>();
        globalCanvas.overrideSorting = true;
        globalCanvas.sortingOrder = 100; // �T�O�ƧǦb��L Canvas ���W
    }
    void Start()
    {
        currentScene = null;
        // �T�{�ثe�w�[���������ƶq�A�u���qglobal�Ұʮɤ~�|�۰ʫe��Menu�A�_�h�u�ݭnFadeIn coverImg�Y�i
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
                    currentScene = scene.name; // ��^�Ĥ@�Ӥ��O Global �������W��
                    FadeIn();
                }
            }
        }
    }

    public void SwitchScene(string targetScene)
    {
        if (currentScene == targetScene)
        {
            Debug.Log($"�ؼг��� {targetScene} �w�O��e�����A�L�ݤ����C");
            return;
        }

        if (!string.IsNullOrEmpty(currentScene) && SceneManager.GetSceneByName(currentScene).isLoaded)
        {
            //Debug.Log($"��e����{currentScene}�D�ťB�w�g���J�A����FadeOut�C");
            FadeOut(targetScene);
        }
        else
        {
            //Debug.Log($"��e����{currentScene}���ũΩ|�����J�C����Ū�J�ؼг���{targetScene}");
            LoadTargetScene(targetScene);
        }
    }

    private void LoadTargetScene(string targetScene)
    {
        if (SceneManager.GetSceneByName(targetScene).isLoaded)
        {
            coverImg.DOFade(0, 0).SetEase(Ease.InQuad);
            currentScene = targetScene;
            Debug.LogWarning($"���� {targetScene} �w�[���A�L�ݭ��ƥ[���C");
            return;
        }

        SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive).completed += (op) =>
        {
            //Debug.Log($"���� {targetScene} �[�������C");
            currentScene = targetScene;
            FadeIn();
        };
    }

    public void FadeIn()
    {
        if (coverImg == null)
        {
            Debug.LogError("FadeIn �L�k����A�]�� Cover �����T�]�m�I");
            return;
        }
        coverImg.DOFade(0, Duration).SetEase(Ease.InQuad);
    }

    public void FadeOut(string targetScene)
    {
        if (coverImg == null)
        {
            Debug.LogError("FadeOut �L�k����A�]�� Cover �����T�]�m�I");
            return;
        }

        coverImg.DOFade(1, Duration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            SceneManager.UnloadSceneAsync(currentScene).completed += (op) =>
            {
                Debug.Log($"����{currentScene}�����AŪ��{targetScene}����");
                LoadTargetScene(targetScene);
            };
        });
    }
}
