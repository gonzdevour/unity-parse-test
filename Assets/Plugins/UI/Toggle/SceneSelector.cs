using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class SceneSelector : ToggleSelector
{
    private void OnEnable()
    {
        // 當此腳本啟用時，訂閱 sceneLoaded 事件
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 當此腳本關閉或物件被銷毀時，取消訂閱
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public override void Start()
    {
        base.Start(); // 為toggles賦值
        RefreshToggles();
    }

    /// <summary>
    /// 當任何Scene完成Load(無論Single或Additive模式)後被呼叫
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 若僅想針對Additive模式刷新，可以檢查mode == LoadSceneMode.Additive
        if (mode == LoadSceneMode.Additive && base.toggles != null)
        {
            Debug.Log($"Scene {scene.name} loaded in {mode} mode. Refresh toggles...");
            RefreshToggles();
        }
    }
    public void RefreshToggles() 
    {
        // 將已加載的所有場景名稱收集起來 (可能是多場景 Additive 模式)
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

        // 逐一檢查父類別中收集到的 toggles
        Toggle[] toggles = base.toggles;
        foreach (Toggle toggle in toggles)
        {
            ToggleVariables tv = toggle.GetComponent<ToggleVariables>();
            if (tv == null) continue;

            string targetSceneName = tv.LabelName;
            //Debug.Log($"toggle's LabelName = {targetSceneName}");
            // 如果該 Toggle 對應的場景名稱在目前已加載場景列表中，將它設為 isOn
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
