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
        // Global Canvas �]�m
        Canvas globalCanvas = FindObjectOfType<Canvas>();
        globalCanvas.overrideSorting = true;
        globalCanvas.sortingOrder = 100; // �T�O�ƧǦb��L Canvas ���W

        currentScene = null;
        SwitchScene("Menu");
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
            Debug.LogWarning($"���� {targetScene} �w�[���A�L�ݭ��ƥ[���C");
            return;
        }

        SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive).completed += (op) =>
        {
            Debug.Log($"���� {targetScene} �[�������C");
            currentScene = targetScene;
            FadeIn();
        };
    }

    public void FadeIn()
    {
        if (Cover == null)
        {
            Debug.LogError("FadeIn �L�k����A�]�� Cover �����T�]�m�I");
            return;
        }

        Debug.Log($"FadeIn Dur:{Duration}");
        Cover.DOFade(0, Duration).SetEase(Ease.InQuad);
    }

    public void FadeOut(string targetScene)
    {
        if (Cover == null)
        {
            Debug.LogError("FadeOut �L�k����A�]�� Cover �����T�]�m�I");
            return;
        }

        Cover.DOFade(1, Duration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            SceneManager.UnloadSceneAsync(currentScene).completed += (op) =>
            {
                Debug.Log($"����{currentScene}�AŪ��{targetScene}");
                LoadTargetScene(targetScene);
            };
        });
    }
}
