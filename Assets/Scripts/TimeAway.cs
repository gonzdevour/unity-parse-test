using UnityEngine;
using System;

public class TimeAway : MonoBehaviour
{
    private const string LastUnfocusTimeKey = "LastUnfocusTime"; // 用于存储时间的键
    private float elapsedTime; // 记录经过时间

    private void Start()
    {
        // 每秒調用一次 RecordCurrentTime，立即開始
        InvokeRepeating(nameof(RecordCurrentTime), 0f, 1f);
    }

    private void RecordCurrentTime()
    {
        PlayerPrefs.SetString(LastUnfocusTimeKey, DateTime.Now.ToString());
        PlayerPrefs.Save();
        //Debug.Log($"記錄時間: {DateTime.Now}");
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            // 应用程序重新获得焦点时计算时间间隔
            string lastTimeStr = PlayerPrefs.GetString(LastUnfocusTimeKey, "");
            if (!string.IsNullOrEmpty(lastTimeStr))
            {
                DateTime lastUnfocusTime = DateTime.Parse(lastTimeStr);
                elapsedTime = (float)(DateTime.Now - lastUnfocusTime).TotalSeconds;
                Debug.Log($"從上次關閉或失去焦點的時間間隔: {elapsedTime} 秒");
            }
        }
        else
        {
            // 应用程序失去焦点时记录时间
            RecordCurrentTime();
        }
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            // 应用程序进入后台时记录时间
            RecordCurrentTime();
        }
    }

    private void OnApplicationQuit()
    {
        // 应用程序退出时记录时间
        RecordCurrentTime();
    }

    /// <summary>
    /// 檢查指定事件的冷卻時間是否已完成。
    /// </summary>
    /// <param name="eventID">事件的唯一標識符。</param>
    /// <param name="cooldownSeconds">冷卻時間（秒）。</param>
    /// <param name="remainingTime">輸出剩餘冷卻時間（秒）。</param>
    /// <returns>如果冷卻時間已過，返回 true；否則返回 false。</returns>
    public bool CheckCooldownRealtime(string eventID, float cooldownSeconds, out float remainingTime)
    {
        string key = $"RealtimeCDStart_{eventID}";
        string lastTimeStr = PlayerPrefs.GetString(key, "");
        if (string.IsNullOrEmpty(lastTimeStr))
        {
            // 如果沒有記錄時間，冷卻完成
            remainingTime = 0f;
            return true;
        }

        DateTime lastInvokeTime = DateTime.Parse(lastTimeStr);
        TimeSpan timeSinceLastInvoke = DateTime.Now - lastInvokeTime;

        remainingTime = cooldownSeconds - (float)timeSinceLastInvoke.TotalSeconds;
        remainingTime = Mathf.Max(0f, remainingTime); // 確保不會返回負值

        // 冷卻完成後，刪除 key
        if (remainingTime == 0f)
        {
            PlayerPrefs.DeleteKey(key);
        }

        return remainingTime == 0f;
    }

    /// <summary>
    /// 記錄指定事件的冷卻開始時間。
    /// </summary>
    /// <param name="eventID">按鈕的唯一標識符。</param>
    public void StartCooldownRealtime(string eventID)
    {
        string key = $"RealtimeCDStart_{eventID}";
        PlayerPrefs.SetString(key, DateTime.Now.ToString());
        PlayerPrefs.Save(); // 確保資料被保存
    }
}
