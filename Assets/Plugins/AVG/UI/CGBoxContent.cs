using UnityEngine;
using UnityEngine.UI;

public class CGBoxContent : MonoBehaviour
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
