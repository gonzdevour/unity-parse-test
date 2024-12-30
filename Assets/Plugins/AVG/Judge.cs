using System;
using UnityEngine;

public class Judge
{
    // �D��ơG�ѪR�çP�_����
    public static bool EvaluateCondition(string condition)
    {
        Debug.Log($"�}�l�ѪR����: \n{condition}");

        // �N����H�����A�C��N��@��&&����
        string[] lines = condition.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            Debug.Log($"�ѪR�����: {line}");

            // �N�C��������� && �����
            string[] andConditions = line.Split(new[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string andCondition in andConditions)
            {
                Debug.Log($"�ѪR AND ����: {andCondition}");

                // �N && ���󤤪��C�Ӥl����i��P�_
                if (!EvaluateOrConditions(andCondition))
                {
                    // �p�G���@���󤣦��ߡA��^ false
                    Debug.Log($"���󤣦���: {andCondition}");
                    return false;
                }
            }
        }

        // �p�G�Ҧ����󳣦��ߡA��^ true
        Debug.Log("�Ҧ����󳣦��ߡC");
        return true;
    }

    // �P�_�@�椤�� OR ����
    private static bool EvaluateOrConditions(string andCondition)
    {
        Debug.Log($"�ѪR OR ����: {andCondition}");

        // �N����� || ����
        string[] orConditions = andCondition.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string orCondition in orConditions)
        {
            Debug.Log($"�ѪR��@ OR �l����: {orCondition.Trim()}");

            // �Y���@ OR �l���󦨥ߡA�h��� OR ���󬰯u
            if (EvaluateSingleCondition(orCondition.Trim()))
            {
                Debug.Log($"OR �l���󦨥�: {orCondition.Trim()}");
                return true;
            }
        }

        // �Ҧ� OR �l���󳣤����ߡA��^ false
        Debug.Log($"�Ҧ� OR �l���󳣤�����: {andCondition}");
        return false;
    }

    // �P�_��@����
    public static bool EvaluateSingleCondition(string condition)
    {
        Debug.Log($"�ѪR��@����: {condition}");

        // �N��@ "=" ������ "=="�A�T�O���v�T��L�B���
        if (condition.Contains("=") && !condition.Contains("==") && !condition.Contains(">=") && !condition.Contains("<="))
        {
            Debug.Log($"�N��@ '=' ������ '==': {condition}");
            condition = condition.Replace("=", "==");
        }

        // �䴩���B��ŦC��
        string[] operators = { "==", ">=", "<=", ">", "<", "!=" };

        foreach (string op in operators)
        {
            if (condition.Contains(op))
            {
                // �������Gkey �M �����
                string[] parts = condition.Split(new[] { op }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    // �p�G�ȥ��Q���޸��]�q�B�L�k�ѪR���ƭȡA�h���ձq PlayerPrefs �����
                    if ((!value.StartsWith("\"") || !value.EndsWith("\"")) && !int.TryParse(value, out _) && !float.TryParse(value, out _))
                    {
                        value = PlayerPrefs.GetString(value, value);
                    }

                    Debug.Log($"������: key={key}, value={value}, op={op}");

                    // �q PlayerPrefs Ū�� key ����
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
                    Debug.LogError($"����榡���~�A�L�k����� key �M value: {condition}");
                    return false;
                }
            }
        }

        Debug.LogError($"���󤤤��]�t���Ī��B���: {condition}");
        return false;
    }

    // ����ƭȫ�����
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
            default: throw new ArgumentException($"�������B���: {op}");
        }
    }

    // ���L Debug �T��
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
