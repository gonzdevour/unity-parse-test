using System.Collections.Generic;
using UnityEngine;

public class  StoryBox : MonoBehaviour,IStoryPlayer
{
    public AVGPortrait portrait;
    public StoryDisplayCG storyDisplayCG;
    public StoryDisplayBubble storyDisplayBubble;
    public StoryDisplayBox storyDisplayBox;

    private GameObject storyCG;
    private GameObject storyBubble;

    private void Awake()
    {
        storyCG = storyDisplayCG.gameObject;
        storyBubble = storyDisplayBubble.gameObject;
    }

    public bool IsTyping()
    {
        bool cgIsTyping = storyDisplayCG.IsTyping();
        bool bbIsTyping = storyDisplayBubble.IsTyping();
        bool bxIsTyping = storyDisplayBox.IsTyping();
        if ( bxIsTyping || bbIsTyping || cgIsTyping)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SkipTyping()
    {
        bool cgIsTyping = storyDisplayCG.IsTyping();
        bool bbIsTyping = storyDisplayBubble.IsTyping();
        bool bxIsTyping = storyDisplayBox.IsTyping();
        if (cgIsTyping) storyDisplayCG.SkipTyping();
        if (bbIsTyping) storyDisplayBubble.SkipTyping();
        if (bxIsTyping) storyDisplayBox.SkipTyping();
    }

    public void ClearPortrait()
    {
        if (portrait != null) portrait.Clear();
    }

    public void PortraitGoTo(string key)
    {
        portrait.GoTo(key);
    }

    public void Display(
        Dictionary<string, string> charData,
        string charUID,
        string charPos,
        string charEmo,
        string charSimbol,
        string charTone,
        string charEffect,
        string DisplayName,
        string Content
        )
    {
        if (AVG.Inst.CGMode)
        {
            if (!storyCG.activeSelf) storyCG.SetActive(true);//���CG���O
            string effectName = AVG.Inst.isDifferentSayer ? "" : "";
            storyDisplayCG.Serif(Content, effectName, AVG.Inst.OnTypingComplete);
        }
        else
        {
            if (storyCG.activeSelf) storyCG.SetActive(false);//����CG���O
            var gbjLayerChar = AVG.Inst.LayerChar.gameObject;
            if (charData != null) //��������
            {
                bool HasChar = charData["��ø"].ToLower() == "y";
                bool HasPortrait = charData["�Y��"].ToLower() == "y";

                if (AVG.Inst.DisplayChar)
                {
                    Director.Inst.CharsUnfocusAll(); //��L�����ܶ�
                    if (!gbjLayerChar.activeSelf) gbjLayerChar.SetActive(true);//��ܨ���
                    if (HasChar)
                    {
                        IChar Char = Director.Inst.CharIn(charData, charUID, charPos, charEmo, charSimbol); //����i��
                        if (AVG.Inst.DisplayBubble)
                        {
                            //���bubble
                            if (!storyBubble.activeSelf) storyBubble.SetActive(true);

                            // �]�w bubble �� X �b�P char �@�P
                            RectTransform charRect = Char.GetGameObject().GetComponent<RectTransform>();
                            RectTransform bbRect = storyBubble.GetComponent<RectTransform>();
                            Vector2 toPos = bbRect.anchoredPosition;
                            toPos.x = charRect.anchoredPosition.x; 
                            bbRect.anchoredPosition = toPos;
                            CanvasUI.Inst.ClampToBounds(bbRect); // �j��bubble�bcanvas��

                            // �]�wbubble�����e
                            string effectName = AVG.Inst.isDifferentSayer ? "" : "";
                            storyDisplayBubble.Name(DisplayName, effectName);
                            storyDisplayBubble.Serif(Content, effectName, AVG.Inst.OnTypingComplete);
                        }
                        else
                        {
                            if (storyBubble.activeSelf) storyBubble.SetActive(false);//����bubble
                        }
                    }
                    else
                    {
                        if (storyBubble.activeSelf) storyBubble.SetActive(false);//����bubble
                    }
                }
                else
                {
                    if (gbjLayerChar.activeSelf) gbjLayerChar.SetActive(false);//���è���
                }
                if (AVG.Inst.DisplayPortrait && HasPortrait) //���Y�ϥB���\�Y�ϡA�~����Y��
                {
                    if (!portrait.gameObject.activeSelf) portrait.gameObject.SetActive(true);//����Y��
                    Director.Inst.PortraitIn(charUID, charEmo);
                }
                else
                {
                    if (portrait.gameObject.activeSelf) portrait.gameObject.SetActive(false);//�����Y��
                }
            }
            else //�L�����ơA�ǥթΥD���o��
            {
                Director.Inst.CharsUnfocusAll(); //��L�����ܶ�
                if (portrait.gameObject.activeSelf) portrait.gameObject.SetActive(false);//�����Y��
                if (storyBubble.activeSelf) storyBubble.SetActive(false);//����bubble
            }
            if (AVG.Inst.DisplayStoryBox)
            {
                if (! gameObject.activeSelf)  gameObject.SetActive(true);//���box
                string effectName = AVG.Inst.isDifferentSayer ? "" : "";
                storyDisplayBox.Name(DisplayName, effectName);
                storyDisplayBox.Serif(Content, effectName, AVG.Inst.OnTypingComplete);
            }
            else
            {
                if ( gameObject.activeSelf)  gameObject.SetActive(false);//����box
            }
        }
    }
}
