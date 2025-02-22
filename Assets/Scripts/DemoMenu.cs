using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Demo_Menu : MonoBehaviour
{
    void Start()
    {
        Debug.Log("�}�lŪ���Ϥ�");
        StartCoroutine(TestLoadImg("Img_FromSA", "StreamingAssets://Mods/Official/Images/Misc/duck.png"));
    }

    private IEnumerator TestLoadImg(string gameObjectName, string url)
    {
        yield return null; //���ݳ�Ҫ�l�Ƨ���
        SpriteCacher.Inst.GetSprite(url, (sprite) =>
        {
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
