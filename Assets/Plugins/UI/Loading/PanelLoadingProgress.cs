using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // �ޤJ DOTween
using System.Collections; // �ޤJ��{

public class PanelLoadingProgress : MonoBehaviour
{
    public Image ImgLoadingKnob; // �i�ױ��]�Ϥ��Afilled �����^
    public Text TxLPPercent;    // ��ܶi�צʤ��񪺤�r
    public Text TxLPTitle;      // ��ܼ��D����r
    public Text TxLPMsg;        // ��ܰT������r

    private int totalTaskCount; // �`���ȼƶq
    private int currentProgress; // ��e�i��

    /// <summary>
    /// ��l�ƶi�ױ��C
    /// </summary>
    /// <param name="totalTaskCount">�`���ȼƶq</param>
    /// <param name="title">���D</param>
    /// <param name="msg">�T��</param>
    public void StartProgress(int totalTaskCount, string title = "", string msg = "")
    {
        gameObject.SetActive(true); // �ҥ� PanelLoadingProgress
        this.totalTaskCount = totalTaskCount;
        currentProgress = 0;
        UpdateUI(title, msg, true);
    }

    /// <summary>
    /// �]�w�i�צܫ��w�ƭȡC
    /// </summary>
    /// <param name="targetProgress">�ؼжi��</param>
    /// <param name="title">���D</param>
    /// <param name="msg">�T��</param>
    public void GoTo(int targetProgress, string title = "", string msg = "")
    {
        currentProgress = Mathf.Clamp(targetProgress, 0, totalTaskCount);
        UpdateUI(title, msg);
        CheckCompletion();
    }

    /// <summary>
    /// �W�[���w�i�סC
    /// </summary>
    /// <param name="progressToAdd">�W�[���i�׭�</param>
    /// <param name="title">���D</param>
    /// <param name="msg">�T��</param>
    public void Add(int progressToAdd, string title = "", string msg = "")
    {
        currentProgress = Mathf.Clamp(currentProgress + progressToAdd, 0, totalTaskCount);
        UpdateUI(title, msg);
        CheckCompletion();
    }

    /// <summary>
    /// ��s UI�C
    /// </summary>
    /// <param name="title">���D</param>
    /// <param name="msg">�T��</param>
    /// <param name="ifInstant">�O�_�ߧY��s</param>
    private void UpdateUI(string title, string msg, bool ifInstant = false)
    {
        // ��s�i�צʤ���
        float fillAmount = totalTaskCount > 0 ? (float)currentProgress / totalTaskCount : 0;
        if (ImgLoadingKnob != null)
        {
            if (ifInstant)
            {
                ImgLoadingKnob.fillAmount = fillAmount;
            }
            else
            {
                ImgLoadingKnob.DOFillAmount(fillAmount, 0.5f).SetEase(Ease.Linear); // �ϥ� DOTween �i�業�ưʵe
            }
        }

        // ��s�ʤ����r
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

        // ��s���D��r
        if (!string.IsNullOrEmpty(title) && TxLPTitle != null)
        {
            TxLPTitle.text = title;
        }

        // ��s�T����r
        if (!string.IsNullOrEmpty(msg) && TxLPMsg != null)
        {
            TxLPMsg.text = msg;
        }
    }

    /// <summary>
    /// �ˬd�i�׬O�_�����C
    /// </summary>
    private void CheckCompletion()
    {
        if (currentProgress >= totalTaskCount)
        {
            StartCoroutine(DeactivateAfterDelay(0.5f));
        }
    }

    /// <summary>
    /// ���𰱥Ϊ���C
    /// </summary>
    /// <param name="delay">����ɶ��]��^</param>
    /// <returns></returns>
    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
