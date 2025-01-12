using UnityEngine;
using DG.Tweening;
using System.Collections; // �T�O�w�g�w�� DOTween �èϥΦ��R�W�Ŷ�

public class ImageTweenSize : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 initialSize;
    private Tween sizeTween;

    [Header("Tween Settings")]
    public int loopTimes = -1;
    public bool StartOnEnable = false;
    public bool ResetAfterTween = false;
    public bool DestroyAfterTween = false;
    public float relativeWidth = 0.1f; // �۹�e�ס]��ҡ^
    public float relativeHeight = 0.1f; // �۹ﰪ�ס]��ҡ^
    public float durIncrease = 0.5f; // �W�j�ؤo����ɶ�
    public float durDecrease = 0.5f; // ��p�ؤo����ɶ�
    public Ease easeIncrease = Ease.Linear; // �W�j�ؤo���w�ʮĪG
    public Ease easeDecrease = Ease.Linear; // ��p�ؤo���w�ʮĪG
    public float delay = 0f; // �ʵe����
    public Vector2 initial = Vector2.zero;

    /// <summary>
    /// ��l�ƨç�s�ܼơC
    /// </summary>
    public void Init(
        int? loopTimes = null,
        bool? startOnEnable = null,
        bool? resetAfterTween = null,
        bool? destroyAfterTween = null,
        float? relativeWidth = null,
        float? relativeHeight = null,
        float? durIncrease = null,
        float? durDecrease = null,
        Ease? easeIncrease = null,
        Ease? easeDecrease = null,
        float? delay = null,
        Vector2? initial = null
    )
    {
        if (loopTimes.HasValue) this.loopTimes = loopTimes.Value;
        if (startOnEnable.HasValue) this.StartOnEnable = startOnEnable.Value;
        if (resetAfterTween.HasValue) this.ResetAfterTween = resetAfterTween.Value;
        if (destroyAfterTween.HasValue) this.DestroyAfterTween = destroyAfterTween.Value;
        if (relativeWidth.HasValue) this.relativeWidth = Mathf.Clamp01(relativeWidth.Value);
        if (relativeHeight.HasValue) this.relativeHeight = Mathf.Clamp01(relativeHeight.Value);
        if (durIncrease.HasValue) this.durIncrease = Mathf.Max(0, durIncrease.Value);
        if (durDecrease.HasValue) this.durDecrease = Mathf.Max(0, durDecrease.Value);
        if (easeIncrease.HasValue) this.easeIncrease = easeIncrease.Value;
        if (easeDecrease.HasValue) this.easeDecrease = easeDecrease.Value;
        if (delay.HasValue) this.delay = Mathf.Max(0, delay.Value);
        if (initial.HasValue) this.initial = initial.Value;
    }

    private void Awake()
    {
        // ��� RectTransform
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("���}�����������b�֦� RectTransform ������W�I");
            enabled = false;
        }
    }

    private void OnEnable()
    {
        // �O����l�ؤo
        if (initial != Vector2.zero)
        {
            rectTransform.sizeDelta = initial;
        }
        initialSize = rectTransform.sizeDelta;

        // �}�l Tween �ʵe
        if (StartOnEnable) StartCoroutine(Tween(delay));
    }

    private void OnDisable()
    {
        // ����ʵe�í��m�ؤo
        StopTween();
    }

    private IEnumerator Tween(float Delay = 0f)
    {
        yield return new WaitForSeconds(Delay);

        // ����w���� Tween
        sizeTween?.Kill();

        // �]�w�ؼФؤo���۹���l�ؤo�����
        Vector2 targetSize = new Vector2(
            initialSize.x * (1 + relativeWidth),
            initialSize.y * (1 + relativeHeight)
        );

        // �ЫبӦ^�`�� Tween �ʵe
        sizeTween = rectTransform.DOSizeDelta(targetSize, durIncrease)
            .SetEase(easeIncrease) // �W�j�ؤo���w�ʮĪG
            .SetLoops(loopTimes, LoopType.Yoyo) // �L���Ӧ^�`��
            .OnStepComplete(() =>
            {
                // �ʵe��V���ܮɤ��� Ease
                if (sizeTween.IsPlaying())
                {
                    sizeTween.SetEase(sizeTween.CompletedLoops() % 2 == 0 ? easeDecrease : easeIncrease);
                }
            })
            .OnComplete(() => 
            { 
                if (ResetAfterTween) rectTransform.sizeDelta = initialSize;
                if (DestroyAfterTween) Destroy(gameObject); 
            });
    }

    private void StopTween()
    {
        // ����ʵe
        sizeTween?.Kill();

        // �^���l�ؤo
        rectTransform.sizeDelta = initialSize;
    }
}
