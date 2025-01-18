using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public partial class AVG
{
    IEnumerator StartChoose(Dictionary<string, object> storyCutDict)
    {
        // ��l�ƭ��O
        ChoicePanel.SetActive(true);
        ClearExistingButtons();

        // ��l�ƿ�ܪ��A
        isChoiceSelected = false;

        string[] targets = storyCutDict["�e��"].ToString().Split('\n');
        string[] options = storyCutDict["�ﶵ"].ToString().Split('\n');
        List<GameObject> buttons = new List<GameObject>();

        for (int i = 0; i < options.Length && i < targets.Length; i++)
        {
            // �Ыث��s
            GameObject button = Instantiate(ChoicePrefab, ChoicePanel.transform);
            button.GetComponentInChildren<Text>().text = TxR.Inst.Render(ParseEx(options[i])); // �]�m���s��r
            int resultCutIndex = int.Parse(ParseEx(targets[i]));

            // �]�m���s�^��
            Button btn = button.GetComponent<Button>();
            btn.onClick.AddListener(() => OnChoiceSelected(resultCutIndex, button));

            // ���s���J�ʵe
            //button.transform.localPosition = new Vector3(0, 500, 0); // ��l��m
            //button.transform.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutBounce); // ���J�ʵe
            buttons.Add(button);
        }

        // ���ݿ�ܧ���
        yield return new WaitUntil(() => isChoiceSelected);

        // ���s���X�ʵe
        foreach (var button in buttons)
        {
            button.transform.DOLocalMoveY(500, 0.5f).SetEase(Ease.InBack)
                .OnComplete(() => Destroy(button)); // �ʵe������P�����s
        }

        // ���ݭ��X�ʵe����
        yield return new WaitForSeconds(0.5f);

        // ���í��O
        ChoicePanel.SetActive(false);
    }

    private void ClearExistingButtons()
    {
        foreach (Transform child in ChoicePanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnChoiceSelected(int resultIndex, GameObject button)
    {
        gotoIndex = resultIndex;
        isChoiceSelected = true;
        Debug.Log($"choose to go to cut: {gotoIndex}");
    }
}
