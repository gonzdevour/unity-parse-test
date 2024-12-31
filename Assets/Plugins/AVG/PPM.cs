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

    private const string KeyListKey = "PPM_KeyList"; // ����M����

    /// <summary>
    /// �x�s�Ȩç�s���
    /// </summary>
    /// <typeparam name="T">�ƾ�����</typeparam>
    /// <param name="key">��</param>
    /// <param name="value">��</param>
    public void Set<T>(string key, T value)
    {
        // �x�s��
        PlayerPrefs.SetString(key, value.ToString());
        Debug.Log($"[PPM]�w�]�w{key}��{value.ToString()}");

        // ��s���
        var keys = GetKeyList();
        if (!keys.Contains(key))
        {
            keys.Add(key);
            UpdateKeyList(keys);
        }

        PlayerPrefs.Save(); // �T�O�ƾڥߧY�O�s
    }

    /// <summary>
    /// ����ȡA�Y�䤣�s�b�h��^���w���w�]��
    /// </summary>
    /// <param name="key">��</param>
    /// <param name="defaultValue">���䤣�s�b�ɪ�^���w�]��</param>
    /// <returns>�ȡ]�r��^</returns>
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
    /// �R�����w��ç�s���
    /// </summary>
    /// <param name="key">��</param>
    public void Remove(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);

            // ��s���
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
    /// �M���Ҧ����
    /// </summary>
    public void Clear()
    {
        var keys = GetKeyList();

        // �R���Ҧ���
        foreach (var key in keys)
        {
            PlayerPrefs.DeleteKey(key);
        }

        // �M�����
        PlayerPrefs.DeleteKey(KeyListKey);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// ����Ҧ���
    /// </summary>
    /// <returns>�䪺�C��</returns>
    public List<string> GetAllKeys()
    {
        return GetKeyList();
    }

    /// <summary>
    /// ������
    /// </summary>
    /// <returns>��C��</returns>
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
    /// ��s���
    /// </summary>
    /// <param name="keys">�s����C��</param>
    private void UpdateKeyList(List<string> keys)
    {
        string keyListString = string.Join(",", keys);
        PlayerPrefs.SetString(KeyListKey, keyListString);
    }
}
