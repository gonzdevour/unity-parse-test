using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BtnActivator : MonoBehaviour
{
    [Header("���O�ե�")]
    public Button Btn_Open;
    public Button Btn_Close;
    public GameObject Modal;
    public GameObject Board;
    public float transitionDuration = 0.5f; // �ʵe�ɶ�

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        // �T�O Board �� CanvasGroup
        canvasGroup = Board.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = Board.AddComponent<CanvasGroup>();
        }
        Btn_Open.onClick.AddListener(Open);
        Btn_Close.onClick.AddListener(Close);
    }

    public void Open()
    {
        Modal.SetActive(true); // �ҥΥD����
        Board.SetActive(true); // �ҥ� Board

        // �ҥΤl���󪺤���
        SetChildrenInteractable(Board, false);

        // �u�X�ʵe�G�Y�� + �H�J
        Board.transform.localScale = Vector3.zero;
        //canvasGroup.alpha = 0;

        Sequence openSequence = DOTween.Sequence()
            .Append(Board.transform.DOScale(1f, transitionDuration).SetEase(Ease.OutBack))
            //.Join(canvasGroup.DOFade(1f, transitionDuration))
            .OnComplete(() =>
            {
                SetChildrenInteractable(Board, true); // �ҥΤl���󪺤���
            });
    }

    public void Close()
    {
        // ����l���󪺾ާ@
        SetChildrenInteractable(Board, false);

        // ���X�ʵe�G�Y�� + �H�X
        Sequence closeSequence = DOTween.Sequence()
            .Append(Board.transform.DOScale(0f, transitionDuration).SetEase(Ease.InBack))
            //.Join(canvasGroup.DOFade(0f, transitionDuration))
            .OnComplete(() =>
            {
                Board.SetActive(false); // ���X���������� Board
                Modal.SetActive(false); // ���åD����
            });
    }

    private void SetChildrenInteractable(GameObject parent, bool state)
    {
        foreach (Selectable selectable in parent.GetComponentsInChildren<Selectable>(true))
        {
            selectable.interactable = state;
        }
    }
}
