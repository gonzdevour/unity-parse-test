using System;
using UnityEngine;
using UnityEngine.UI;

public class StoryBubbleContent : MonoBehaviour
{
    public Text TxContent;
    private Typing typer;

    public void Awake()
    {
        typer = GetComponent<Typing>();
    }

    public void Display(string text = "", bool playEffect = false, Action cbk = null)
    {
        if (typer != null) typer.StartTyping(message: text, onComplete: cbk);
        if (playEffect) DoEffect();
    }

    public void SkipTyping()
    {
        if (typer != null) typer.SkipTyping();
    }

    public bool IsTyping()
    {
        bool result = false;
        if (typer != null) result = typer.IsTyping();
        return result;
    }

    public void DoEffect()
    {
    }
}
