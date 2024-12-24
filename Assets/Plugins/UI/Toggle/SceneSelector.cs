using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class SceneSelector : ToggleSelector
{
    private void OnEnable()
    {
        // ���}���ҥήɡA�q�\ sceneLoaded �ƥ�
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // ���}�������Ϊ���Q�P���ɡA�����q�\
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public override void Start()
    {
        base.Start(); // ��toggles���
        RefreshToggles();
    }

    /// <summary>
    /// �����Scene����Load(�L��Single��Additive�Ҧ�)��Q�I�s
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // �Y�ȷQ�w��Additive�Ҧ���s�A�i�H�ˬdmode == LoadSceneMode.Additive
        if (mode == LoadSceneMode.Additive && base.toggles != null)
        {
            Debug.Log($"Scene {scene.name} loaded in {mode} mode. Refresh toggles...");
            RefreshToggles();
        }
    }
    public void RefreshToggles() 
    {
        // �N�w�[�����Ҧ������W�٦����_�� (�i��O�h���� Additive �Ҧ�)
        HashSet<string> loadedSceneNames = new HashSet<string>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.isLoaded)
            {
                loadedSceneNames.Add(scene.name);
            }
        }
        //Debug.Log($"loadedSceneNames:{string.Join(", ", loadedSceneNames)}");

        // �v�@�ˬd�����O�������쪺 toggles
        Toggle[] toggles = base.toggles;
        foreach (Toggle toggle in toggles)
        {
            ToggleVariables tv = toggle.GetComponent<ToggleVariables>();
            if (tv == null) continue;

            string targetSceneName = tv.LabelName;
            //Debug.Log($"toggle's LabelName = {targetSceneName}");
            // �p�G�� Toggle �����������W�٦b�ثe�w�[�������C���A�N���]�� isOn
            if (loadedSceneNames.Contains(targetSceneName))
            {
                //Debug.Log($"{string.Join(",", loadedSceneNames)} contains {targetSceneName}");
                toggle.isOn = true;
            }
        }
    }

    public override void OnToggleValueChanged(Toggle toggle, bool isOn)
    {
        base.OnToggleValueChanged(toggle, isOn);
        if (isOn) 
        {
            string targetSceneName = toggle.GetComponent<ToggleVariables>().LabelName;
            SceneSwitcher.Inst.SwitchScene(targetSceneName);
        }
    }
}
