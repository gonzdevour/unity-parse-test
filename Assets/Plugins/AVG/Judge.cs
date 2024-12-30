using System;
using UnityEngine;

public class Judge
{
    // 主函數：解析並判斷條件
    public static bool EvaluateCondition(string condition)
    {
        Debug.Log($"開始解析條件: \n{condition}");

        // 將條件以行拆分，每行代表一組&&條件
        string[] lines = condition.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            Debug.Log($"解析行條件: {line}");

            // 將每行條件拆分為 && 條件組
            string[] andConditions = line.Split(new[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string andCondition in andConditions)
            {
                Debug.Log($"解析 AND 條件: {andCondition}");

                // 將 && 條件中的每個子條件進行判斷
                if (!EvaluateOrConditions(andCondition))
                {
                    // 如果任一條件不成立，返回 false
                    Debug.Log($"條件不成立: {andCondition}");
                    return false;
                }
            }
        }

        // 如果所有條件都成立，返回 true
        Debug.Log("所有條件都成立。");
        return true;
    }

    // 判斷一行中的 OR 條件
    private static bool EvaluateOrConditions(string andCondition)
    {
        Debug.Log($"解析 OR 條件: {andCondition}");

        // 將條件按 || 分割
        string[] orConditions = andCondition.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string orCondition in orConditions)
        {
            Debug.Log($"解析單一 OR 子條件: {orCondition.Trim()}");

            // 若任一 OR 子條件成立，則整個 OR 條件為真
            if (EvaluateSingleCondition(orCondition.Trim()))
            {
                Debug.Log($"OR 子條件成立: {orCondition.Trim()}");
                return true;
            }
        }

        // 所有 OR 子條件都不成立，返回 false
        Debug.Log($"所有 OR 子條件都不成立: {andCondition}");
        return false;
    }

    // 判斷單一條件
    public static bool EvaluateSingleCondition(string condition)
    {
        Debug.Log($"解析單一條件: {condition}");

        // 將單一 "=" 替換為 "=="，確保不影響其他運算符
        if (condition.Contains("=") && !condition.Contains("==") && !condition.Contains(">=") && !condition.Contains("<="))
        {
            Debug.Log($"將單一 '=' 替換為 '==': {condition}");
            condition = condition.Replace("=", "==");
        }

        // 支援的運算符列表
        string[] operators = { "==", ">=", "<=", ">", "<", "!=" };

        foreach (string op in operators)
        {
            if (condition.Contains(op))
            {
                // 拆分條件：key 和 比較值
                string[] parts = condition.Split(new[] { op }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    // 如果值未被雙引號包裹且無法解析為數值，則嘗試從 PlayerPrefs 獲取值
                    if ((!value.StartsWith("\"") || !value.EndsWith("\"")) && !int.TryParse(value, out _) && !float.TryParse(value, out _))
                    {
                        value = PlayerPrefs.GetString(value, value);
                    }

                    Debug.Log($"條件拆分: key={key}, value={value}, op={op}");

                    // 從 PlayerPrefs 讀取 key 的值
                    if (int.TryParse(value, out int intValue))
                    {
                        string ppValue = PlayerPrefs.GetString(key, string.Empty);
                        int playerValue = int.Parse(ppValue);
                        bool result = CompareValues(playerValue, intValue, op);
                        PrintDebugMessage(condition, result);
                        return result;
                    }
                    else if (float.TryParse(value, out float floatValue))
                    {
                        string ppValue = PlayerPrefs.GetString(key, string.Empty);
                        float playerValue = float.Parse(ppValue);
                        bool result = CompareValues(playerValue, floatValue, op);
                        PrintDebugMessage(condition, result);
                        return result;
                    }
                    else
                    {
                        string playerValue = PlayerPrefs.GetString(key, string.Empty);
                        bool result = CompareValues(playerValue, value, op);
                        PrintDebugMessage(condition, result);
                        return result;
                    }
                }
                else
                {
                    Debug.LogError($"條件格式錯誤，無法拆分為 key 和 value: {condition}");
                    return false;
                }
            }
        }

        Debug.LogError($"條件中不包含有效的運算符: {condition}");
        return false;
    }

    // 比較數值型條件
    private static bool CompareValues<T>(T playerValue, T conditionValue, string op) where T : IComparable<T>
    {
        switch (op)
        {
            case "==": return playerValue.CompareTo(conditionValue) == 0;
            case "!=": return playerValue.CompareTo(conditionValue) != 0;
            case ">": return playerValue.CompareTo(conditionValue) > 0;
            case "<": return playerValue.CompareTo(conditionValue) < 0;
            case ">=": return playerValue.CompareTo(conditionValue) >= 0;
            case "<=": return playerValue.CompareTo(conditionValue) <= 0;
            default: throw new ArgumentException($"未知的運算符: {op}");
        }
    }

    // 打印 Debug 訊息
    private static void PrintDebugMessage(string condition, bool result)
    {
        if (result)
        {
            Debug.Log($"passed: {condition}");
        }
        else
        {
            Debug.Log($"not passed: {condition}");
        }
    }
}
