using UnityEngine;
using DG.Tweening; // �T�O�w�g�w�� DOTween �èϥΦ��R�W�Ŷ�

public class ImageTweenPosition : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector3 initialPosition;
    private Tween moveTween;

    [Header("Tween Settings")]
    public float targetX = 0f; // �ؼЦ첾�q X
    public float targetY = -20f; // �ؼЦ첾�q Y
    public float durForward = 0.5f; // �ؼЦ첾�q X
    public float durBackward = 0.5f; // �ؼЦ첾�q Y
    public Ease easeForward = Ease.InOutSine; // �e���ؼЪ��w�ʮĪG
    public Ease easeBackward = Ease.InOutSine; // ��^��l��m���w�ʮĪG

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
        initialPosition = rectTransform.localPosition;

        // �}�l Tween �ʵe
        StartBounceTween();
    }

    private void OnDisable()
    {
        // ����ʵe�í��m��m
        StopBounceTween();
    }

    private void StartBounceTween()
    {
        // ����w���� Tween
        moveTween?.Kill();

        // �]�w�ؼЦ�m
        Vector3 targetPosition = initialPosition + new Vector3(targetX, targetY, 0);

        // �ЫبӦ^�`�� Tween �ʵe
        moveTween = rectTransform.DOLocalMove(targetPosition, 0.5f)
            .SetEase(easeForward) // �e���ؼЪ��w�ʮĪG
            .SetLoops(-1, LoopType.Yoyo) // �L���Ӧ^�`��
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
            });

    }

    private void StopBounceTween()
    {
        // ����ʵe
        moveTween?.Kill();

        // �^���l��m
        rectTransform.localPosition = initialPosition;
    }
}
