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
        ExecuteAction("��,�i�N�M,��");
        ExecuteAction("����,����1,10,20");
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
    public void ExecuteAction(string command)
    {
        var parts = command.Split(',');

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
}
