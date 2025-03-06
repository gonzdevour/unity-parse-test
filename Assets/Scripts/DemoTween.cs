using UnityEngine;

public class Demo_Tween : MonoBehaviour
{
    public GameObject tweenerPrefab;
    public Transform tweenerLayer;
    
    public void Generate()
    {
        Instantiate(tweenerPrefab, tweenerLayer);
    }
}
