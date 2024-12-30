using System.Collections.Generic;
using UnityEngine;

public class TxR : MonoBehaviour
{
    public static TxR Inst { get; private set; }

    private void Awake()
    {
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// �s�W�Χ�s�ܼơ]�ϥ� PPM�^
    /// </summary>
    /// <param name="key">�e��ŦW��</param>
    /// <param name="value">������</param>
    public void SetVariable(string key, string value)
    {
        PPM.Inst.Set(key, value); // �����ϥ� PPM �x�s�ܼ�
    }

    /// <summary>
    /// ��q�s�W�Χ�s�ܼơ]�ϥ� PPM�^
    /// </summary>
    /// <param name="newVariables">�]�t��ȹ諸�r��</param>
    public void SetVariables(Dictionary<string, string> newVariables)
    {
        foreach (var kvp in newVariables)
        {
            SetVariable(kvp.Key, kvp.Value); // �ϥ� SetVariable �ӷs�W�Χ�s�ܼ�
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
        List<string> keys = PPM.Inst.GetAllKeys(); // ��� PPM �����

        foreach (string key in keys)
        {
            string placeholder = $"{{{{{key}}}}}"; // �榡�ƥe��šA�Ҧp {{name}}
            string value = PPM.Inst.Get(key); // �q PPM ���������
            result = result.Replace(placeholder, value ?? string.Empty);
        }

        return result;
    }

    /// <summary>
    /// �M���Ҧ��ܼơ]�ϥ� PPM�^
    /// </summary>
    public void ClearVariables()
    {
        PPM.Inst.Clear(); // �M�ũҦ��ܼ�
    }
}
