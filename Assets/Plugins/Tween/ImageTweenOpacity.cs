using UnityEngine;
using DG.Tweening;
using UnityEngine.UI; // �T�O Image �ե�i��
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
    public float relativeOpacity = 1f; // �۹�z�����ܤƶq�]0 �� 1�^
    public float durIncrease = 0.5f; // �W�[�z���׫���ɶ�
    public float durDecrease = 0.5f; // ��ֳz���׫���ɶ�
    public Ease easeIncrease = Ease.Linear; // �W�[�z���ת��w�ʮĪG
    public Ease easeDecrease = Ease.Linear; // ��ֳz���ת��w�ʮĪG
    public float delay = 0f; // �ʵe����
    public float initial = -1f;

    public void Init
    (
    //�U���[���A��W�޼�~
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
        // ��� Image �ե�
        img = GetComponent<Image>();
        if (img == null)
        {
            Debug.LogError("���}�����������b�֦� Image �ե󪺪���W�I");
            enabled = false;
        }
    }

    private void OnEnable()
    {
        // ��l�z����
        if (initial != -1f)
        {
            img = GetComponent<Image>();
            img.color = new Color(img.color.r, img.color.g, img.color.b, initial);
        }
        initialColorA = img.color.a;

        // �}�l Tween �ʵe
        if (StartOnEnable) StartCoroutine(Tween(delay));
    }

    private void OnDisable()
    {
        // ����ʵe�í��m�z����
        StopTween();
    }

    private IEnumerator Tween(float Delay = 0f)
    {
        yield return new WaitForSeconds(Delay);

        // ����w���� Tween
        TweenColorA?.Kill();

        // �]�w�ؼгz���׬��۹���l�z���ת��ܤ�
        float targetOpacity = Mathf.Clamp(initialColorA + relativeOpacity, 0f, 1f);

        // �ЫبӦ^�`�� Tween �ʵe
        TweenColorA = DOTween.To(
            () => img.color,
            color => img.color = color,
            new Color(img.color.r, img.color.g, img.color.b, targetOpacity),
            durIncrease
        )
        .SetEase(easeIncrease) // �W�[�z���ת��w�ʮĪG
        .SetLoops(loopTimes, LoopType.Yoyo) // �L���Ӧ^�`��
        .OnStepComplete(() =>
        {
            // �ʵe��V���ܮɤ��� Ease
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
        // ����ʵe
        TweenColorA?.Kill();

        // �^���l�z����
        var color = img.color;
        color.a = initialColorA;
        img.color = color;
    }
}
