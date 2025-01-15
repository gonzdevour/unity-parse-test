using UnityEngine;
using UnityEngine.UI;

public class StoryBubbleName : MonoBehaviour
{
    public Text TxName;

    public void Display(string text = "", bool playEffect = false)
    {
        TxName.text = text;
        if (playEffect) DoEffect();
    }

    public void DoEffect()
    {
    }
}
