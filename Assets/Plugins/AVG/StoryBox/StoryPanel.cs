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
        var gbjLayerChar = AVG.Inst.LayerChar.gameObject; // 取得角色圖層
        if (AVG.Inst.CGMode)
        {
            if (gbjLayerChar.activeSelf) gbjLayerChar.SetActive(false);//隱藏角色
            if (storyBubble.activeSelf) storyBubble.SetActive(false);//隱藏bubble
            if (storyBox.activeSelf) storyBox.SetActive(false);//隱藏box
            if (!storyCG.activeSelf) storyCG.SetActive(true);//顯示CG面板
            string effectName = AVG.Inst.isDifferentSayer ? "" : "";
            storyDisplayCG.Serif(Content, effectName, AVG.Inst.OnTypingComplete);
        }
        else // 不是CGMode
        {
            if (storyCG.activeSelf) storyCG.SetActive(false);//隱藏CG面板
            if (charData != null) //有角色資料
            {
                bool HasChar = charData["立繪"].ToLower() == "y";
                bool HasPortrait = charData["頭圖"].ToLower() == "y";

                if (AVG.Inst.DisplayChar)
                {
                    Director.Inst.CharsUnfocusAll(); //其他角色變黑
                    if (!gbjLayerChar.activeSelf) gbjLayerChar.SetActive(true);//顯示角色
                    if (HasChar)
                    {
                        IChar Char = Director.Inst.CharIn(charData, charUID, charPos, charEmo, charSimbol); //角色進場
                        if (AVG.Inst.DisplayBubble && !AVG.Inst.ChoiceMode)
                        {
                            //顯示bubble
                            if (!storyBubble.activeSelf) storyBubble.SetActive(true);

                            // 設定 bubble 的 X 軸與 char 一致
                            RectTransform charRect = Char.GetGameObject().GetComponent<RectTransform>();
                            RectTransform bbRect = storyBubble.GetComponent<RectTransform>();
                            Vector2 toPos = bbRect.anchoredPosition;
                            toPos.x = charRect.anchoredPosition.x; 
                            bbRect.anchoredPosition = toPos;
                            CanvasUI.Inst.ClampToBounds(bbRect, 40f); // 強制bubble在canvas內

                            // bubble特效
                            bubbleJump.Kill();
                            bubbleJump = BubbleJumpStart(bbRect, 40, 0.3f);

                            // 設定bubble的內容
                            string effectName = AVG.Inst.isDifferentSayer ? "" : "";
                            storyDisplayBubble.SetTone(charTone);
                            storyDisplayBubble.AimX(charRect.anchoredPosition.x - bbRect.anchoredPosition.x);
                            storyDisplayBubble.Name(DisplayName, effectName);
                            storyDisplayBubble.Serif(Content, effectName, AVG.Inst.OnTypingComplete);
                        }
                        else // !DisplayBubble
                        {
                            if (storyBubble.activeSelf) storyBubble.SetActive(false);//隱藏bubble
                        }
                    }
                    else // !HasChar
                    {
                        if (storyBubble.activeSelf) storyBubble.SetActive(false);//隱藏bubble
                    }
                }
                else // !DisplayChar
                {
                    if (gbjLayerChar.activeSelf) gbjLayerChar.SetActive(false);//隱藏角色
                }
                if (AVG.Inst.DisplayPortrait && HasPortrait) //有頭圖且允許頭圖，才顯示頭圖
                {
                    if (!portrait.gameObject.activeSelf) portrait.gameObject.SetActive(true);//顯示頭圖
                    Director.Inst.PortraitIn(charUID, charEmo);
                }
                else
                {
                    if (portrait.gameObject.activeSelf) portrait.gameObject.SetActive(false);//隱藏頭圖
                }
            }
            else //無角色資料，旁白或主角發言
            {
                Director.Inst.CharsUnfocusAll(); //其他角色變黑
                if (portrait.gameObject.activeSelf) portrait.gameObject.SetActive(false);//隱藏頭圖
                if (storyBubble.activeSelf) storyBubble.SetActive(false);//隱藏bubble
            }
            if (AVG.Inst.DisplayStoryBox)
            {                
                if (AVG.Inst.DisplayBubble && !AVG.Inst.ChoiceMode) // bubble與box共存時，box不隱藏但是內容清空
                {
                    if (!storyBox.activeSelf) storyBox.SetActive(true);//顯示box
                    if (portrait.gameObject.activeSelf) portrait.gameObject.SetActive(false);//隱藏頭圖
                    storyDisplayBox.Clear();
                } 
                else
                {
                    if (!storyBox.activeSelf) storyBox.SetActive(true);//顯示box
                    string effectName = AVG.Inst.isDifferentSayer ? "" : "";
                    storyDisplayBox.Name(DisplayName, effectName);
                    storyDisplayBox.Serif(Content, effectName, AVG.Inst.OnTypingComplete);
                }
            }
            else // !DisplayStoryBox
            {
                if (storyBox.activeSelf) storyBox.SetActive(false);//隱藏box
            }
        }
    }

    public Tween BubbleJumpStart(RectTransform bubble, float jumpHeight = 50f, float duration = 0.5f)
    {
        // 取得 UI 當前位置
        float originalY = bubble.anchoredPosition.y;

        // 使用 Sequence 組合上升與下降
        Sequence jumpSequence = DOTween.Sequence();
        jumpSequence
            .Append(bubble.DOAnchorPosY(originalY + jumpHeight, duration / 2).SetEase(Ease.OutQuad)) // 向上
            .Append(bubble.DOAnchorPosY(originalY, duration / 2).SetEase(Ease.InQuad)); // 落下

        return jumpSequence;
    }
}
