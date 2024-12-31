#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

public class GlobalTextEditorWindow : EditorWindow
{
    private Font selectedUnityFont = null;
    private float lineSpacing = 1f;

    [MenuItem("Tools/Global Text Editor")]
    public static void ShowWindow()
    {
        GetWindow<GlobalTextEditorWindow>("Global Font and Line Spacing");
    }

    void OnGUI()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Global Font and Line Spacing", EditorStyles.boldLabel);

        // Unity Text 字型選擇
        selectedUnityFont = (Font)EditorGUILayout.ObjectField(
            "Unity Font",
            selectedUnityFont,
            typeof(Font),
            false
        );

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Line Spacing", EditorStyles.boldLabel);

        // Line Spacing 設定
        lineSpacing = EditorGUILayout.FloatField("Line Spacing", lineSpacing);

        EditorGUILayout.Space(20);

        // 應用按鈕
        if (GUILayout.Button("Apply Changes to All Scenes", GUILayout.Height(40)))
        {
            ApplyChangesToAllScenes();
        }
    }

    void ApplyChangesToAllScenes()
    {
        // 確認是否執行
        if (!EditorUtility.DisplayDialog(
            "確認修改",
            "是否確定要將 `Assets/Scenes` 資料夾中所有文字的字型與行距進行更新？",
            "確定",
            "取消"))
        {
            return;
        }

        // 保存當前開啟的場景
        string currentScenePath = EditorSceneManager.GetActiveScene().path;

        // 搜尋 `Assets/Scenes` 資料夾中的場景檔案
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });

        int processedScenes = 0;

        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);

            // 加載場景
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            // 遍歷場景中所有根物件並遞迴處理
            foreach (GameObject rootObject in scene.GetRootGameObjects())
            {
                ApplyChangesToGameObject(rootObject);
            }

            // 保存場景
            EditorSceneManager.SaveScene(scene);

            processedScenes++;
            EditorUtility.DisplayProgressBar(
                "Updating Text Properties",
                $"Processing scene: {scenePath} ({processedScenes}/{sceneGuids.Length})",
                (float)processedScenes / sceneGuids.Length);
        }

        // 回到原場景
        if (!string.IsNullOrEmpty(currentScenePath))
        {
            EditorSceneManager.OpenScene(currentScenePath);
        }

        // 清除進度條
        EditorUtility.ClearProgressBar();

        // 完成提示
        EditorUtility.DisplayDialog(
            "Text Properties Update",
            "已將 `Assets/Scenes` 資料夾中所有文字的字型與行距進行更新。",
            "確定"
        );
    }

    void ApplyChangesToGameObject(GameObject obj)
    {
        // 修改 Unity Text 的屬性
        Text textComponent = obj.GetComponent<Text>();
        if (textComponent != null)
        {
            Undo.RecordObject(textComponent, "Adjust Unity Text Properties");
            if (selectedUnityFont != null)
            {
                textComponent.font = selectedUnityFont;
            }
            textComponent.lineSpacing = lineSpacing;
            EditorUtility.SetDirty(textComponent);
        }

        // 遞迴處理子物件
        foreach (Transform child in obj.transform)
        {
            ApplyChangesToGameObject(child.gameObject);
        }
    }
}
#endif
