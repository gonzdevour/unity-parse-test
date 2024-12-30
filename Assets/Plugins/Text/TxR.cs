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
    /// 新增或更新變數（使用 PPM）
    /// </summary>
    /// <param name="key">占位符名稱</param>
    /// <param name="value">替換值</param>
    public void SetVariable(string key, string value)
    {
        PPM.Inst.Set(key, value); // 直接使用 PPM 儲存變數
    }

    /// <summary>
    /// 批量新增或更新變數（使用 PPM）
    /// </summary>
    /// <param name="newVariables">包含鍵值對的字典</param>
    public void SetVariables(Dictionary<string, string> newVariables)
    {
        foreach (var kvp in newVariables)
        {
            SetVariable(kvp.Key, kvp.Value); // 使用 SetVariable 來新增或更新變數
        }
    }

    /// <summary>
    /// 渲染模板
    /// </summary>
    /// <param name="template">包含占位符的模板字符串</param>
    /// <returns>渲染後的結果字符串</returns>
    public string Render(string template)
    {
        string result = template;
        List<string> keys = PPM.Inst.GetAllKeys(); // 獲取 PPM 的鍵表

        foreach (string key in keys)
        {
            string placeholder = $"{{{{{key}}}}}"; // 格式化占位符，例如 {{name}}
            string value = PPM.Inst.Get(key); // 從 PPM 獲取對應值
            result = result.Replace(placeholder, value ?? string.Empty);
        }

        return result;
    }

    /// <summary>
    /// 清除所有變數（使用 PPM）
    /// </summary>
    public void ClearVariables()
    {
        PPM.Inst.Clear(); // 清空所有變數
    }
}
