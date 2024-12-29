using System.Collections.Generic;
using UnityEngine;

public class TxR : MonoBehaviour
{
    private Dictionary<string, string> variables = new(); // 纗家狾跑计

    public static TxR Inst { get; private set; }
    private void Awake()
    {
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 穝糤┪穝跑计
    /// </summary>
    /// <param name="key">才嘿</param>
    /// <param name="value">蠢传</param>
    public void SetVariable(string key, string value)
    {
        if (variables.ContainsKey(key))
        {
            variables[key] = value; // 穝Τ龄
        }
        else
        {
            variables.Add(key, value); // 睰穝龄
        }
    }

    /// <summary>
    /// у秖穝糤┪穝跑计
    /// </summary>
    /// <param name="newVariables">龄癸ㄥ</param>
    public void SetVariables(Dictionary<string, string> newVariables)
    {
        foreach (var kvp in newVariables)
        {
            SetVariable(kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// 磋琕家狾
    /// </summary>
    /// <param name="template">才家狾才﹃</param>
    /// <returns>磋琕挡狦才﹃</returns>
    public string Render(string template)
    {
        string result = template;

        foreach (var kvp in variables)
        {
            string placeholder = $"{{{{{kvp.Key}}}}}"; // Αて才ㄒ {{name}}
            result = result.Replace(placeholder, kvp.Value ?? string.Empty);
        }

        return result;
    }

    /// <summary>
    /// 睲埃┮Τ跑计
    /// </summary>
    public void ClearVariables()
    {
        variables.Clear();
    }
}
