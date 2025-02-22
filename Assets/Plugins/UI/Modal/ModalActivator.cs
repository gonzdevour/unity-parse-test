using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ModalActivator : MonoBehaviour
{
    [Header("面板組件")]
    public Button Btn_Open;
    public Button Btn_Close;
    public GameObject Modal;
    public GameObject Board;
    public float transitionDuration = 0.5f; // 動畫時間

    private CanvasGroup canvasGroup;
    private Button lastClickedButton; // 記錄最後觸發的按鈕

    private void Awake()
    {
        // 確保 Board 有 CanvasGroup
        canvasGroup = Board.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = Board.AddComponent<CanvasGroup>();
        }
        Btn_Open.onClick.AddListener(() => Open(this)); // 傳遞activator給reciever
        Btn_Close.onClick.AddListener(Close);
    }

    public void Open(ModalActivator activator)
    {
        Debug.Log($"開啟面板的按鈕: {activator.name}");
        var modalReciever = Modal.GetComponent<ModalReciever>();
        if (modalReciever != null) modalReciever.activator = activator; // 傳遞觸發的按鈕

        if (modalReciever == null) Debug.Log("modalReciever == null");
        if (modalReciever != null) Debug.Log($"modalReciever.activator.name = {modalReciever.activator.name}");

        Modal.SetActive(true); // 啟用主物件
        Board.SetActive(true); // 啟用 Board

        // 啟用子物件的互動
        SetChildrenInteractable(Board, false);

        // 彈出動畫：縮放 + 淡入
        Board.transform.localScale = Vector3.zero;
        //canvasGroup.alpha = 0;

        Sequence openSequence = DOTween.Sequence()
            .Append(Board.transform.DOScale(1f, transitionDuration).SetEase(Ease.OutBack))
            //.Join(canvasGroup.DOFade(1f, transitionDuration))
            .OnComplete(() =>
            {
                SetChildrenInteractable(Board, true); // 啟用子物件的互動
            });
    }

    public void Close()
    {
        // 停止子物件的操作
        SetChildrenInteractable(Board, false);

        // 收合動畫：縮放 + 淡出
        Sequence closeSequence = DOTween.Sequence()
            .Append(Board.transform.DOScale(0f, transitionDuration).SetEase(Ease.InBack))
            //.Join(canvasGroup.DOFade(0f, transitionDuration))
            .OnComplete(() =>
            {
                Board.SetActive(false); // 收合完成後隱藏 Board
                Modal.SetActive(false); // 隱藏主物件
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
