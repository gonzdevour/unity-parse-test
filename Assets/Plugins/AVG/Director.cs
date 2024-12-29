using System;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour
{
    public static Director Inst { get; private set; }
    private void Awake()
    {
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    // 定義函數字典
    private Dictionary<string, Action<object[]>> actions;

    private void Start()
    {
        // 初始化字典並綁定函數
        actions = new Dictionary<string, Action<object[]>>
        {
            { "表情", args => ChangeExpression(args) },
            { "移動", args => MoveCharacter(args) }
        };

        // 測試
        ExecuteAction("表情,張沐霖,怒");
        ExecuteAction("移動,角色1,10,20");
    }

    // 字典中的函數
    private void ChangeExpression(object[] args)
    {
        string character = args[0]?.ToString();
        string expression = args[1]?.ToString();
        Debug.Log($"Change {character}'s expression to {expression}");
    }

    private void MoveCharacter(object[] args)
    {
        string character = args[0]?.ToString();
        float x = float.Parse(args[1].ToString());
        float y = float.Parse(args[2].ToString());
        Debug.Log($"Move {character} to position ({x}, {y})");
    }

    // 執行函數
    public void ExecuteAction(string command)
    {
        var parts = command.Split(',');

        if (parts.Length >= 1 && actions.ContainsKey(parts[0]))
        {
            // 提取動作名稱和參數
            string actionName = parts[0];
            object[] parameters = parts[1..]; // 提取剩下的參數

            // 執行對應的函數
            actions[actionName](parameters);
        }
        else
        {
            Debug.LogWarning("Invalid command or action not found.");
        }
    }
}
