using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public partial class Typing : MonoBehaviour
{
    public static string[] GDCodeParse(string input)
    {
        string pattern = @"\[(\w+)=([^]]+)\]"; // 匹配 [key=param0,param1,param2]

        Match match = Regex.Match(input, pattern);
        if (!match.Success) return Array.Empty<string>(); // 若格式錯誤，回傳空陣列

        string key = match.Groups[1].Value; // 取得 key
        string[] values = match.Groups[2].Value.Split(','); // 解析參數
        string[] result = new string[values.Length + 1]; // 創建新陣列（key + params）

        result[0] = key;
        Array.Copy(values, 0, result, 1, values.Length); // 將 values 複製到 result

        return result;
    }

    public static string[] GDCodeReindex(string input)
    {
        input = Regex.Replace(input, "<.*?>", ""); // 移除所有RichText的 <tag>
        List<string> result = new List<string>();
        string pattern = @"\[[^\]]+\]"; // 正則表達式匹配 [key=value] 標籤
        MatchCollection matches = Regex.Matches(input, pattern);

        int lastIndex = 0;
        int index = 0;

        foreach (Match match in matches)
        {
            // 取得標籤的開始索引
            int tagIndex = match.Index;

            // 先添加標籤前的普通字元（單字存入並加索引）
            for (int i = lastIndex; i < tagIndex; i++)
            {
                result.Add($"{index}:{input[i]}");
                index++;
            }

            // 添加完整標籤
            result.Add($"{index}:{match.Value}");
            index++;

            // 更新 lastIndex 到標籤的結束位置
            lastIndex = tagIndex + match.Length;
        }

        // 處理最後的剩餘字元
        for (int i = lastIndex; i < input.Length; i++)
        {
            result.Add($"{index}:{input[i]}");
            index++;
        }

        return result.ToArray();
    }
}
