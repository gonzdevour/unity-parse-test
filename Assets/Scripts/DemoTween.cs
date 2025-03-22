using System.Collections;
using UnityEngine;

public class Demo_Tween : MonoBehaviour
{
    public GameObject tweenerPrefab;
    public Transform tweenerLayer;
    private string resPathSound = "StreamingAssets://Mods/Official/Sounds/";

    public void Generate()
    {
        Instantiate(tweenerPrefab, tweenerLayer);
        Dialog.Inst.Y("最多可支援12個全形字");
        //Sound.Inst.PlayBGM("Mods/Official/Sounds/basanova");
        Sound.Inst.PlayBGM(resPathSound + "basanova.mp3");
    }

    private void Start()
    {
        //StartCoroutine(Demo());
    }

    private IEnumerator Demo()
    {
        yield return null;
        Dialog.Inst.Y("最多可支援12個全形字");
    }
}
