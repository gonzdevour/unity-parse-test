using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class SpriteCacher : MonoBehaviour
{
    // Sprite �w�s�r��A�C�Ӧa�}�����@�Ӥw�[���� Sprite
    private Dictionary<string, Sprite> SpriteCache = new Dictionary<string, Sprite>();

    // ��e���b�[�����a�}���X�A����ƥ[��
    private HashSet<string> LoadingSprites = new HashSet<string>();

    // �a�}�������^�զC��A�Ω����h�Ӧ^�սШD
    private Dictionary<string, List<Action<Sprite>>> CallbackMap = new Dictionary<string, List<Action<Sprite>>>();

    private readonly string corsanywhereUrl = "https://playoneapps.com.tw/corsanywhere/";

    // ��ҼҦ�
    public static SpriteCacher Inst { get; private set; }

    private void Awake()
    {
        // �T�O��Ұߤ@��
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject); // ���������ɫO�����P��
    }

    /// <summary>
    /// ������w�a�}�� Sprite�A�p�G�|���[���h�Ұʥ[���y�{
    /// </summary>
    /// <param name="address">�귽�a�}</param>
    /// <param name="onComplete">�[�������ɪ��^��</param>
    public void GetSprite(string address, Action<Sprite> onComplete)
    {
        Debug.Log($"�I�sGetSprite���o{address}");
        // �p�G�a�}���b�[���A�N�^�ե[�J���C
        if (LoadingSprites.Contains(address))
        {
            Debug.Log($"{address}�b���C��");
            if (CallbackMap.TryGetValue(address, out var callbacks))
            {
                callbacks.Add(onComplete);
            }
            else
            {
                Debug.Log($"�s�W{address}�춤�C");
                CallbackMap[address] = new List<Action<Sprite>> { onComplete };
            }
            Debug.Log($"Sprite is still loading: {address}");
            return;
        }

        // �p�G�귽�w�g�b�w�s���A��������^��
        if (SpriteCache.TryGetValue(address, out Sprite cachedSprite))
        {
            Debug.Log($"{address}�w�g�b�w�s��");
            onComplete?.Invoke(cachedSprite);
            return;
        }

        // �}�l�s���[���L�{
        LoadingSprites.Add(address);
        CallbackMap[address] = new List<Action<Sprite>> { onComplete };
        //Debug.Log($"{address}�b���W�F");
        // �ھڦa�}�����i��[��
        if (IsUrl(address))
        {
            StartCoroutine(DownloadAndCacheSprite(address));
        }
        else if (address.StartsWith("Resources://"))
        {
            CacheFromResources(TrimAddress(address, "Resources://"), address);
        }
        else if (address.StartsWith("StreamingAssets://"))
        {
            StartCoroutine(CacheFromStreamingAssets(TrimAddress(address, "StreamingAssets://"), address));
        }
        else
        {
            Debug.LogWarning($"Invalid address: {address}");
            LoadingSprites.Remove(address);
            CallbackMap.Remove(address);
            onComplete?.Invoke(null);
        }
    }

    // �q Resources �귽�[�� Sprite
    private void CacheFromResources(string resourceAddress, string originalAddress)
    {
        Debug.Log($"�}�l�qResourcesŪ�� {resourceAddress}");

        Sprite sprite;

        if (resourceAddress.Contains("|"))
        {
            // �Y�]�t "|"�A�ĥ� multiple sprite ��Ū���覡
            string[] resourceParts = resourceAddress.Split('|');
            if (resourceParts.Length < 2)
            {
                Debug.LogError("resourceAddress �榡���~�A���]�t '|' ���j�����|�M�W�١I");
                OnSpriteLoaded(originalAddress, null);
                return;
            }

            string resourcePath = resourceParts[0];
            string spriteName = resourceParts[1];

            Debug.Log($"�q Resources �[���h�i Sprite�A���|: {resourcePath}�A�W��: {spriteName}");
            Sprite[] sprites = Resources.LoadAll<Sprite>(resourcePath);

            sprite = sprites.FirstOrDefault(s => s.name == spriteName);
            if (sprite == null)
            {
                Debug.LogWarning($"�����W�٬� {spriteName} �� Sprite�I");
            }
        }
        else
        {
            // �Y���]�t "|"�A�ĥγ�@�Ϥ���Ū���覡
            resourceAddress = resourceAddress.Split('.')[0];
            Debug.Log($"�q Resources �[����i Sprite�A���|: {resourceAddress}");
            sprite = Resources.Load<Sprite>(resourceAddress);

            if (sprite == null)
            {
                Debug.LogWarning($"�������|�� {resourceAddress} ����i Sprite�I");
            }
        }

        // �^�ճB�z���G
        OnSpriteLoaded(originalAddress, sprite);
    }


    // �q StreamingAssets �귽�[�� Sprite
    private IEnumerator CacheFromStreamingAssets(string filePath, string address)
    {
        Debug.Log($"�}�l�qsaŪ��{filePath}");
        var sa = StreamingAssets.Inst;
        yield return sa.LoadImg(filePath, (texture) => 
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            OnSpriteLoaded(address, sprite);
        });
    }

    // �q URL �U���ýw�s Sprite
    private IEnumerator DownloadAndCacheSprite(string url)
    {
        Debug.Log($"SpriteCacher�}�l�U��{url}");
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(corsanywhereUrl + url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to download sprite from URL: {url}, Error: {request.error}");
                OnSpriteLoaded(url, null);
            }
            else
            {
                Debug.Log($"SpriteCacher���\�U��Texture2D�A�q:{url}");
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                texture.wrapMode = TextureWrapMode.Clamp;

                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                OnSpriteLoaded(url, sprite);
            }
        }
    }

    // �귽�[����������檺�q�Φ^�ճB�z
    private void OnSpriteLoaded(string address, Sprite sprite)
    {
        if (sprite != null)
        {
            SpriteCache[address] = sprite;
            Debug.Log($"Sprite cached: {address}");
        }

        if (CallbackMap.TryGetValue(address, out var callbacks))
        {
            foreach (var callback in callbacks)
            {
                callback?.Invoke(sprite);
            }
            CallbackMap.Remove(address);
        }

        LoadingSprites.Remove(address);
    }

    // �P�_�O�_�� URL �a�}
    private bool IsUrl(string address)
    {
        return address.StartsWith("http://") || address.StartsWith("https://");
    }

    // �h���a�}�����w�e��
    private string TrimAddress(string address, string prefix)
    {
        if (address.StartsWith(prefix))
        {
            return address.Substring(prefix.Length);
        }
        return "";
    }
}
