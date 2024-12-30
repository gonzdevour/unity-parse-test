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

    // �w�q��Ʀr��
    private Dictionary<string, Action<object[]>> actions;

    private void Start()
    {
        // ��l�Ʀr��øj�w���
        actions = new Dictionary<string, Action<object[]>>
        {
            { "��", args => ChangeExpression(args) },
            { "����", args => MoveCharacter(args) }
        };

        // ����
        PPM.Inst.Set("����", "100");
        PPM.Inst.Set("�J�|����", "100");
        PPM.Inst.Set("�ѥ]����", "60");
        PPM.Inst.Set("�d���_�ջ���", "120");
        PPM.Inst.Set("�ߤl�X���Ի���", "150");
        PPM.Inst.Set("MaxHP", "199");
        PPM.Inst.Set("��Ӱ}��", "������R�x");
        //ExecuteAction("����,����1,10,20");
        //ExecuteAction("����,100");
        //ExecuteAction("����+100-20");
        //ExecuteAction("���� = 10");
        //ExecuteAction("�m�W = \"\"��i�Ҵ���\"\"");
        //ExecuteAction("�}�� = ��Ӱ}��");
        //ExecuteAction("HP=MaxHP*50");
        //ExecuteAction("��,�i�N�M,��");
    }

    // �r�夤�����
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

    // ������

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
                //Debug.Log($"[Pkg����]: {command}");
                ExecuteAction(command); // �եγ�ӫ��O��������
            }
            else
            {
                Debug.LogWarning("Skipped empty or null command.");
            }
        }
    }

    public void ExecuteAction(string command)
    {
        command = PreprocessCommand(command);// �ѪRcommand�í���
        Debug.Log($"����ѪR��cmd:{command}");

        var parts = command.Split(',').Select(part => part.Trim()).ToArray(); // Trim�B�z

        if (parts.Length >= 1 && actions.ContainsKey(parts[0]))
        {
            // �����ʧ@�W�٩M�Ѽ�
            string actionName = parts[0];
            object[] parameters = parts[1..]; // �����ѤU���Ѽ�

            // ������������
            actions[actionName](parameters);
        }
        else
        {
            Debug.LogWarning("Invalid command or action not found.");
        }
    }

    public string PreprocessCommand(string command)
    {
        //Debug.Log($"�}�l�B�z���O�G{command}");

        // 1. �P�_�O�_���B�⦡�B���]�t "," �P "="
        if (!command.Contains(",") && !command.Contains("=") && IsExpression(command))
        {
            // ���Ĥ@�ӹB��Ū���m
            int operatorIndex = command.IndexOfAny(new[] { '+', '-', '*', '/' });
            if (operatorIndex > 0)
            {
                // �H�B��Ť��� command
                string actionNamePre = command.Substring(0, operatorIndex).Trim();
                string expressionPre = command.Substring(operatorIndex).Trim();

                // ���լ� "actionName,actionName+expression"
                command = $"{actionNamePre},{actionNamePre}{expressionPre}";
                //Debug.Log($"���չB�⦡���O: {command}");
            }
        }
        // 2. ���Ϋ��O
        var parts = command.Split(',').Select(part => part.Trim()).ToArray(); // Trim�B�z

        if (parts.Length == 0)
        {
            Debug.LogWarning("Command is empty.");
            return command; // ��^��l���O
        }

        string actionName;
        // 3. �B�z�Ĥ@�� part�]��ƦW�١^
        var firstPart = parts[0].Split('=').Select(part => part.Trim()).ToArray(); // Trim�B�z
        if (firstPart.Length > 1)
        {
            // �p�G�� "="�A�������e�������@����ƦW��
            actionName = firstPart[0];

            // �N�����᪺��������ô��J�� parts �}�C��
            List<string> newParts = new List<string> { actionName }; // �s�}�C���Ĥ@�����O��ƦW��
            newParts.AddRange(firstPart[1..]); // ���J�����᪺����
            newParts.AddRange(parts[1..]); // ���J�쥻 parts ���Ѿl����

            // �N newParts ��s�^ parts
            parts = newParts.ToArray();
        }
        else
        {
            // �p�G�S�� "="�A�Ĥ@�� part �����N�O��ƦW��
            actionName = parts[0];
        }
        //Debug.Log($"��ƦW�١G{actionName}");

        // 4. �B�z�Ѿl�� parts
        for (int i = 1; i < parts.Length; i++)
        {
            string part = parts[i];

            // �p�G�O�Q���޸��]�q�A�������Ȭ��¦r��
            if (part.StartsWith("\"") && part.EndsWith("\""))
            {
                parts[i] = part.Trim('\"');
                //Debug.Log($"{part}�Q���޸��]�q");
            }
            else
            {
                // �_�h���ձq PlayerPrefs ������
                string prefValue = PPM.Inst.Get(part, part); // �q�{��^ part ����
                //Debug.Log($"{part}�S���Q���޸��]�q�A���ձqPP���ȡG{prefValue}");
                parts[i] = prefValue;

                // �p�G�O�⦡�A���ոѪR�íp��
                if (IsExpression(prefValue))
                {
                    //Debug.Log($"{prefValue}�O�⦡");
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

        // 5. �����̲׫��O
        string resultCmd = string.Join(",", new[] { actionName }.Concat(parts[1..]));
        //Debug.Log($"�����̲׫��O�G{resultCmd}");
        return resultCmd;
    }

    private bool IsExpression(string input)
    {
        // ²��P�_�O�_�]�t�B���
        return input.Contains("+") || input.Contains("-") || input.Contains("*") || input.Contains("/");
    }

    private float EvaluateExpression(string expression)
    {
        // �䴩���B���
        char[] operators = { '+', '-', '*', '/' };

        // ���Ϊ�F���ëO�d�B���
        List<string> tokens = new List<string>();
        int lastIndex = 0;

        for (int i = 0; i < expression.Length; i++)
        {
            if (operators.Contains(expression[i]))
            {
                // �N�B��ƩM�B��Ť��O�[�J
                tokens.Add(expression.Substring(lastIndex, i - lastIndex).Trim());
                tokens.Add(expression[i].ToString());
                lastIndex = i + 1;
            }
        }

        // �̫�@�ӹB���
        if (lastIndex < expression.Length)
        {
            tokens.Add(expression.Substring(lastIndex).Trim());
        }

        // �B�z�C�� token
        for (int i = 0; i < tokens.Count; i++)
        {
            if (!operators.Contains(tokens[i][0]))
            {
                // �D�B��šA�����ܼƦW�٩μƭ�
                string token = tokens[i].Trim();

                // �q PlayerPrefs ���ը��ȡA�q�{�� token ����
                if (!PlayerPrefs.HasKey(token))
                {
                    Debug.LogError($"[EvaluateExpression]{token}�L��");
                }
                string value = PPM.Inst.Get(token, "0");

                // ���ձN�ȸѪR���B�I��
                if (float.TryParse(value, out float number))
                {
                    tokens[i] = number.ToString(); // �N�ѪR���G��s�^ token
                }
                else
                {
                    throw new FormatException($"Invalid value for token: {token}");
                }
            }
        }

        // ���ժ�F��
        string processedExpression = string.Join("", tokens);

        // �p���F�����G
        DataTable table = new DataTable();
        object result = table.Compute(processedExpression, string.Empty);
        return Convert.ToSingle(result);
    }
}
