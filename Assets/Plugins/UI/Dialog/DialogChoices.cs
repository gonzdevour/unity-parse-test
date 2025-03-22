using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class DialogChoices : MonoBehaviour
{
    public GameObject Modal;
    public GameObject Board;
    public Text TxTitle;
    public Text TxContent;
    public Transform ChoiceParent;
    public GameObject ChoicePrefab;
    public float transitionDuration = 0.5f;

    private List<GameObject> buttons = new();
    private Action<string, GameObject> cbkY;

    public void Open(string[] options, string[] results,string Title = "", string Content = "", Action<string, GameObject> CallbackY = null)
    {
        TxTitle.gameObject.SetActive(!string.IsNullOrEmpty(Title));
        TxContent.gameObject.SetActive(!string.IsNullOrEmpty(Content));
        TxTitle.text = Title;
        TxContent.text = Content;
        cbkY = CallbackY;

        for (int i = 0; i < options.Length && i < results.Length; i++)
        {
            string curOption = options[i];
            string curResult = results[i];
            //Debug.Log($"�ﶵ�G{curOption}/�e��{curResult}");
            // �Ыث��s
            GameObject button = Instantiate(ChoicePrefab, ChoiceParent);
            // �]�m���s��r
            button.GetComponentInChildren<Text>().text = curOption;
            // �]�m���s�^��
            Button btn = button.GetComponent<Button>();
            btn.onClick.AddListener(() => OnChoiceSelected(curResult, button));

            buttons.Add(button);
        }
        StartCoroutine(WaitForAnim());
        // ���楻�a��
        Loc.Inst.Setup(gameObject);
    }

    private IEnumerator WaitForAnim()
    {
        // ����LayoutGroup�ƦC����
        yield return new WaitForEndOfFrame();
        // ���s�ʵe
        foreach (var button in buttons)
        {
            Image buttonImg = button.GetComponent<Image>();
            SetImageRGBA(buttonImg, a: 0f);
            buttonImg.DOFade(1f, 0.5f).SetEase(Ease.Linear); // ��ܰʵe
            Vector3 posTarget = button.transform.localPosition;// �]�w���J�ؼЦ�m
            Vector3 posStart = new Vector3(posTarget.x, posTarget.y+100, posTarget.z);// �]�w���J��l��m
            button.transform.localPosition = posStart; 
            button.transform.DOLocalMove(posTarget, 0.5f).SetEase(Ease.OutBack); // ���J�ʵe
        }
    }

    public void Close(string resultString, GameObject resultButton)
    {
        // ����l���󪺾ާ@
        SetChildrenInteractable(Board, false);
        // ���X�ʵe
        Sequence sequence = DOTween.Sequence();
        foreach (var button in buttons)
        {
            Image buttonImg = button.GetComponent<Image>();
            SetImageRGBA(buttonImg, a: 1f);
            sequence.Join(buttonImg.DOFade(0f, 0.5f).SetEase(Ease.Linear)); // ���ðʵe
            Vector3 posStart = button.transform.localPosition;// �]�w���X��l��m
            Vector3 posTarget = new Vector3(posStart.x, posStart.y + 100, posStart.z);// �]�w���X�ؼЦ�m
            sequence.Join(button.transform.DOLocalMove(posTarget, 0.5f).SetEase(Ease.OutBack)); // ���J�ʵe
        }
        sequence.OnComplete(() =>
        {
            cbkY?.Invoke(resultString, resultButton);
            Destroy(gameObject);
        });
    }

    private void OnChoiceSelected(string resultString, GameObject button)
    {
        //button�ʺA��modal�����ʺA
        button.transform.DOScale(1.1f, 0.2f)
        .SetLoops(2, LoopType.Yoyo)
        .OnComplete(() =>
        {
            Close(resultString, button);
        });
    }

    private void SetChildrenInteractable(GameObject parent, bool state)
    {
        foreach (Selectable selectable in parent.GetComponentsInChildren<Selectable>(true))
        {
            selectable.interactable = state;
        }
    }

    private void SetImageRGBA(Image img, float r = -1f, float g = -1f, float b = -1f, float a = -1f)
    {
        Color newColor = img.color;
        if (r > -1f) { newColor.r = r; };
        if (g > -1f) { newColor.g = g; };
        if (b > -1f) { newColor.b = b; };
        if (a > -1f) { newColor.a = a; };
        img.color = newColor;
    }
}

