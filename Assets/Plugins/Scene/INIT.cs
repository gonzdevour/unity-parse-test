using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class INIT : MonoBehaviour
{
    public bool CleanStorageAtStart;
    void Awake()
    {
        if (CleanStorageAtStart)
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
