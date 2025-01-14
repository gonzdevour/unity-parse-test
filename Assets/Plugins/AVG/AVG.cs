using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using DG.Tweening;

// �ޤJ����Story����Ƶ��c
using Story;
using System.Runtime.InteropServices;

public class AVG : MonoBehaviour
{
    public static AVG Inst { get; private set; }
    private void Awake()
    {
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public RectTransform MainPanel; //�D�R�x
    public GameObject TEffectPanel; // �H�J�H�X�S�ĭ��O
    public GameObject ChoicePanel; // ��ܭ��O
    public GameObject ChoicePrefab; // �ﶵ���sPrefab

    public AVGBackground Background; //�I��
    public AVGPortrait Portrait; //�Y��
    public RectTransform LayerChar; //����h
    public GameObject CharPrefab; // ����Prefab

    public Button Btn_Next; //�U�@�B���O(���s)
    public List<string> PendingStoryTitles;
    private List<Dictionary<string, object>> StoryEventDicts = new();
    SQLiteManager dbManager;

    private void Start()
    {
        dbManager = new SQLiteManager(Path.Combine(Application.persistentDataPath, "dynamicDatabase.db"));
        
        // ��l����Panel
        ChoicePanel.SetActive(false);
        MainPanel.gameObject.SetActive(false);

        // �T�O���e���U�@�B���s�i�ΨòK�[��ť
        if (Btn_Next != null)
        {
            Btn_Next.onClick.AddListener(OnProcessButtonClicked);
        }
    }

    public bool DisplayChar = true;
    public bool DisplayPortrait = true;
    public bool DisplayStoryBox = true;
    public bool DisplayBubble = false;
    public bool SingleCharMode = false;

    private bool isReadyToNext = false;
    private bool isTyping = false;
    private bool isWaiting = false;
    private bool isChoiceSelected = true;
    private bool isStoryEnd = false;
    private int gotoIndex = -1; // �ﶵ��ܫ�N�e����cutIndex
    public int nextCutIndex; //�U�@�d�����ޭȡA�i�H���~������

    public IEnumerator Init()
    {
        yield return null;
        UpdatePPM("Preset");//��s�w�]��
        Background.Init(GetBgData("Bgs"));
        Portrait.Init(GetCharDataAll("Chars"));
    }

    public void On()
    {
        MainPanel.gameObject.SetActive(true);
        TEffectPanel.SetActive(true);
    }

    public void Off()
    {
        Director.Inst.Off(); //�M��Director�ި����s�A�pBackground�PTEffect
        SetInactive(ChoicePanel);
        SetInactive(MainPanel.gameObject);
    }

    private void SetInactive(GameObject gameObject) 
    {
        if (gameObject != null)
        {
            gameObject.SetActive(false);
        }
    }

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

    public IEnumerator StoryQueueStart(Action onComplete)
    {
        FilterStories("StoryList");//�M���P�_�ثe�ŦX���󪺼@���A�N�@���W�٥[�JAVG player
        while (PendingStoryTitles.Count > 0)
        {
            // ���X�ò����Ĥ@�Ӥ���
            string currentTitle = PendingStoryTitles[0];
            PendingStoryTitles.RemoveAt(0);
            
            isStoryEnd = false;
            Debug.Log($"�G�ƶ}�l: {currentTitle}");
            yield return StoryStart<StoryCut>(currentTitle);
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
        nextCutIndex = cutIndex;//���H�N���

        // ���ݪ��a�ާ@
        isReadyToNext = false;

        //Debug.Log($"StoryCutStart: {cutIndex}");
        var storyCutDict = StoryEventDicts[cutIndex];

        // ��ܷ�ecut�����e
        string charUID = TxR.Inst.Render(ParseEx(storyCutDict["���ܪ�"].ToString()));
        string charPos = TxR.Inst.Render(ParseEx(storyCutDict["��m"].ToString()));
        string charEmo = TxR.Inst.Render(ParseEx(storyCutDict["��"].ToString()));

        string DisplayName = charUID; //�o�̪�charUID�����O�ѪR�᪺���ܪ̦r��A��DisplayName���w�]��
        // ���w����
        Dictionary<string, string> charData = GetCharDataByUID("Chars", charUID);
        bool HasChar = charData["��ø"].ToLower() == "y";
        bool HasPortrait = charData["�Y��"].ToLower() == "y";

        var gbjLayerChar = LayerChar.gameObject;
        var gbjPortrait = Portrait.gameObject;
        if (charData != null) //��������
        {
            if (DisplayChar) 
            {
                Director.Inst.CharsUnfocusAll(); //��L�����ܶ�
                if (!gbjLayerChar.activeSelf) gbjLayerChar.SetActive(true);//��ܨ���
                if (HasChar) Director.Inst.CharIn(charData, charUID, charPos, charEmo); //����i��
            }
            else
            {
                if (gbjLayerChar.activeSelf) gbjLayerChar.SetActive(false);//���è���
            }
            if (DisplayPortrait && HasPortrait) //���Y�ϥB���\�Y�ϡA�~����Y��
            {
                if(!gbjPortrait.activeSelf) gbjPortrait.SetActive(true);//����Y��
                Director.Inst.PortraitIn(charUID,charEmo);
            }
            else
            {
                if (gbjPortrait.activeSelf) gbjPortrait.SetActive(false);//�����Y��
            }
            DisplayName = $"{charData["�m"]}{charData["�W"]}";
        }
        else //�L�����ơA�ǥթΥD���o��
        {
            if (gbjPortrait.activeSelf) gbjPortrait.SetActive(false);//�����Y��
        }
        // ��ܦW��
        if (HasValidValue(storyCutDict, "��ܦW��"))
        {
            DisplayName = TxR.Inst.Render(ParseEx(storyCutDict["��ܦW��"].ToString()));
        }
        // ���满�ܫe��ƶ�
        if (HasValidValue(storyCutDict, "���ܫe"))
        {
            string[] commands = storyCutDict["���ܫe"].ToString().Split('\n');
            for (int i = 0; i < commands.Length; i++)
            {
                commands[i] = ParseEx(commands[i]);
            }
            Director.Inst.ExecuteActionPackage(commands);
        }
        // ����
        string Content = TxR.Inst.Render(ParseEx(storyCutDict["���ܤ��e"].ToString()));
        Debug.Log($"Cut{cutIndex} - {DisplayName}�G{Content}");

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
                nextCutIndex = int.Parse(ParseEx(targets[0]));
                //Debug.Log($"���e�����O�S���ﶵ�A���w�e���Ĥ@��index{nextCutIndex}");
            }
        }
        else
        {
            nextCutIndex = cutIndex + 1;
            //Debug.Log($"�e���L�ȡA����+1: �e��{nextCutIndex}");
        }

        //���ܫ᪺��ƥi�H���ܫe����nextCutIndex
        if (HasValidValue(storyCutDict, "���ܫ�"))
        {
            string[] commands = storyCutDict["���ܫ�"].ToString().Split('\n');
            for (int i = 0; i < commands.Length; i++)
            {
                commands[i] = ParseEx(commands[i]);
            }
            Director.Inst.ExecuteActionPackage(commands);
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

    private bool HasValidValue(Dictionary<string, object> dict, string key)
    {
        return dict.ContainsKey(key) && dict[key] != null && !string.IsNullOrWhiteSpace(dict[key].ToString());
    }

    public Dictionary<string, string> GetCharDataByUID(string pageName, string UID)
    {
        // ��l�Ʊ���
        string condition = $"UID = '{UID}'";

        // �I�s QueryTable ���
        List<CharData> results = dbManager.QueryTable<CharData>(pageName, condition);

        CharData charData = null;
        // ����Ĥ@���d�ߵ��G
        if (results.Count > 0)
        {
            charData = results[0];
            //Debug.Log($"����ơG{bgData.�W} ({bgData.UID})");
        }
        else
        {
            //Debug.Log("�d�ߵL���G");
        }

        var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(charData));

        return dict;
    }

    public List<Dictionary<string, string>> GetCharDataAll(string pageName)
    {
        // �I�s QueryTable ���
        List<CharData> results = dbManager.QueryTable<CharData>(pageName);
        var dictList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(
                JsonConvert.SerializeObject(results)
            );

        return dictList;
    }

    public List<Dictionary<string, string>> GetBgData(string pageName)
    {
        // �I�s QueryTable ���
        List<BgData> results = dbManager.QueryTable<BgData>(pageName);
        var dictList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(
                JsonConvert.SerializeObject(results)
            );

        return dictList;
    }

    public void UpdatePPM(string pageName)
    {
        // �b PPM ���]�m���զr��ƾڡA�P�ɤ䴩TxR
        List<Preset> allItems = dbManager.QueryTable<Preset>(pageName);
        foreach (var item in allItems)
        {
            //Debug.Log($"{item.Key}={item.Value}");
            PPM.Inst.Set(item.Key, item.Value);
        }
    }

    public void FilterStories(string pageName)
    {
        List<StoryList> allItems = dbManager.QueryTable<StoryList>(pageName);

        foreach (var item in allItems)
        {
            Debug.Log($"title:{item.Title}, cond:{item.Condition}, desc:{item.Description}");

            if (Judge.EvaluateCondition(item.Condition))
            {
                Debug.Log($"Condition met: {item.Title}");

                // �N�ŦX���� Title �K�[�� PendingStoryTitles
                AVG.Inst.PendingStoryTitles.Add(item.Title);
            }
        }
    }

    /// <summary>
    /// �ѪR����B��l�r��ê�^���G�C
    /// </summary>
    /// <param name="expression">����B��l�r��A�Ҧp "����>=�J�|����?3:8"</param>
    /// <returns>�ѪR�çP�_�᪺���G</returns>
    public static string ParseEx(string expression)
    {
        if (string.IsNullOrEmpty(expression))
        {
            //Debug.LogError("��F�����ũ� null");
            return string.Empty;
        }

        try
        {
            expression = expression.Trim();
            // ������B��l����m
            int questionMarkIndex = expression.IndexOf('?');
            int colonIndex = expression.IndexOf(':');

            // �ˬd�榡�O�_���T
            if (questionMarkIndex == -1 || colonIndex == -1 || questionMarkIndex > colonIndex)
            {
                //Debug.Log($"�D�T�����G{expression}�A�^�ǭȬ�{expression}");
                if (!expression.StartsWith("\"") || !expression.EndsWith("\""))
                {
                    return PPM.Inst.Get(expression, expression);
                }
                else
                {
                    return expression;
                }
            }

            // �ѪR����B���u���G�B�������G
            string condition = expression.Substring(0, questionMarkIndex).Trim();
            string trueResult = expression.Substring(questionMarkIndex + 1, colonIndex - questionMarkIndex - 1).Trim();
            string falseResult = expression.Substring(colonIndex + 1).Trim();

            // �ϥ� Judge.EvaluateCondition �P�_����O�_����
            bool conditionResult = Judge.EvaluateSingleCondition(condition);

            // �ھڱ��󵲪G��^��������
            string result = conditionResult ? trueResult : falseResult;
            Debug.Log($"�T�������G{expression}�A�^�ǭȬ�{result}");
            return result;
        }
        catch (Exception ex)
        {
            Debug.LogError($"�ѪR�ΧP�_��F���ɵo�Ϳ��~: {ex.Message}");
            return string.Empty;
        }
    }
}
