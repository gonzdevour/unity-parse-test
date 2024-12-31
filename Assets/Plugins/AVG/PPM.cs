using System.Collections.Generic;
using UnityEngine;

public class PPM : MonoBehaviour
{
    public static PPM Inst { get; private set; }
    private void Awake()
    {
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    private const string KeyListKey = "PPM_KeyList"; // 鍵表的專用鍵

    /// <summary>
    /// 儲存值並更新鍵表
    /// </summary>
    /// <typeparam name="T">數據類型</typeparam>
    /// <param name="key">鍵</param>
    /// <param name="value">值</param>
    public void Set<T>(string key, T value)
    {
        // 儲存值
        PlayerPrefs.SetString(key, value.ToString());
        Debug.Log($"[PPM]已設定{key}為{value.ToString()}");

        // 更新鍵表
        var keys = GetKeyList();
        if (!keys.Contains(key))
        {
            keys.Add(key);
            UpdateKeyList(keys);
        }

        PlayerPrefs.Save(); // 確保數據立即保存
    }

    /// <summary>
    /// 獲取值，若鍵不存在則返回指定的預設值
    /// </summary>
    /// <param name="key">鍵</param>
    /// <param name="defaultValue">當鍵不存在時返回的預設值</param>
    /// <returns>值（字串）</returns>
    public string Get(string key, string defaultValue = "")
    {
        if (PlayerPrefs.HasKey(key))
        {
            return PlayerPrefs.GetString(key);
        }

        //Debug.LogWarning($"PPM: Key '{key}' does not exist. Returning default value: {defaultValue}");
        return defaultValue;
    }

    /// <summary>
    /// 刪除指定鍵並更新鍵表
    /// </summary>
    /// <param name="key">鍵</param>
    public void Remove(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);

            // 更新鍵表
            var keys = GetKeyList();
            if (keys.Contains(key))
            {
                keys.Remove(key);
                UpdateKeyList(keys);
            }

            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning($"PPM: Key '{key}' does not exist.");
        }
    }

    /// <summary>
    /// 清除所有鍵值
    /// </summary>
    public void Clear()
    {
        var keys = GetKeyList();

        // 刪除所有鍵
        foreach (var key in keys)
        {
            PlayerPrefs.DeleteKey(key);
        }

        // 清空鍵表
        PlayerPrefs.DeleteKey(KeyListKey);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 獲取所有鍵
    /// </summary>
    /// <returns>鍵的列表</returns>
    public List<string> GetAllKeys()
    {
        return GetKeyList();
    }

    /// <summary>
    /// 獲取鍵表
    /// </summary>
    /// <returns>鍵列表</returns>
    private List<string> GetKeyList()
    {
        if (PlayerPrefs.HasKey(KeyListKey))
        {
            string keyListString = PlayerPrefs.GetString(KeyListKey);
            return new List<string>(keyListString.Split(','));
        }
        return new List<string>();
    }

    /// <summary>
    /// 更新鍵表
    /// </summary>
    /// <param name="keys">新的鍵列表</param>
    private void UpdateKeyList(List<string> keys)
    {
        string keyListString = string.Join(",", keys);
        PlayerPrefs.SetString(KeyListKey, keyListString);
    }
}
