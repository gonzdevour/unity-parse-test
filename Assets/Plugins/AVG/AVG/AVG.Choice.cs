using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public partial class AVG
{
    IEnumerator StartChoose(Dictionary<string, object> storyCutDict)
    {
        // 初始化面板
        ChoicePanel.SetActive(true);
        ClearExistingButtons();

        // 初始化選擇狀態
        isChoiceSelected = false;

        string[] targets = storyCutDict["前往"].ToString().Split('\n');
        string[] options = storyCutDict["選項"].ToString().Split('\n');
        List<GameObject> buttons = new List<GameObject>();

        for (int i = 0; i < options.Length && i < targets.Length; i++)
        {
            // 創建按鈕
            GameObject button = Instantiate(ChoicePrefab, ChoicePanel.transform);
            button.GetComponentInChildren<Text>().text = TxR.Inst.Render(ParseEx(options[i])); // 設置按鈕文字
            int resultCutIndex = int.Parse(ParseEx(targets[i]));

            // 設置按鈕回調
            Button btn = button.GetComponent<Button>();
            btn.onClick.AddListener(() => OnChoiceSelected(resultCutIndex, button));

            // 按鈕飛入動畫
            //button.transform.localPosition = new Vector3(0, 500, 0); // 初始位置
            //button.transform.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutBounce); // 飛入動畫
            buttons.Add(button);
        }

        // 等待選擇完成
        yield return new WaitUntil(() => isChoiceSelected);

        // 按鈕飛出動畫
        foreach (var button in buttons)
        {
            button.transform.DOLocalMoveY(500, 0.5f).SetEase(Ease.InBack)
                .OnComplete(() => Destroy(button)); // 動畫完成後銷毀按鈕
        }

        // 等待飛出動畫完成
        yield return new WaitForSeconds(0.5f);

        // 隱藏面板
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
