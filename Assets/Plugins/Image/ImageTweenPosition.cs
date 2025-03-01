using UnityEngine;
using DG.Tweening;
using System.Collections; // �T�O�w�g�w�� DOTween �èϥΦ��R�W�Ŷ�

public class ImageTweenPosition : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector3 initialPosition;
    private Tween moveTween;

    [Header("Tween Settings")]
    public int loopTimes = -1;
    public bool StartOnEnable = false;
    public bool ResetAfterTween = false;
    public bool DestroyAfterTween = false;
    public float targetX = 0f; // �ؼЦ첾�q X
    public float targetY = -20f; // �ؼЦ첾�q Y
    public float durForward = 0.5f; // �e���ؼЮɶ�
    public float durBackward = 0.5f; // ��^��l��m���ɶ�
    public Ease easeForward = Ease.Linear; // �e���ؼЪ��w�ʮĪG
    public Ease easeBackward = Ease.Linear; // ��^��l��m���w�ʮĪG
    public float delay = 0f;
    public Vector2 initial = Vector2.zero;

    /// <summary>
    /// ��l�ƨç�s�ܼơC
    /// </summary>
    public void Init(
        int? loopTimes = null,
        bool? startOnEnable = null,
        bool? resetAfterTween = null,
        bool? destroyAfterTween = null,
        float? targetX = null,
        float? targetY = null,
        float? durForward = null,
        float? durBackward = null,
        Ease? easeForward = null,
        Ease? easeBackward = null,
        float? delay = null,
        Vector2? initial = null
    )
    {
        if (loopTimes.HasValue) this.loopTimes = loopTimes.Value;
        if (startOnEnable.HasValue) this.StartOnEnable = startOnEnable.Value;
        if (resetAfterTween.HasValue) this.ResetAfterTween = resetAfterTween.Value;
        if (destroyAfterTween.HasValue) this.DestroyAfterTween = destroyAfterTween.Value;
        if (targetX.HasValue) this.targetX = targetX.Value;
        if (targetY.HasValue) this.targetY = targetY.Value;
        if (durForward.HasValue) this.durForward = Mathf.Max(0, durForward.Value);
        if (durBackward.HasValue) this.durBackward = Mathf.Max(0, durBackward.Value);
        if (easeForward.HasValue) this.easeForward = easeForward.Value;
        if (easeBackward.HasValue) this.easeBackward = easeBackward.Value;
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
        // �O����l��m
        if (initial != Vector2.zero)
        {
            rectTransform.localPosition = initial;
        }
        initialPosition = rectTransform.localPosition;

        // �}�l Tween �ʵe
        if (StartOnEnable) StartCoroutine(Tween(delay));
    }

    private void OnDisable()
    {
        // ����ʵe�í��m��m
        StopTween();
    }

    private IEnumerator Tween(float Delay = 0f)
    {
        yield return new WaitForSeconds(Delay);

        // ����w���� Tween
        moveTween?.Kill();

        // �]�w�ؼЦ�m
        Vector3 targetPosition = initialPosition + new Vector3(targetX, targetY, 0);

        // �ЫبӦ^�`�� Tween �ʵe
        moveTween = rectTransform.DOLocalMove(targetPosition, durForward)
            .SetEase(easeForward) // �e���ؼЪ��w�ʮĪG
            .SetLoops(loopTimes, LoopType.Yoyo) // �L���Ӧ^�`��
            .OnStepComplete(() =>
            {
                // �ʵe��V���ܮɤ��� Ease
                if (moveTween.CompletedLoops() % 2 == 0)
                {
                    moveTween.SetEase(easeBackward);
                }
                else
                {
                    moveTween.SetEase(easeForward);
                }
            })
            .OnComplete(() => 
            { 
                if (ResetAfterTween) rectTransform.localPosition = initialPosition;
                if (DestroyAfterTween) Destroy(gameObject); 
            });

    }

    private void StopTween()
    {
        // ����ʵe
        moveTween?.Kill();

        // �^���l��m
        rectTransform.localPosition = initialPosition;
    }
}
