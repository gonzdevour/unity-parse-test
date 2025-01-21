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
        "素材來源", "角色素材路徑", "頭圖素材路徑", "背景素材路徑", "表情類型列表",
        "姓", "名", "稱呼", "體重", "日期", "hp", "mp", "目前時間",
        "王小美好感度", "李大花好感度", "金錢", "蛋糕價格", "麵包價格",
        "卡布奇諾價格", "栗子蒙布朗價格", "MaxHP", "獲勝陣營"
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
