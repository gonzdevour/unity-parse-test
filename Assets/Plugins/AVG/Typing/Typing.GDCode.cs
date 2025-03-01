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
        string pattern = @"\[(\w+)=([^]]+)\]"; // �ǰt [key=param0,param1,param2]

        Match match = Regex.Match(input, pattern);
        if (!match.Success) return Array.Empty<string>(); // �Y�榡���~�A�^�ǪŰ}�C

        string key = match.Groups[1].Value; // ���o key
        string[] values = match.Groups[2].Value.Split(','); // �ѪR�Ѽ�
        string[] result = new string[values.Length + 1]; // �Ыطs�}�C�]key + params�^

        result[0] = key;
        Array.Copy(values, 0, result, 1, values.Length); // �N values �ƻs�� result

        return result;
    }

    public static string[] GDCodeReindex(string input)
    {
        input = Regex.Replace(input, "<.*?>", ""); // �����Ҧ�RichText�� <tag>
        List<string> result = new List<string>();
        string pattern = @"\[[^\]]+\]"; // ���h��F���ǰt [key=value] ����
        MatchCollection matches = Regex.Matches(input, pattern);

        int lastIndex = 0;
        int index = 0;

        foreach (Match match in matches)
        {
            // ���o���Ҫ��}�l����
            int tagIndex = match.Index;

            // ���K�[���ҫe�����q�r���]��r�s�J�å[���ޡ^
            for (int i = lastIndex; i < tagIndex; i++)
            {
                result.Add($"{index}:{input[i]}");
                index++;
            }

            // �K�[�������
            result.Add($"{index}:{match.Value}");
            index++;

            // ��s lastIndex ����Ҫ�������m
            lastIndex = tagIndex + match.Length;
        }

        // �B�z�̫᪺�Ѿl�r��
        for (int i = lastIndex; i < input.Length; i++)
        {
            result.Add($"{index}:{input[i]}");
            index++;
        }

        return result.ToArray();
    }
}
