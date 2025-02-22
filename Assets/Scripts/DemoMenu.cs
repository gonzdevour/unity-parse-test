using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Demo_Menu : MonoBehaviour
{
    void Start()
    {
        Debug.Log("開始讀取圖片");
        StartCoroutine(TestLoadImg("Img_FromSA", "StreamingAssets://Mods/Official/Images/Misc/duck.png"));
    }

    private IEnumerator TestLoadImg(string gameObjectName, string url)
    {
        yield return null; //等待單例初始化完成
        SpriteCacher.Inst.GetSprite(url, (sprite) =>
        {
            //Debug.Log("取得GameObject:" + gameObjectName);
            GameObject imgFromSA = GameObject.Find(gameObjectName);
            if (imgFromSA != null)
            {
                Debug.Log($"{gameObjectName}圖片已更換");
                Image image = imgFromSA.GetComponent<Image>();
                image.sprite = sprite;
            }
        });
    }
}
