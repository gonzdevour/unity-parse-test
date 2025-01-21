#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PlayerPrefsEditor : EditorWindow
{
    private Vector2 scrollPosition;
    private Dictionary<string, string> playerPrefsData = new Dictionary<string, string>();

    private static readonly string[] predefinedKeys = new string[]
    {
        "�����ӷ�", "����������|", "�Y�ϯ������|", "�I���������|", "�������C��",
        "�m", "�W", "�٩I", "�魫", "���", "hp", "mp", "�ثe�ɶ�",
        "���p���n�P��", "���j��n�P��", "����", "�J�|����", "�ѥ]����",
        "�d���_�ջ���", "�ߤl�X���Ի���", "MaxHP", "��Ӱ}��"
    };

    [MenuItem("Tools/PlayerPrefs Editor")]
    public static void ShowWindow()
    {
        GetWindow<PlayerPrefsEditor>("PlayerPrefs Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("PlayerPrefs Data", EditorStyles.boldLabel);

        if (GUILayout.Button("Load PlayerPrefs"))
        {
            LoadPlayerPrefs();
        }

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));

        foreach (var kvp in playerPrefsData)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(kvp.Key, GUILayout.Width(250));
            GUILayout.Label(kvp.Value, GUILayout.Width(250));

            if (GUILayout.Button("Delete", GUILayout.Width(100)))
            {
                PlayerPrefs.DeleteKey(kvp.Key);
                PlayerPrefs.Save();
                LoadPlayerPrefs();
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();

        if (GUILayout.Button("Clear All PlayerPrefs"))
        {
            if (EditorUtility.DisplayDialog("Warning", "Are you sure you want to delete all PlayerPrefs?", "Yes", "No"))
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
                LoadPlayerPrefs();
            }
        }
    }

    private void LoadPlayerPrefs()
    {
        playerPrefsData.Clear();

        foreach (string key in predefinedKeys)
        {
            if (PlayerPrefs.HasKey(key))
            {
                playerPrefsData[key] = PlayerPrefs.GetString(key);
            }
        }

        Repaint();
    }
}
#endif
