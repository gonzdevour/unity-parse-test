using UnityEngine;
using DG.Tweening;
using System.Collections; // �T�O�w�g�w�� DOTween �èϥΦ��R�W�Ŷ�

public class ImageTweenAngle : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector3 initialRotation;
    private Tween angleTween;

    [Header("Tween Settings")]
    public int loopTimes = -1;
    public bool StartOnEnable = false;
    public bool ResetAfterTween = false;
    public bool DestroyAfterTween = false;
    public float relativeAngle = 30f; // �۹���ਤ�ס]�׼ơ^
    public float durIncrease = 0.5f; // �W�[�������ɶ�
    public float durDecrease = 0.5f; // ��ֱ������ɶ�
    public Ease easeIncrease = Ease.Linear; // �W�[���઺�w�ʮĪG
    public Ease easeDecrease = Ease.Linear; // ��ֱ��઺�w�ʮĪG
    public float delay = 0f; // �ʵe����
    public float initial = -1f;

    /// <summary>
    /// ��l�ƨç�s�ܼơC
    /// </summary>
    public void Init(
        int? loopTimes = null,
        bool? startOnEnable = null,
        bool? resetAfterTween = null,
        bool? destroyAfterTween = null,
        float? relativeAngle = null,
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
        if (relativeAngle.HasValue) this.relativeAngle = relativeAngle.Value;
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
        // ��l����
        if (initial != -1f)
        {
            rectTransform.localEulerAngles = new Vector3(0, 0, initial);
        }
        // �O����l���ਤ��
        initialRotation = rectTransform.localEulerAngles;

        // �}�l Tween �ʵe
        if (StartOnEnable) StartCoroutine(Tween(delay));
    }

    private void OnDisable()
    {
        // ����ʵe�í��m����
        StopTween();
    }

    private IEnumerator Tween(float Delay = 0f)
    {
        yield return new WaitForSeconds(Delay);

        // ����w���� Tween
        angleTween?.Kill();

        // �]�w�ؼб��ਤ�׬��۹���l���઺���
        Vector3 targetRotation = initialRotation + new Vector3(0, 0, relativeAngle);

        // �ЫبӦ^�`�� Tween �ʵe
        angleTween = rectTransform.DOLocalRotate(targetRotation, durIncrease, RotateMode.FastBeyond360)
            .SetEase(easeIncrease) // �W�[���઺�w�ʮĪG
            .SetLoops(loopTimes, LoopType.Yoyo) // �L���Ӧ^�`��
            .OnStepComplete(() =>
            {
                // �ʵe��V���ܮɤ��� Ease
                if (angleTween.IsPlaying())
                {
                    angleTween.SetEase(angleTween.CompletedLoops() % 2 == 0 ? easeDecrease : easeIncrease);
                }
            })
            .OnComplete(() => 
            {
                if (ResetAfterTween) rectTransform.localEulerAngles = initialRotation;
                if (DestroyAfterTween) Destroy(gameObject); 
            });
    }

    private void StopTween()
    {
        // ����ʵe
        angleTween?.Kill();

        // �^���l����
        rectTransform.localEulerAngles = initialRotation;
    }
}
