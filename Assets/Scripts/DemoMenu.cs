using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Demo_Menu : MonoBehaviour
{
    private Dictionary<string, string> ImgPathsToPreload = new Dictionary<string, string>(); // 儲存圖片資源路徑

    void Start()
    {
        Debug.Log("開始讀取圖片");
        StartCoroutine(TestLoadImg("Img_FromSA", "StreamingAssets://Image/duck.png"));
        StartCoroutine(Preload());
    }

    IEnumerator Preload()
    {
        yield return null; //等待global scene
        //ImgPathsToPreload["街道"] = "Resources://Sprites/AVG/BG/Landscape/Daily/AChos001_19201080.jpg";
        //ImgPathsToPreload["店裡"] = "Resources://Sprites/AVG/BG/Landscape/Daily/130machi_19201080.jpg";
        //SpriteCacher.Inst.PreloadBatch(ImgPathsToPreload);
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
