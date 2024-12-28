using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AVG : MonoBehaviour
{
    public List<string> PendingStoryTitles;
    public static AVG Inst { get; private set; }
    private void Awake()
    {
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator StoryStart() 
    {
        yield return null;
    }
}
