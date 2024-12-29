using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using DG.Tweening;

public class AVG : MonoBehaviour
{
    public static AVG Inst { get; private set; }
    private void Awake()
    {
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public GameObject choicePanel; // ��ܭ��O
    public GameObject choicePrefab; // �ﶵ���sPrefab
    public Button Btn_Next; //�U�@�B���O(���s)
    public List<string> PendingStoryTitles;
    private List<Dictionary<string, object>> StoryEventDicts = new();
    SQLiteManager dbManager;

    private void Start()
    {
        dbManager = new SQLiteManager(Path.Combine(Application.persistentDataPath, "dynamicDatabase.db"));
        // ����choicePanel
        choicePanel.SetActive(false);
        // �T�O���s�i�ΨòK�[��ť
        if (Btn_Next != null)
        {
            Btn_Next.onClick.AddListener(OnProcessButtonClicked);
        }
    }

    private bool isReadyToNext = false;
    private bool isTyping = false;
    private bool isWaiting = false;
    private bool isChoiceSelected = true;
    private bool isStoryEnd = false;
    private int gotoIndex = -1; // �ﶵ��ܫ�N�e����cutIndex

    private void OnProcessButtonClicked()
    {
        CheckIfReadyToNext();
    }

    private void CheckIfReadyToNext()
    {
        if (!isTyping && !isWaiting && isChoiceSelected)
        {
            isReadyToNext = true;
        }
    }

    public IEnumerator StoryQueueStart<T>(Action onComplete) where T : class, new()
    {
        yield return null;
        while (PendingStoryTitles.Count > 0)
        {
            // ���X�ò����Ĥ@�Ӥ���
            string currentTitle = PendingStoryTitles[0];
            PendingStoryTitles.RemoveAt(0);
            
            isStoryEnd = false;
            Debug.Log($"�G�ƶ}�l: {currentTitle}");
            yield return StoryStart<T>(currentTitle);
            Debug.Log($"�G�ƤwŪ: {currentTitle}");
        }

        Debug.Log("All stories processed.");
        onComplete?.Invoke(); // ����^��
    }

    public IEnumerator StoryStart<T>(string StoryTitle) where T : class, new()
    {
        // �N�x��List�নJSON dict List�A���ȸ��F��
        StoryEventDicts.Clear();//JSON dict List���M�ŦA�`�J

        List<T> StoryEvents = dbManager.QueryTable<T>(StoryTitle);
        foreach (var item in StoryEvents)
        {
            var json = JsonConvert.SerializeObject(item);
            var storyCutDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            StoryEventDicts.Add(storyCutDict);
        }
        var startIndex = 0;
        var endIndex = StoryEvents.Count;
        StartCoroutine(StoryCutStart(startIndex));
        yield return new WaitUntil(() => isStoryEnd); // ���ݪ���o�ӬG�Ƶ���
    }

    public IEnumerator StoryCutStart(int cutIndex)
    {
        int nextCutIndex = cutIndex;//���H�N���

        // ���ݪ��a�ާ@
        isReadyToNext = false;

        Debug.Log($"StoryCutStart: {cutIndex}");
        var storyCutDict = StoryEventDicts[cutIndex];
        // ���满�ܫe��ƶ�
        if (HasValidValue(storyCutDict, "���ܫe"))
        {
           
        }
        // ��ܷ�ecut�����e
        string Name = TxR.Inst.Render(storyCutDict["���ܪ�"].ToString());
        string DisplayName = Name;
        if (HasValidValue(storyCutDict, "��ܦW��"))
        {
            DisplayName = TxR.Inst.Render(storyCutDict["��ܦW��"].ToString());
        }
        string Content = TxR.Inst.Render(storyCutDict["���ܤ��e"].ToString());
        Debug.Log($"{DisplayName}�G{Content}");

        //Debug.Log($"=> �e�����ȡG{storyCutDict["�e��"]}");
        if (HasValidValue(storyCutDict, "�e��"))
        {
            string[] targets = storyCutDict["�e��"].ToString().Split('\n');
            if (HasValidValue(storyCutDict, "�ﶵ"))
            {
                // �οﶵ��callback�^��index
                yield return StartChoose(storyCutDict);
                nextCutIndex = gotoIndex;
                CheckIfReadyToNext(); //�j��P�_�O�_�i�H����U�@�B
            }
            else
            {
                nextCutIndex = int.Parse(targets[0]);
                //Debug.Log($"���e�����O�S���ﶵ�A���w�e���Ĥ@��index{nextCutIndex}");
            }
        }
        else
        {
            nextCutIndex = cutIndex + 1;
            //Debug.Log($"�e���L�ȡA����+1: �e��{nextCutIndex}");
        }

        yield return new WaitUntil(() => isReadyToNext);
        if (nextCutIndex >= StoryEventDicts.Count)
        {
            //Debug.Log("cutIndex�W�X�d��A�G�Ƶ���");
            isStoryEnd = true;
        }
        else
        {
            StartCoroutine(StoryCutStart(nextCutIndex));
        }
    }

    IEnumerator StartChoose(Dictionary<string, object> storyCutDict)
    {
        // ��l�ƭ��O
        choicePanel.SetActive(true);
        ClearExistingButtons();

        // ��l�ƿ�ܪ��A
        isChoiceSelected = false;

        string[] targets = storyCutDict["�e��"].ToString().Split('\n');
        string[] options = storyCutDict["�ﶵ"].ToString().Split('\n');
        List<GameObject> buttons = new List<GameObject>();

        for (int i = 0; i < options.Length && i < targets.Length; i++)
        {
            // �Ыث��s
            GameObject button = Instantiate(choicePrefab, choicePanel.transform);
            button.GetComponentInChildren<Text>().text = options[i]; // �]�m���s��r
            int resultCutIndex = int.Parse(targets[i]);

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
        choicePanel.SetActive(false);
    }

    private void ClearExistingButtons()
    {
        foreach (Transform child in choicePanel.transform)
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

    private bool HasValidValue(Dictionary<string, object> dict, string key)
    {
        return dict.ContainsKey(key) && dict[key] != null && !string.IsNullOrWhiteSpace(dict[key].ToString());
    }
}
