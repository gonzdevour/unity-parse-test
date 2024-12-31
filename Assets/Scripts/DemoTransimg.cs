using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Demo_Transimg : MonoBehaviour
{
    public ToggleSelector EffectSelector;
    public Button Button_Switch; // ���w�����s
    public TransitionImage TImg; // ���] TImg �O�@�ӭt�d�B�z�Ϥ��ഫ����

    private Dictionary<string, string> imagePaths = new Dictionary<string, string>(); // �x�s�Ϥ��귽���|
    private List<string> pathKeys; // �Ω�����Ǧs�����|���䶰�X
    private int currentIndex = 0; // ��e�Ϥ�����

    void Start()
    {
        // ��l�ƹϤ��귽���|
        imagePaths["Image1"] = "Assets/Resources/Sprites/AVG/BG/Landscape/Daily/AChos001_19201080.jpg";
        imagePaths["Image2"] = "Assets/Resources/Sprites/AVG/BG/Landscape/Daily/ACstreet001_19201080.jpg";
        imagePaths["Image3"] = "Assets/Resources/Sprites/AVG/BG/Landscape/Daily/rtshoppingstreet01_19201080.jpg";

        pathKeys = new List<string>(imagePaths.Keys);

        // �T�O���s�w���t�A�øj�w�I���ƥ�
        if (Button_Switch != null)
        {
            Button_Switch.onClick.AddListener(OnButtonSwitch);
        }
    }

    private void OnButtonSwitch()
    {
        if (TImg != null && pathKeys.Count > 0)
        {
            // ���o��e���ު��Ϥ����|
            string currentPath = imagePaths[pathKeys[currentIndex]];
            string effectType = EffectSelector.SelectedToggleNamesString;
            TImg.StartTransition(currentPath, effectType, 2f);

            // ��s���ޥH���V�U�@�i�Ϥ��A�ô`��
            currentIndex = (currentIndex + 1) % pathKeys.Count;
        }
    }
}
