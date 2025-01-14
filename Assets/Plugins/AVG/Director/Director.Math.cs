using System;
using UnityEngine;

public partial class Director
{
    /// <summary>
    /// 設定隨機值。
    /// </summary>
    /// <param name="args">參數陣列，args[0] 為隨機設定字串</param>
    private void SetRandomValue(object[] args)
    {
        if (args == null || args.Length == 0 || args[0] == null)
        {
            Debug.LogError("參數錯誤，args[0] 為空");
            return;
        }

        // 將 args[0] 轉為字串
        string input = args[0].ToString();

        if (input.Contains("~"))
        {
            // 處理 "~" 隨機範圍
            string[] parts = input.Split('~');

            if (parts.Length == 2)
            {
                string startStr = parts[0].Trim();
                string endStr = parts[1].Trim();

                if (int.TryParse(startStr, out int startInt) && int.TryParse(endStr, out int endInt))
                {
                    // 整數範圍
                    int randomInt = UnityEngine.Random.Range(startInt, endInt + 1);
                    PPM.Inst.Set("隨機值", randomInt.ToString());
                }
                else if (float.TryParse(startStr, out float startFloat) && float.TryParse(endStr, out float endFloat))
                {
                    // 浮點數範圍
                    float randomFloat = UnityEngine.Random.Range(startFloat, endFloat);

                    // 取得小數位數
                    int decimalPlaces = Math.Max(startStr.Length - startStr.IndexOf('.') - 1, endStr.Length - endStr.IndexOf('.') - 1);

                    // 格式化為相同小數位數
                    string formattedFloat = randomFloat.ToString("F" + decimalPlaces);
                    PPM.Inst.Set("隨機值", formattedFloat);
                }
                else
                {
                    Debug.LogError("隨機值請求的前後值不是合法數值" + input);
                }
            }
            else
            {
                Debug.LogError("隨機值請求格式錯誤: " + input);
            }
        }
        else if (input.Contains("&"))
        {
            // 處理 "&" 隨機選擇
            string[] options = input.Split('&');

            if (options.Length > 0)
            {
                string randomOption = options[UnityEngine.Random.Range(0, options.Length)].Trim();
                PPM.Inst.Set("隨機值", randomOption);
            }
            else
            {
                Debug.LogError("隨機值請求缺少選項: " + input);
            }
        }
        else
        {
            Debug.LogError("隨機值請求參數錯誤: " + input);
        }
    }
}
