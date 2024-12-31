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

        // Unity Text �r�����
        selectedUnityFont = (Font)EditorGUILayout.ObjectField(
            "Unity Font",
            selectedUnityFont,
            typeof(Font),
            false
        );

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Line Spacing", EditorStyles.boldLabel);

        // Line Spacing �]�w
        lineSpacing = EditorGUILayout.FloatField("Line Spacing", lineSpacing);

        EditorGUILayout.Space(20);

        // ���Ϋ��s
        if (GUILayout.Button("Apply Changes to All Scenes", GUILayout.Height(40)))
        {
            ApplyChangesToAllScenes();
        }
    }

    void ApplyChangesToAllScenes()
    {
        // �T�{�O�_����
        if (!EditorUtility.DisplayDialog(
            "�T�{�ק�",
            "�O�_�T�w�n�N `Assets/Scenes` ��Ƨ����Ҧ���r���r���P��Z�i���s�H",
            "�T�w",
            "����"))
        {
            return;
        }

        // �O�s��e�}�Ҫ�����
        string currentScenePath = EditorSceneManager.GetActiveScene().path;

        // �j�M `Assets/Scenes` ��Ƨ����������ɮ�
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });

        int processedScenes = 0;

        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);

            // �[������
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            // �M���������Ҧ��ڪ���û��j�B�z
            foreach (GameObject rootObject in scene.GetRootGameObjects())
            {
                ApplyChangesToGameObject(rootObject);
            }

            // �O�s����
            EditorSceneManager.SaveScene(scene);

            processedScenes++;
            EditorUtility.DisplayProgressBar(
                "Updating Text Properties",
                $"Processing scene: {scenePath} ({processedScenes}/{sceneGuids.Length})",
                (float)processedScenes / sceneGuids.Length);
        }

        // �^������
        if (!string.IsNullOrEmpty(currentScenePath))
        {
            EditorSceneManager.OpenScene(currentScenePath);
        }

        // �M���i�ױ�
        EditorUtility.ClearProgressBar();

        // ��������
        EditorUtility.DisplayDialog(
            "Text Properties Update",
            "�w�N `Assets/Scenes` ��Ƨ����Ҧ���r���r���P��Z�i���s�C",
            "�T�w"
        );
    }

    void ApplyChangesToGameObject(GameObject obj)
    {
        // �ק� Unity Text ���ݩ�
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

        // ���j�B�z�l����
        foreach (Transform child in obj.transform)
        {
            ApplyChangesToGameObject(child.gameObject);
        }
    }
}
#endif
