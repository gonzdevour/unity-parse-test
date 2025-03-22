using UnityEngine;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System;
using System.Text;

public partial class Director
{
    // 執行函數
    public void ExecuteActionPackage(string[] commands)
    {
        if (commands == null || commands.Length == 0)
        {
            Debug.LogWarning("No commands provided for execution.");
            return;
        }

        foreach (var command in commands)
        {
            if (!string.IsNullOrWhiteSpace(command))
            {
                Debug.Log($"[Pkg執行]: {command}");
                ExecuteAction(command); // 調用單個指令的執行函數
            }
            else
            {
                Debug.LogWarning("Skipped empty or null command.");
            }
        }
    }

    public void ExecuteAction(string command)
    {
        command = PreprocessCommand(command);// 解析command並重組
        Debug.Log($"執行解析後cmd:{command}");

        var parts = command.Split('|').Select(part => part.Trim()).ToArray(); // Trim處理

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

    public string PreprocessCommand(string command)
    {
        //Debug.Log($"開始處理指令：{command}");

        // 1. 判斷是否為運算式且不包含 "," 與 "=" (ex: 金錢-蛋糕價格)
        if (!command.Contains(",") && !command.Contains("=") && IsExpression(command))
        {
            // 找到第一個運算符的位置
            int operatorIndex = command.IndexOfAny(new[] { '+', '-', '*', '/' });
            if (operatorIndex > 0)
            {
                // 以運算符分割 command
                string actionNamePre = command.Substring(0, operatorIndex).Trim(); // ex: "金錢"
                string expressionPre = command.Substring(operatorIndex).Trim();    // ex: "-蛋糕價格"

                // 重組為 "actionName,actionName+expression" (ex: 轉換成："金錢,金錢-蛋糕價格")
                command = $"{actionNamePre},{actionNamePre}{expressionPre}";
                //Debug.Log($"重組運算式指令: {command}");
            }
        }
        // 2. 分割指令
        //var parts = command.Split(',').Select(part => part.Trim()).ToArray(); // Trim處理
        var parts = ParseQuotedLine(command, ',');
        Debug.Log($"Preprocess parts: [{string.Join("], [", parts)}]");

        if (parts.Length == 0)
        {
            Debug.LogWarning("Command is empty.");
            return command; // 返回原始指令
        }

        string actionName;
        // 3. 處理第一個 part（函數名稱）
        //var firstPart = parts[0].Split('=').Select(part => part.Trim()).ToArray(); // Trim處理
        var firstPart = ParseQuotedLine(parts[0], '=');
        if (firstPart.Length > 1)
        {
            // 如果有 "="，取等號前的部分作為函數名稱
            actionName = firstPart[0];

            // 將等號後的部分拆分並插入到 parts 陣列中
            List<string> newParts = new List<string> { actionName }; // 新陣列的第一部分是函數名稱
            newParts.AddRange(firstPart[1..]); // 插入等號後的部分
            newParts.AddRange(parts[1..]); // 插入原本 parts 的剩餘部分

            // 將 newParts 更新回 parts
            parts = newParts.ToArray();
        }
        else
        {
            // 如果沒有 "="，第一個 part 本身就是函數名稱
            actionName = parts[0];
        }
        Debug.Log($"函數名稱：{actionName}");

        // 4. 處理剩餘的 parts
        for (int i = 1; i < parts.Length; i++)
        {
            string part = parts[i];

            // 如果是被雙引號包裹，直接取值為純字串
            if (part.StartsWith("\"") && part.EndsWith("\""))
            {
                parts[i] = part.Trim('\"');
                Debug.Log($"{part}被雙引號包裹");
            }
            else
            {
                // 否則嘗試從 PlayerPrefs 中取值
                string prefValue = PPM.Inst.Get(part, part); // 默認返回 part 本身
                Debug.Log($"{part}沒有被雙引號包裹，嘗試從PP取值：{prefValue}");
                parts[i] = prefValue;

                // 如果是算式，嘗試解析並計算
                if (IsExpression(prefValue))
                {
                    //Debug.Log($"{prefValue}是算式");
                    try
                    {
                        float result = EvaluateExpression(prefValue);
                        parts[i] = result.ToString();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to evaluate expression: {prefValue}, Error: {ex.Message}");
                    }
                }
            }
        }

        // 5. 拼接最終指令
        string resultCmd = string.Join("|", new[] { actionName }.Concat(parts[1..]));
        Debug.Log($"拼接最終指令：{resultCmd}");
        return resultCmd;
    }

    private bool IsExpression(string input)
    {
        // 如果整個字串都被引號包起來，則不是表達式
        if (IsQuoted(input))
            return false;

        // 檢查是否存在運算符
        bool hasOperator = false;
        bool inQuotes = false;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            // 處理引號
            if (c == '"')
            {
                // 檢查是否為逃脫的引號 (兩個連續引號)
                if (i + 1 < input.Length && input[i + 1] == '"')
                {
                    i++; // 跳過下一個引號
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            // 檢查不在引號內的運算符
            else if (!inQuotes && (c == '+' || c == '-' || c == '*' || c == '/'))
            {
                hasOperator = true;
                break;
            }
        }
        Debug.Log($"{input} 是運算式? {hasOperator}");
        return hasOperator;
    }

    private bool IsQuoted(string input)
    {
        if (input.StartsWith("\"") && input.EndsWith("\""))
        {
            Debug.Log($"{input}被引號包圍");
            return true;
        }
        else
        {
            Debug.Log($"{input}沒有被引號包圍");
            return false;
        }
    }

    public static string[] ParseQuotedLine(string line, char seperator)
    {
        if (string.IsNullOrEmpty(line))
            return Array.Empty<string>();

        List<string> result = new List<string>();
        StringBuilder currentField = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            // 處理引號
            if (c == '"')
            {
                Debug.Log("有引號");
                // 檢查是否為逃脫的引號 (兩個連續引號)
                if (i + 1 < line.Length && line[i + 1] == '"')
                {
                    currentField.Append('"');
                    i++; // 跳過下一個引號
                }
                else
                {
                    currentField.Append('"');
                    inQuotes = !inQuotes;
                }
            }
            // 處理分隔符號 (不在引號內)
            else if (c == seperator && !inQuotes)
            {
                result.Add(currentField.ToString()); // 保留完整內容 (不 Trim)
                currentField.Clear();
            }
            // 處理一般字元
            else
            {
                currentField.Append(c);
            }
            Debug.Log(currentField);
        }

        // 添加最後一個欄位
        result.Add(currentField.ToString());

        return result.ToArray();
    }

    private float EvaluateExpression(string expression)
    {
        // 支援的運算符
        char[] operators = { '+', '-', '*', '/' };

        // 分割表達式並保留運算符
        List<string> tokens = new List<string>();
        int lastIndex = 0;

        for (int i = 0; i < expression.Length; i++)
        {
            if (operators.Contains(expression[i]))
            {
                // 將運算數和運算符分別加入
                tokens.Add(expression.Substring(lastIndex, i - lastIndex).Trim());
                tokens.Add(expression[i].ToString());
                lastIndex = i + 1;
            }
        }

        // 最後一個運算數
        if (lastIndex < expression.Length)
        {
            tokens.Add(expression.Substring(lastIndex).Trim());
        }

        // 處理每個 token
        for (int i = 0; i < tokens.Count; i++)
        {
            if (!operators.Contains(tokens[i][0]))
            {
                // 非運算符，視為變數名稱或數值
                string token = tokens[i].Trim();

                // 從 PlayerPrefs 嘗試取值，默認為 token 本身
                if (!PlayerPrefs.HasKey(token))
                {
                    Debug.LogWarning($"[EvaluateExpression]{token}無值");
                }
                string value = PPM.Inst.Get(token, "0");

                // 嘗試將值解析為浮點數
                if (float.TryParse(value, out float number))
                {
                    tokens[i] = number.ToString(); // 將解析結果更新回 token
                }
                else
                {
                    throw new FormatException($"Invalid value for token: {token}");
                }
            }
        }

        // 重組表達式
        string processedExpression = string.Join("", tokens);

        // 計算表達式結果
        DataTable table = new DataTable();
        object result = table.Compute(processedExpression, string.Empty);
        return Convert.ToSingle(result);
    }
}
