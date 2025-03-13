using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class StoryDisplay : MonoBehaviour
{
    public Text TxName;
    public Typing SerifTyper;

    public virtual void Name(string text = "", string effectName = "")
    {
        TxName.text = text;
        if (!string.IsNullOrEmpty(effectName)) DoEffect(TxName.gameObject, effectName);
    }

    public virtual void Serif(string text = "", string effectName = "", Action cbk = null)
    {
        if (SerifTyper != null) SerifTyper.StartTyping(message: text, onComplete: cbk);
        if (!string.IsNullOrEmpty(effectName)) DoEffect(SerifTyper.gameObject, effectName);
    }

    public virtual void SkipTyping()
    {
        if (SerifTyper != null) SerifTyper.SkipTyping();
    }

    public virtual bool IsTyping()
    {
        return SerifTyper != null && SerifTyper.IsTyping();
    }

    public virtual void DoEffect(GameObject effectTarget, string effectName)
    {
        //¯S®Ä
    }
}
