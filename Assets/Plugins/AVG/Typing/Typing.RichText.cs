using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public partial class Typing : MonoBehaviour
{
    public static List<string> ParseRichTextCharacters(string input)
    {
        List<string> result = new List<string>(); // �s��ѪR�᪺�C�Ӧr
        Stack<string> openTags = new Stack<string>(); // �x�s��e�����Ұ��|
        string buffer = ""; // �Ȧs���Ҥ��e
        bool insideTag = false;

        foreach (char c in input)
        {
            if (c == '<')
            {
                insideTag = true;
                buffer = "<"; // �}�l�O������
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
        Stack<string> tagStack = new Stack<string>(); // �ϥΰ��|�ӽT�O���Ҷ���
        string pattern = @"<([^<>]+)>"; // ���h��F���ǰt <tag> �� <tag=value>
        MatchCollection matches = Regex.Matches(openTag, pattern);

        // �ѪR�Ҧ��}�Ҽ���
        foreach (Match match in matches)
        {
            string tag = match.Groups[1].Value; // ���o���Ҥ��e
            int equalSignIndex = tag.IndexOf('=');

            if (equalSignIndex != -1)
            {
                tag = tag.Substring(0, equalSignIndex); // �����ݩʳ��� color=red -> color
            }

            tagStack.Push(tag); // ���|�x�s���ҡA�T�O��}��������
            //Debug.Log($"tagPushed:{tag}");
        }

        // �ͦ����T���������ҡ]��}�����ҥ������^
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
        if (tag.StartsWith("</")) // ��������
        {
            if (openTags.Count > 0) openTags.Pop();
        }
        else
        {
            openTags.Push(tag); // �}�Ҽ���
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
