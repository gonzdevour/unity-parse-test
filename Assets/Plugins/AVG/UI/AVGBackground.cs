using UnityEngine;

public class AVGBackground : MonoBehaviour
{
    public TransitionImage Background;

    public void GoTo(string key, string effectType = "fade", float duration = 2f)
    {
        var imagePath = Director.Inst.GetBackgroundImgUrl(key);
        Background.StartTransition(imagePath, effectType, duration);
    }

    public void Clear()
    {
        Background.Clear();
    }
}
