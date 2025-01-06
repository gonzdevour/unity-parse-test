using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TransitionEffectAnim : MonoBehaviour, ITransitionEffect
{
    private GameObject UnmaskCtnr;
    private Image ScreenImg;
    private float DurOut;
    private float DurIn;
    private Ease EaseOut;
    private Ease EaseIn;
    private Ease EaseCurrent;
    private Tween activeTween;
    private System.Action onComplete; // �Ω�O�s�^�հѼ�
    private AnimEvents animEvents;
    private float animDirection;

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
        this.onComplete = onComplete;
        //Debug.Log("circle fade in");
        // �������i�椤�� Tween�A��l��
        Stop(true);
        ScreenImg.gameObject.SetActive(true);

        // ����Unmask
        GameObject prefab = Resources.Load<GameObject>($"prefabs/{prefabName}");
        var inst = Instantiate(prefab, Vector3.zero, Quaternion.identity, UnmaskCtnr.transform);
        var RTrans = inst.GetComponent<RectTransform>();

        // �q�\�^�ըƥ�
        animEvents = inst.GetComponent<AnimEvents>();
        animEvents.OnAnimationStartCallback += OnAnimationStart;
        animEvents.OnAnimationEndCallback += OnAnimationEnd;

        // ���oanimator
        var animator = inst.GetComponent<Animator>();

        // �NLocal Position �k 0
        RTrans.localPosition = Vector3.zero;

        // �]�wsize��Screen����
        float screenMaxDimension = Mathf.Max(ScreenImg.rectTransform.rect.width, ScreenImg.rectTransform.rect.height);
        RTrans.sizeDelta = new Vector2(screenMaxDimension, screenMaxDimension);

        //����ʵe
        animDirection = -1;
        animator.SetFloat("Direction", -1);
        animator.Play("TEffect_Water01", -1, float.NegativeInfinity);
    }

    public void FadeOut(System.Action onComplete)
    {
        this.onComplete = onComplete;
        //Debug.Log("circle fade out");
        // �������i�椤�� Tween�A��l��
        Stop(true);
        ScreenImg.gameObject.SetActive(true);

        // ����Unmask
        GameObject prefab = Resources.Load<GameObject>($"prefabs/{prefabName}");
        var inst = Instantiate(prefab, Vector3.zero, Quaternion.identity, UnmaskCtnr.transform);
        var RTrans = inst.GetComponent<RectTransform>();

        // �q�\�^�ըƥ�
        animEvents = inst.GetComponent<AnimEvents>();
        animEvents.OnAnimationStartCallback += OnAnimationStart;
        animEvents.OnAnimationEndCallback += OnAnimationEnd;

        // ���oanimator
        var animator = inst.GetComponent<Animator>();

        // �NLocal Position �k 0
        RTrans.localPosition = Vector3.zero;

        // �]�wsize��Screen����
        float screenMaxDimension = Mathf.Max(ScreenImg.rectTransform.rect.width, ScreenImg.rectTransform.rect.height);
        RTrans.sizeDelta = new Vector2(screenMaxDimension, screenMaxDimension);

        //����ʵe
        animDirection = 1;
        animator.speed = 1; // ���V����
        animator.Play("TEffect_Water01", -1, 0f); // �q�ʵe�_�I�}�l
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

    private void OnAnimationEnd()
    {
        if (animDirection == 1) //fadeOut
        {
            Debug.Log("OnAnimationEnd");
            // �����q�\�A����^�զA��Ĳ�o
            animEvents.OnAnimationEndCallback -= OnAnimationEnd;

            activeTween = null;
            ScreenImg.gameObject.SetActive(true);
            foreach (Transform child in UnmaskCtnr.transform)
            {
                Destroy(child.gameObject); // �R���Ҧ�Unmask
            }
            onComplete?.Invoke();
            onComplete = null;
        }
        else
        {
            ScreenImg.gameObject.SetActive(true); //fadeIn
        }

    }

    private void OnAnimationStart()
    {
        if (animDirection == -1) //fadeIn
        {
            Debug.Log("OnAnimationStart");
            // �����q�\�A����^�զA��Ĳ�o
            animEvents.OnAnimationStartCallback -= OnAnimationStart;

            activeTween = null;
            ScreenImg.gameObject.SetActive(false);
            foreach (Transform child in UnmaskCtnr.transform)
            {
                Destroy(child.gameObject); // �R���Ҧ�Unmask
            }
            onComplete?.Invoke();
            onComplete = null;
        }
        else
        {
            ScreenImg.gameObject.SetActive(true); //fadeOut
        }

    }
}
