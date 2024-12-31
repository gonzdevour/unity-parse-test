using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Demo_Menu : MonoBehaviour
{
    void Start()
    {
        Debug.Log("開始讀取圖片");
        StartCoroutine(TestLoadImg("Img_FromSA", "StreamingAssets://Image/duck.png"));
    }

    private IEnumerator TestLoadImg(string gameObjectName, string url)
    {
        //yield return StreamingAssets.Inst.LoadImg("Image/duck.png", resultTexture =>
        //{
        //    Debug.Log($"LoadImg from StreamingAsset complete");
        //    StreamingAssets.Inst.UpdateImageTexture("Img_FromSA", resultTexture);
        //});
        yield return null; //等待單例初始化完成
        //Debug.Log("開始GetSprite");
        var sc = SpriteCacher.Inst;
        if (sc != null)
        {
            //Debug.Log("sc存在");
        }
        else
        {
            //Debug.Log("sc不存在"); 
        }
        SpriteCacher.Inst.GetSprite(url, (sprite) =>
        {
            // 尋找 GameObject
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
