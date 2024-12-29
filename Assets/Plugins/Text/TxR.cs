using System.Collections.Generic;
using UnityEngine;

public class TxR : MonoBehaviour
{
    private Dictionary<string, string> variables = new(); // �x�s�ҪO�ܼ�

    public static TxR Inst { get; private set; }
    private void Awake()
    {
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// �s�W�Χ�s�ܼ�
    /// </summary>
    /// <param name="key">�e��ŦW��</param>
    /// <param name="value">������</param>
    public void SetVariable(string key, string value)
    {
        if (variables.ContainsKey(key))
        {
            variables[key] = value; // ��s�w����
        }
        else
        {
            variables.Add(key, value); // �K�[�s��
        }
    }

    /// <summary>
    /// ��q�s�W�Χ�s�ܼ�
    /// </summary>
    /// <param name="newVariables">�]�t��ȹ諸�r��</param>
    public void SetVariables(Dictionary<string, string> newVariables)
    {
        foreach (var kvp in newVariables)
        {
            SetVariable(kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// ��V�ҪO
    /// </summary>
    /// <param name="template">�]�t�e��Ū��ҪO�r�Ŧ�</param>
    /// <returns>��V�᪺���G�r�Ŧ�</returns>
    public string Render(string template)
    {
        string result = template;

        foreach (var kvp in variables)
        {
            string placeholder = $"{{{{{kvp.Key}}}}}"; // �榡�ƥe��šA�Ҧp {{name}}
            result = result.Replace(placeholder, kvp.Value ?? string.Empty);
        }

        return result;
    }

    /// <summary>
    /// �M���Ҧ��ܼ�
    /// </summary>
    public void ClearVariables()
    {
        variables.Clear();
    }
}
