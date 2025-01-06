using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TransitionEffectAnim2 : MonoBehaviour, ITransitionEffect
{
    private GameObject UnmaskCtnr;
    private Image ScreenImg;
    private float DurOut;
    private float DurIn;
    private Ease EaseOut;
    private Ease EaseIn;
    private Ease EaseCurrent;
    private Tween activeTween;
    private bool IsForward;
    private string animationName = "TEffect_Water01";

    public void Init(TransitionEffectConfig config)
    {
        UnmaskCtnr = config.UnmaskCtnr;
        ScreenImg = config.ScreenImg;
        DurOut = config.DurOut;
        DurIn = config.DurIn;
        EaseOut = config.EaseOut;
        EaseIn = config.EaseIn;

        activeTween = null;
    }

    private string prefabName = "Unmask_Water01";

    public void FadeIn(System.Action onComplete)
    {
        Stop(true); // �����e���ɶ�

        ScreenImg.gameObject.SetActive(true);

        // ���� Unmask
        GameObject prefab = Resources.Load<GameObject>($"prefabs/{prefabName}");
        var inst = Instantiate(prefab, Vector3.zero, Quaternion.identity, UnmaskCtnr.transform);
        var RTrans = inst.GetComponent<RectTransform>();

        RTrans.localPosition = Vector3.zero;
        RTrans.sizeDelta = new Vector2(ScreenImg.rectTransform.rect.width, ScreenImg.rectTransform.rect.height);

        EaseCurrent = EaseIn;
        IsForward = false;

        float startTime = 0.99f; // FadeIn �f�V����A�q���I�}�l(��1���ܷ|���@�V�^��_�I�Ӱ{�{�A�ҥH�n�Τp��1����)
        float endTime = 0f;   // �����_�I

        var animator = inst.GetComponent<Animator>();
        animator.Play(animationName, 0, startTime);
        animator.Update(0f); // �j���s���A

        activeTween = DOTween.To(
            () => animator.GetCurrentAnimatorStateInfo(0).normalizedTime,
            x => animator.Play(animationName, 0, x),
            endTime,
            DurIn
        ).SetEase(EaseCurrent)
         .OnComplete(() =>
         {
             activeTween = null;
             ScreenImg.gameObject.SetActive(false);
             foreach (Transform child in UnmaskCtnr.transform)
             {
                 Destroy(child.gameObject);
             }
             onComplete?.Invoke();
         });
    }


    public void FadeOut(System.Action onComplete)
    {
        //Debug.Log("circle fade out");
        // �������i�椤�� Tween�A��l��
        Stop(true);
        ScreenImg.gameObject.SetActive(true);

        // ����Unmask
        GameObject prefab = Resources.Load<GameObject>($"prefabs/{prefabName}");
        var inst = Instantiate(prefab, Vector3.zero, Quaternion.identity, UnmaskCtnr.transform);
        var RTrans = inst.GetComponent<RectTransform>();

        // �N circle �� Local Position �k 0
        RTrans.localPosition = Vector3.zero;

        // �Nunmask�]���PScreen�ۦP�j�p
        RTrans.sizeDelta = new Vector2(ScreenImg.rectTransform.rect.width, ScreenImg.rectTransform.rect.height);

        // �M�wEase�P�����V
        EaseCurrent = EaseOut;
        IsForward = true;

        // �]�m�ʵe�_�l�i��
        float startTime = IsForward ? 0f : 1f;
        float endTime = IsForward ? 1f : 0f;

        // ���oanimator
        var animator = inst.GetComponent<Animator>();

        // ����ʵe
        animator.Play(animationName, 0, startTime);

        // �ϥ� DOTween �ɶ� normalizedTime
        activeTween = DOTween.To(
            () => animator.GetCurrentAnimatorStateInfo(0).normalizedTime,
            x => animator.Play(animationName, 0, x),
            endTime,
            DurOut
        ).SetEase(EaseCurrent)
         .OnComplete(() =>
         {
             activeTween = null; // �M�z Tween
             ScreenImg.gameObject.SetActive(true);
             foreach (Transform child in UnmaskCtnr.transform)
             {
                 Destroy(child.gameObject); // �R���Ҧ�Unmask
             }
             onComplete?.Invoke(); // ����^��
         });
    }

    public void Stop(bool instant)
    {
        // ������󥿦b�i�檺 Tween
        if (activeTween != null && activeTween.IsActive())
        {
            activeTween.Kill(true);
        }

        if (instant)
        {
            if (activeTween != null && activeTween.IsPlaying())
            {
                // �ھ� Tween �O FadeIn �� FadeOut �P�_
                if (EaseCurrent == EaseIn)
                {
                    // FadeIn ���̲ת��A
                    ScreenImg.gameObject.SetActive(false);
                    foreach (Transform child in UnmaskCtnr.transform)
                    {
                        Destroy(child.gameObject); // �R���Ҧ�Unmask
                    }
                }
                else if (EaseCurrent == EaseOut)
                {
                    // FadeOut ���̲ת��A
                    ScreenImg.gameObject.SetActive(true);
                    foreach (Transform child in UnmaskCtnr.transform)
                    {
                        Destroy(child.gameObject); // �R���Ҧ�Unmask
                    }
                }
            }
        }
        else
        {
            if (activeTween != null && activeTween.IsPlaying())
            {
                // �ھ� Tween �O FadeIn �� FadeOut �P�_
                if (EaseCurrent == EaseIn)
                {
                    // FadeIn ���̲ת��A
                    ScreenImg.gameObject.SetActive(true);
                }
                else if (EaseCurrent == EaseOut)
                {
                    // FadeOut ���̲ת��A
                    ScreenImg.gameObject.SetActive(false);
                }
                foreach (Transform child in UnmaskCtnr.transform)
                {
                    Destroy(child.gameObject); // �R���Ҧ�Unmask
                }
            }
        }

        activeTween = null; // �M�z Active Tween
    }
}
