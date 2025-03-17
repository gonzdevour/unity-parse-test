using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class StoryPanel : MonoBehaviour,IStoryPlayer
{
    public AVGPortrait portrait;
    public StoryDisplayCG storyDisplayCG;
    public StoryDisplayBubble storyDisplayBubble;
    public StoryDisplayBox storyDisplayBox;

    private GameObject storyBox;
    private GameObject storyCG;
    private GameObject storyBubble;

    private Tween bubbleJump;

    private void Awake()
    {
        storyBox = storyDisplayBox.gameObject;
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
        if (portrait != null) portrait.GoTo(key);
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
        var gbjLayerChar = AVG.Inst.LayerChar.gameObject; // ���o����ϼh
        if (AVG.Inst.CGMode)
        {
            if (gbjLayerChar.activeSelf) gbjLayerChar.SetActive(false);//���è���
            if (storyBubble.activeSelf) storyBubble.SetActive(false);//����bubble
            if (storyBox.activeSelf) storyBox.SetActive(false);//����box
            if (!storyCG.activeSelf) storyCG.SetActive(true);//���CG���O
            string effectName = AVG.Inst.isDifferentSayer ? "" : "";
            storyDisplayCG.Serif(Content, effectName, AVG.Inst.OnTypingComplete);
        }
        else // ���OCGMode
        {
            if (storyCG.activeSelf) storyCG.SetActive(false);//����CG���O
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
                        if (AVG.Inst.DisplayBubble && !AVG.Inst.ChoiceMode)
                        {
                            //���bubble
                            if (!storyBubble.activeSelf) storyBubble.SetActive(true);

                            // �]�w bubble �� X �b�P char �@�P
                            RectTransform charRect = Char.GetGameObject().GetComponent<RectTransform>();
                            RectTransform bbRect = storyBubble.GetComponent<RectTransform>();
                            Vector2 toPos = bbRect.anchoredPosition;
                            toPos.x = charRect.anchoredPosition.x; 
                            bbRect.anchoredPosition = toPos;
                            CanvasUI.Inst.ClampToBounds(bbRect, 40f); // �j��bubble�bcanvas��

                            // bubble�S��
                            bubbleJump.Kill();
                            bubbleJump = BubbleJumpStart(bbRect, 40, 0.3f);

                            // �]�wbubble�����e
                            string effectName = AVG.Inst.isDifferentSayer ? "" : "";
                            storyDisplayBubble.SetTone(charTone);
                            storyDisplayBubble.AimX(charRect.anchoredPosition.x - bbRect.anchoredPosition.x);
                            storyDisplayBubble.Name(DisplayName, effectName);
                            storyDisplayBubble.Serif(Content, effectName, AVG.Inst.OnTypingComplete);
                        }
                        else // !DisplayBubble
                        {
                            if (storyBubble.activeSelf) storyBubble.SetActive(false);//����bubble
                        }
                    }
                    else // !HasChar
                    {
                        if (storyBubble.activeSelf) storyBubble.SetActive(false);//����bubble
                    }
                }
                else // !DisplayChar
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
                if (AVG.Inst.DisplayBubble && !AVG.Inst.ChoiceMode) // bubble�Pbox�@�s�ɡAbox�����æ��O���e�M��
                {
                    if (!storyBox.activeSelf) storyBox.SetActive(true);//���box
                    if (portrait.gameObject.activeSelf) portrait.gameObject.SetActive(false);//�����Y��
                    storyDisplayBox.Clear();
                } 
                else
                {
                    if (!storyBox.activeSelf) storyBox.SetActive(true);//���box
                    string effectName = AVG.Inst.isDifferentSayer ? "" : "";
                    storyDisplayBox.Name(DisplayName, effectName);
                    storyDisplayBox.Serif(Content, effectName, AVG.Inst.OnTypingComplete);
                }
            }
            else // !DisplayStoryBox
            {
                if (storyBox.activeSelf) storyBox.SetActive(false);//����box
            }
        }
    }

    public Tween BubbleJumpStart(RectTransform bubble, float jumpHeight = 50f, float duration = 0.5f)
    {
        // ���o UI ��e��m
        float originalY = bubble.anchoredPosition.y;

        // �ϥ� Sequence �զX�W�ɻP�U��
        Sequence jumpSequence = DOTween.Sequence();
        jumpSequence
            .Append(bubble.DOAnchorPosY(originalY + jumpHeight, duration / 2).SetEase(Ease.OutQuad)) // �V�W
            .Append(bubble.DOAnchorPosY(originalY, duration / 2).SetEase(Ease.InQuad)); // ���U

        return jumpSequence;
    }
}
