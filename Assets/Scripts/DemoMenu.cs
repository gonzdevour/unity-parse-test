using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Demo_Menu : MonoBehaviour
{
    void Start()
    {
        Debug.Log("�}�lŪ���Ϥ�");
        StartCoroutine(TestLoadImg("Img_FromSA", "StreamingAssets://Image/duck.png"));
    }

    private IEnumerator TestLoadImg(string gameObjectName, string url)
    {
        //yield return StreamingAssets.Inst.LoadImg("Image/duck.png", resultTexture =>
        //{
        //    Debug.Log($"LoadImg from StreamingAsset complete");
        //    StreamingAssets.Inst.UpdateImageTexture("Img_FromSA", resultTexture);
        //});
        yield return null; //���ݳ�Ҫ�l�Ƨ���
        //Debug.Log("�}�lGetSprite");
        var sc = SpriteCacher.Inst;
        if (sc != null)
        {
            //Debug.Log("sc�s�b");
        }
        else
        {
            //Debug.Log("sc���s�b"); 
        }
        SpriteCacher.Inst.GetSprite(url, (sprite) =>
        {
            // �M�� GameObject
            //Debug.Log("���oGameObject:" + gameObjectName);
            GameObject imgFromSA = GameObject.Find(gameObjectName);
            if (imgFromSA != null)
            {
                Debug.Log($"{gameObjectName}�Ϥ��w��");
                Image image = imgFromSA.GetComponent<Image>();
                image.sprite = sprite;
            }
        });
    }
}
