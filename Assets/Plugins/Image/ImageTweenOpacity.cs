using UnityEngine;
using DG.Tweening;
using UnityEngine.UI; // 確保 Image 組件可用
using System.Collections;
using Unity.VisualScripting;

public class ImageTweenOpacity : MonoBehaviour
{
    private Image img;
    private float initialColorA;
    private Tween TweenColorA;

    [Header("Tween Settings")]
    public int loopTimes = -1;
    public bool StartOnEnable = false;
    public bool ResetAfterTween = false;
    public bool DestroyAfterTween = false;
    public float relativeOpacity = 1f; // 相對透明度變化量（0 到 1）
    public float durIncrease = 0.5f; // 增加透明度持續時間
    public float durDecrease = 0.5f; // 減少透明度持續時間
    public Ease easeIncrease = Ease.Linear; // 增加透明度的緩動效果
    public Ease easeDecrease = Ease.Linear; // 減少透明度的緩動效果
    public float delay = 0f; // 動畫延遲
    public float initial = -1f;

    public void Init
    (
    //各位觀眾，具名引數~
    int? loopTimes = null,
    bool? startOnEnable = null,
    bool? resetAfterTween = null,
    bool? destroyAfterTween = null,
    float? relativeOpacity = null,
    float? durIncrease = null,
    float? durDecrease = null,
    Ease? easeIncrease = null,
    Ease? easeDecrease = null,
    float? delay = null,
    float? initial = null
    )
    {
        if (loopTimes.HasValue) this.loopTimes = loopTimes.Value;
        if (startOnEnable.HasValue) this.StartOnEnable = startOnEnable.Value;
        if (resetAfterTween.HasValue) this.ResetAfterTween = resetAfterTween.Value;
        if (destroyAfterTween.HasValue) this.DestroyAfterTween = destroyAfterTween.Value;
        if (relativeOpacity.HasValue) this.relativeOpacity = Mathf.Clamp01(relativeOpacity.Value);
        if (durIncrease.HasValue) this.durIncrease = Mathf.Max(0, durIncrease.Value);
        if (durDecrease.HasValue) this.durDecrease = Mathf.Max(0, durDecrease.Value);
        if (easeIncrease.HasValue) this.easeIncrease = easeIncrease.Value;
        if (easeDecrease.HasValue) this.easeDecrease = easeDecrease.Value;
        if (delay.HasValue) this.delay = Mathf.Max(0, delay.Value);
        if (initial.HasValue) this.initial = initial.Value;
    }

    private void Awake()
    {
        // 獲取 Image 組件
        img = GetComponent<Image>();
        if (img == null)
        {
            Debug.LogError("此腳本必須掛載在擁有 Image 組件的物件上！");
            enabled = false;
        }
    }

    private void OnEnable()
    {
        // 初始透明度
        if (initial != -1f)
        {
            img = GetComponent<Image>();
            img.color = new Color(img.color.r, img.color.g, img.color.b, initial);
        }
        initialColorA = img.color.a;

        // 開始 Tween 動畫
        if (StartOnEnable) StartCoroutine(Tween(delay));
    }

    private void OnDisable()
    {
        // 停止動畫並重置透明度
        StopTween();
    }

    private IEnumerator Tween(float Delay = 0f)
    {
        yield return new WaitForSeconds(Delay);

        // 停止已有的 Tween
        TweenColorA?.Kill();

        // 設定目標透明度為相對於初始透明度的變化
        float targetOpacity = Mathf.Clamp(initialColorA + relativeOpacity, 0f, 1f);

        // 創建來回循環 Tween 動畫
        TweenColorA = DOTween.To(
            () => img.color,
            color => img.color = color,
            new Color(img.color.r, img.color.g, img.color.b, targetOpacity),
            durIncrease
        )
        .SetEase(easeIncrease) // 增加透明度的緩動效果
        .SetLoops(loopTimes, LoopType.Yoyo) // 無限來回循環
        .OnStepComplete(() =>
        {
            // 動畫方向改變時切換 Ease
            if (TweenColorA.IsPlaying())
            {
                TweenColorA.SetEase(TweenColorA.CompletedLoops() % 2 == 0 ? easeDecrease : easeIncrease);
            }
        })
        .OnComplete(() => 
        { 
            if( ResetAfterTween ) img.color = new Color(img.color.r, img.color.g, img.color.b, initial);
            if (DestroyAfterTween) Destroy(gameObject); 
        });
    }

    private void StopTween()
    {
        // 停止動畫
        TweenColorA?.Kill();

        // 回到初始透明度
        var color = img.color;
        color.a = initialColorA;
        img.color = color;
    }
}
