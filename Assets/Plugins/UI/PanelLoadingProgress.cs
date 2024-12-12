using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // 引入 DOTween
using System.Collections; // 引入協程

public class PanelLoadingProgress : MonoBehaviour
{
    public Image ImgLoadingKnob; // 進度條（圖片，filled 類型）
    public Text TxLPPercent;    // 顯示進度百分比的文字
    public Text TxLPTitle;      // 顯示標題的文字
    public Text TxLPMsg;        // 顯示訊息的文字

    private int totalTaskCount; // 總任務數量
    private int currentProgress; // 當前進度

    /// <summary>
    /// 初始化進度條。
    /// </summary>
    /// <param name="totalTaskCount">總任務數量</param>
    /// <param name="title">標題</param>
    /// <param name="msg">訊息</param>
    public void StartProgress(int totalTaskCount, string title = "", string msg = "")
    {
        gameObject.SetActive(true); // 啟用 PanelLoadingProgress
        this.totalTaskCount = totalTaskCount;
        currentProgress = 0;
        UpdateUI(title, msg, true);
    }

    /// <summary>
    /// 設定進度至指定數值。
    /// </summary>
    /// <param name="targetProgress">目標進度</param>
    /// <param name="title">標題</param>
    /// <param name="msg">訊息</param>
    public void GoTo(int targetProgress, string title = "", string msg = "")
    {
        currentProgress = Mathf.Clamp(targetProgress, 0, totalTaskCount);
        UpdateUI(title, msg);
        CheckCompletion();
    }

    /// <summary>
    /// 增加指定進度。
    /// </summary>
    /// <param name="progressToAdd">增加的進度值</param>
    /// <param name="title">標題</param>
    /// <param name="msg">訊息</param>
    public void Add(int progressToAdd, string title = "", string msg = "")
    {
        currentProgress = Mathf.Clamp(currentProgress + progressToAdd, 0, totalTaskCount);
        UpdateUI(title, msg);
        CheckCompletion();
    }

    /// <summary>
    /// 更新 UI。
    /// </summary>
    /// <param name="title">標題</param>
    /// <param name="msg">訊息</param>
    /// <param name="ifInstant">是否立即更新</param>
    private void UpdateUI(string title, string msg, bool ifInstant = false)
    {
        // 更新進度百分比
        float fillAmount = totalTaskCount > 0 ? (float)currentProgress / totalTaskCount : 0;
        if (ImgLoadingKnob != null)
        {
            if (ifInstant)
            {
                ImgLoadingKnob.fillAmount = fillAmount;
            }
            else
            {
                ImgLoadingKnob.DOFillAmount(fillAmount, 0.5f).SetEase(Ease.Linear); // 使用 DOTween 進行平滑動畫
            }
        }

        // 更新百分比文字
        if (TxLPPercent != null)
        {
            int endValue = Mathf.RoundToInt(fillAmount * 100);
            if (ifInstant)
            {
                TxLPPercent.text = endValue + "%";
            }
            else
            {
                int startValue = Mathf.RoundToInt((ImgLoadingKnob.fillAmount) * 100);
                DOTween.To(() => startValue, x => {
                    startValue = x;
                    TxLPPercent.text = startValue + "%";
                }, endValue, 0.5f).SetEase(Ease.Linear);
            }
        }

        // 更新標題文字
        if (!string.IsNullOrEmpty(title) && TxLPTitle != null)
        {
            TxLPTitle.text = title;
        }

        // 更新訊息文字
        if (!string.IsNullOrEmpty(msg) && TxLPMsg != null)
        {
            TxLPMsg.text = msg;
        }
    }

    /// <summary>
    /// 檢查進度是否完成。
    /// </summary>
    private void CheckCompletion()
    {
        if (currentProgress >= totalTaskCount)
        {
            StartCoroutine(DeactivateAfterDelay(0.5f));
        }
    }

    /// <summary>
    /// 延遲停用物件。
    /// </summary>
    /// <param name="delay">延遲時間（秒）</param>
    /// <returns></returns>
    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
