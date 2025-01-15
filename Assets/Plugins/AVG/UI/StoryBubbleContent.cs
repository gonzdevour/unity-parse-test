using UnityEngine;
using UnityEngine.UI;

public class StoryBubbleContent : MonoBehaviour
{
    public Text TxContent;

    public void Display(string text = "", bool playEffect = false)
    {
        Typing typer = GetComponent<Typing>();
        typer.StartTyping(text);

        if (playEffect) DoEffect();
    }

    public void DoEffect()
    {
    }
}
