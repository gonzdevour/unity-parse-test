using UnityEngine;

[System.Serializable]
public class ToggleVariables:MonoBehaviour
{
    public string LabelName;
    public Color LabelColor;

    public void SetLabelName(string name) 
    {
        LabelName = name;
    }
}
