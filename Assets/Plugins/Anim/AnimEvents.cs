using System;
using UnityEngine;

public class AnimEvents : MonoBehaviour
{
    public event Action OnAnimationStartCallback;
    public event Action OnAnimationEndCallback;

    public void OnAnimationStart()
    {
        //Debug.Log("Animation started.");
        OnAnimationStartCallback?.Invoke();
    }

    public void OnAnimationEnd()
    {
        //Debug.Log("Animation ended.");
        OnAnimationEndCallback?.Invoke();
    }

    void OnDestroy()
    {
        OnAnimationStartCallback = null;
        OnAnimationEndCallback = null;
    }
}
