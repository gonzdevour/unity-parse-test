using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public partial class Typing : MonoBehaviour
{
    public static List<string> ParseRichTextCharacters(string input)
    {
        List<string> result = new List<string>(); // 存放解析後的每個字
        Stack<string> openTags = new Stack<string>(); // 儲存當前的標籤堆疊
        string buffer = ""; // 暫存標籤內容
        bool insideTag = false;

        foreach (char c in input)
        {
            if (c == '<')
            {
                insideTag = true;
                buffer = "<"; // 開始記錄標籤
            }
            else if (c == '>')
            {
                insideTag = false;
                buffer += ">";
                ProcessTag(buffer, openTags);
            }
            else if (insideTag)
            {
                buffer += c;
            }
            else
            {
                result.Add(ApplyTags(c.ToString(), openTags));
            }
        }
        return result;
    }

    public static string GetClosingTags(string openTag)
    {
        Stack<string> tagStack = new Stack<string>(); // 使用堆疊來確保標籤順序
        string pattern = @"<([^<>]+)>"; // 正則表達式匹配 <tag> 或 <tag=value>
        MatchCollection matches = Regex.Matches(openTag, pattern);

        // 解析所有開啟標籤
        foreach (Match match in matches)
        {
            string tag = match.Groups[1].Value; // 取得標籤內容
            int equalSignIndex = tag.IndexOf('=');

            if (equalSignIndex != -1)
            {
                tag = tag.Substring(0, equalSignIndex); // 移除屬性部分 color=red -> color
            }

            tagStack.Push(tag); // 堆疊儲存標籤，確保後開的先關閉
            //Debug.Log($"tagPushed:{tag}");
        }

        // 生成正確的關閉標籤（後開的標籤先關閉）
        string closingTags = "";
        while (tagStack.Count > 0)
        {
            string poppedTag = tagStack.Pop();
            closingTags += $"</{poppedTag}>";
            //Debug.Log($"tagPopped:{poppedTag}");
        }

        return closingTags;
    }

    private static void ProcessTag(string tag, Stack<string> openTags)
    {
        if (tag.StartsWith("</")) // 關閉標籤
        {
            if (openTags.Count > 0) openTags.Pop();
        }
        else
        {
            openTags.Push(tag); // 開啟標籤
        }
    }

    private static string ApplyTags(string character, Stack<string> openTags)
    {
        string openTag = string.Concat(openTags.Reverse());
        string closeTag = GetClosingTags(string.Concat(openTags.Reverse()));
        //Debug.Log($"open:{openTag} content:{character} close:{closeTag}");
        return openTag + character + closeTag;
    }
}
