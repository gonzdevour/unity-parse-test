using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AVGConfig : MonoBehaviour
{
    public AVG Avg;

    [Header("�ﶵ����")]
    public Toggle toggleDisplayChar;
    public Toggle toggleDisplayPortrait;
    public Toggle toggleDisplayStoryBox;
    public Toggle toggleDisplayBubble;
    public Toggle toggleSingleCharMode;
    public Toggle toggleCGMode;

    [Header("�ƭȱ���")]
    public InputField inputDefaultBgTransDur;
    public InputField inputDefaultCharTransDur;
    public InputField inputDefaultCharFocusDur;
    public InputField inputDefaultCharUnfocusDur;
    public InputField inputDefaultCharMoveDur;
    public InputField inputDefaultTypingInterval;

    [Header("�Ʊ�����")]
    public Slider sliderMusicVolume;
    public Slider sliderSFXVolume;

    private void Start()
    {
        // Ū���s�x����
        LoadSettings();

        // ��l�� UI �����
        InitializeUI();

        // �]�m�ƥ��ť
        SetupListeners();
    }

    private void InitializeUI()
    {
        // �]�m Toggle ��l��
        toggleDisplayChar.isOn = AVG.Inst.DisplayChar;
        toggleDisplayPortrait.isOn = AVG.Inst.DisplayPortrait;
        toggleDisplayStoryBox.isOn = AVG.Inst.DisplayStoryBox;
        toggleDisplayBubble.isOn = AVG.Inst.DisplayBubble;
        toggleSingleCharMode.isOn = AVG.Inst.SingleCharMode;
        toggleCGMode.isOn = AVG.Inst.CGMode;

        // �]�m InputField ��l��
        inputDefaultBgTransDur.text = Director.Inst.DefaultBgTransDur.ToString();
        inputDefaultCharTransDur.text = Director.Inst.DefaultCharTransDur.ToString();
        inputDefaultCharFocusDur.text = Director.Inst.DefaultCharFocusDur.ToString();
        inputDefaultCharUnfocusDur.text = Director.Inst.DefaultCharUnfocusDur.ToString();
        inputDefaultCharMoveDur.text = Director.Inst.DefaultCharMoveDur.ToString();
        inputDefaultTypingInterval.text = Director.Inst.DefaultTypingInterval.ToString();

        // �]�m Slider ��l��
        sliderMusicVolume.value = Director.Inst.MusicVolume;
        sliderSFXVolume.value = Director.Inst.SEVolume;
    }

    private void SetupListeners()
    {
        // Toggle ��ť�ƥ�æs�x�� PlayerPrefs
        toggleDisplayChar.onValueChanged.AddListener(value =>
        {
            AVG.Inst.DisplayChar = value;
            PlayerPrefs.SetInt("DisplayChar", value ? 1 : 0);
        });

        toggleDisplayPortrait.onValueChanged.AddListener(value =>
        {
            AVG.Inst.DisplayPortrait = value;
            PlayerPrefs.SetInt("DisplayPortrait", value ? 1 : 0);
        });

        toggleDisplayStoryBox.onValueChanged.AddListener(value =>
        {
            AVG.Inst.DisplayStoryBox = value;
            PlayerPrefs.SetInt("DisplayStoryBox", value ? 1 : 0);
        });

        toggleDisplayBubble.onValueChanged.AddListener(value =>
        {
            AVG.Inst.DisplayBubble = value;
            PlayerPrefs.SetInt("DisplayBubble", value ? 1 : 0);
        });

        toggleSingleCharMode.onValueChanged.AddListener(value =>
        {
            AVG.Inst.SingleCharMode = value;
            PlayerPrefs.SetInt("SingleCharMode", value ? 1 : 0);
        });

        toggleCGMode.onValueChanged.AddListener(value =>
        {
            AVG.Inst.CGMode = value;
            PlayerPrefs.SetInt("CGMode", value ? 1 : 0);
        });

        // InputField ��ť�ƥ�æs�x�� PlayerPrefs
        inputDefaultBgTransDur.onEndEdit.AddListener(value =>
        {
            Director.Inst.DefaultBgTransDur = ParseFloat(value, Director.Inst.DefaultBgTransDur);
            PlayerPrefs.SetFloat("DefaultBgTransDur", Director.Inst.DefaultBgTransDur);
        });

        inputDefaultCharTransDur.onEndEdit.AddListener(value =>
        {
            Director.Inst.DefaultCharTransDur = ParseFloat(value, Director.Inst.DefaultCharTransDur);
            PlayerPrefs.SetFloat("DefaultCharTransDur", Director.Inst.DefaultCharTransDur);
        });

        inputDefaultCharFocusDur.onEndEdit.AddListener(value =>
        {
            Director.Inst.DefaultCharFocusDur = ParseFloat(value, Director.Inst.DefaultCharFocusDur);
            PlayerPrefs.SetFloat("DefaultCharFocusDur", Director.Inst.DefaultCharFocusDur);
        });

        inputDefaultCharUnfocusDur.onEndEdit.AddListener(value =>
        {
            Director.Inst.DefaultCharUnfocusDur = ParseFloat(value, Director.Inst.DefaultCharUnfocusDur);
            PlayerPrefs.SetFloat("DefaultCharUnfocusDur", Director.Inst.DefaultCharUnfocusDur);
        });

        inputDefaultCharMoveDur.onEndEdit.AddListener(value =>
        {
            Director.Inst.DefaultCharMoveDur = ParseFloat(value, Director.Inst.DefaultCharMoveDur);
            PlayerPrefs.SetFloat("DefaultCharMoveDur", Director.Inst.DefaultCharMoveDur);
        });

        inputDefaultTypingInterval.onEndEdit.AddListener(value =>
        {
            Director.Inst.DefaultTypingInterval = ParseFloat(value, Director.Inst.DefaultTypingInterval);
            PlayerPrefs.SetFloat("DefaultTypingInterval", Director.Inst.DefaultTypingInterval);
        });

        // Slider ��ť�ƥ�æs�x�� PlayerPrefs
        sliderMusicVolume.onValueChanged.AddListener(value =>
        {
            Director.Inst.MusicVolume = value;
            PlayerPrefs.SetFloat("MusicVolume", value);
        });

        sliderSFXVolume.onValueChanged.AddListener(value =>
        {
            Director.Inst.SEVolume = value;
            PlayerPrefs.SetFloat("SEVolume", value);
        });

        // �T�O�ƾ��x�s
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        // Ū�� PlayerPrefs
        AVG.Inst.DisplayChar = PlayerPrefs.GetInt("DisplayChar", 1) == 1;
        AVG.Inst.DisplayPortrait = PlayerPrefs.GetInt("DisplayPortrait", 1) == 1;
        AVG.Inst.DisplayStoryBox = PlayerPrefs.GetInt("DisplayStoryBox", 1) == 1;
        AVG.Inst.DisplayBubble = PlayerPrefs.GetInt("DisplayBubble", 0) == 1;
        AVG.Inst.SingleCharMode = PlayerPrefs.GetInt("SingleCharMode", 0) == 1;
        AVG.Inst.CGMode = PlayerPrefs.GetInt("CGMode", 0) == 1;

        Director.Inst.DefaultBgTransDur = PlayerPrefs.GetFloat("DefaultBgTransDur", 1f);
        Director.Inst.DefaultCharTransDur = PlayerPrefs.GetFloat("DefaultCharTransDur", 1f);
        Director.Inst.DefaultCharFocusDur = PlayerPrefs.GetFloat("DefaultCharFocusDur", 0.5f);
        Director.Inst.DefaultCharUnfocusDur = PlayerPrefs.GetFloat("DefaultCharUnfocusDur", 0f);
        Director.Inst.DefaultCharMoveDur = PlayerPrefs.GetFloat("DefaultCharMoveDur", 0f);
        Director.Inst.DefaultTypingInterval = PlayerPrefs.GetFloat("DefaultTypingInterval", 0.05f);

        Director.Inst.MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        Director.Inst.SEVolume = PlayerPrefs.GetFloat("SEVolume", 1f);
    }

    private float ParseFloat(string value, float defaultValue)
    {
        return float.TryParse(value, out float result) ? result : defaultValue;
    }
}
