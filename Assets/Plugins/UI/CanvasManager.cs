using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public GameObject cover;
    public GameObject pannelSpinner;
    public GameObject pannelLoadingProgress;
    // Start is called before the first frame update
    void Awake()
    {
        cover.SetActive(true);
        pannelSpinner.SetActive(false);
        pannelLoadingProgress.SetActive(false);
    }
}
