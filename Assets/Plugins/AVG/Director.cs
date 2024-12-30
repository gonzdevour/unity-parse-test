using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using Unity.Mathematics;
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
        PPM.Inst.Set("金錢", "100");
        PPM.Inst.Set("蛋糕價格", "100");
        PPM.Inst.Set("麵包價格", "60");
        PPM.Inst.Set("卡布奇諾價格", "120");
        PPM.Inst.Set("栗子蒙布朗價格", "150");
        PPM.Inst.Set("MaxHP", "199");
        PPM.Inst.Set("獲勝陣營", "國民革命軍");
        //ExecuteAction("移動,角色1,10,20");
        //ExecuteAction("金錢,100");
        //ExecuteAction("金錢+100-20");
        //ExecuteAction("等級 = 10");
        //ExecuteAction("姓名 = \"\"柴可夫斯基\"\"");
        //ExecuteAction("陣營 = 獲勝陣營");
        //ExecuteAction("HP=MaxHP*50");
        //ExecuteAction("表情,張沐霖,怒");
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
                //Debug.Log($"[Pkg執行]: {command}");
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

        var parts = command.Split(',').Select(part => part.Trim()).ToArray(); // Trim處理

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

        // 1. 判斷是否為運算式且不包含 "," 與 "="
        if (!command.Contains(",") && !command.Contains("=") && IsExpression(command))
        {
            // 找到第一個運算符的位置
            int operatorIndex = command.IndexOfAny(new[] { '+', '-', '*', '/' });
            if (operatorIndex > 0)
            {
                // 以運算符分割 command
                string actionNamePre = command.Substring(0, operatorIndex).Trim();
                string expressionPre = command.Substring(operatorIndex).Trim();

                // 重組為 "actionName,actionName+expression"
                command = $"{actionNamePre},{actionNamePre}{expressionPre}";
                //Debug.Log($"重組運算式指令: {command}");
            }
        }
        // 2. 分割指令
        var parts = command.Split(',').Select(part => part.Trim()).ToArray(); // Trim處理

        if (parts.Length == 0)
        {
            Debug.LogWarning("Command is empty.");
            return command; // 返回原始指令
        }

        string actionName;
        // 3. 處理第一個 part（函數名稱）
        var firstPart = parts[0].Split('=').Select(part => part.Trim()).ToArray(); // Trim處理
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
        //Debug.Log($"函數名稱：{actionName}");

        // 4. 處理剩餘的 parts
        for (int i = 1; i < parts.Length; i++)
        {
            string part = parts[i];

            // 如果是被雙引號包裹，直接取值為純字串
            if (part.StartsWith("\"") && part.EndsWith("\""))
            {
                parts[i] = part.Trim('\"');
                //Debug.Log($"{part}被雙引號包裹");
            }
            else
            {
                // 否則嘗試從 PlayerPrefs 中取值
                string prefValue = PPM.Inst.Get(part, part); // 默認返回 part 本身
                //Debug.Log($"{part}沒有被雙引號包裹，嘗試從PP取值：{prefValue}");
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
        string resultCmd = string.Join(",", new[] { actionName }.Concat(parts[1..]));
        //Debug.Log($"拼接最終指令：{resultCmd}");
        return resultCmd;
    }

    private bool IsExpression(string input)
    {
        // 簡單判斷是否包含運算符
        return input.Contains("+") || input.Contains("-") || input.Contains("*") || input.Contains("/");
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
                    Debug.LogError($"[EvaluateExpression]{token}無值");
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
