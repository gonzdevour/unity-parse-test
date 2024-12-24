using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class SpriteCacher : MonoBehaviour
{
    private Dictionary<string, Sprite> SpriteCache = new Dictionary<string, Sprite>(); // �Ȧs Sprite
    private HashSet<string> LoadingSprites = new HashSet<string>(); // ���ܷ�e���b�[�����a�}

    public static SpriteCacher Inst { get; private set; }
    private void Awake()
    {
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void GetSprite(string address, System.Action<Sprite> onComplete)
    {
        // �p�G�a�}�w�g�b�[�����A������^
        //if (LoadingSprites.Contains(address))
        //{
        //    Debug.Log($"Sprite is still loading: {address}");
        //    return;
        //}

        // �p�G�w�g�s�b��w�s���A�����^�ժ�^
        if (SpriteCache.TryGetValue(address, out Sprite cachedSprite))
        {
            onComplete?.Invoke(cachedSprite);
            return;
        }

        // �аO�a�}���[����
        LoadingSprites.Add(address);

        // �ھڦa�}�����i�椣�P���[���ާ@
        if (IsUrl(address))
        {
            // �p�G�O�����a�}�A�ϥΨ�{�i��U���ýw�s
            StartCoroutine(DownloadAndCacheSprite(address, onComplete));
        }
        else if (address.StartsWith("Resources://"))
        {
            // �p�G�O Resources �귽�A�եι����[����k
            CacheFromResources(TrimAddress(address, "Resources://"), address, onComplete);
        }
        else if (address.StartsWith("StreamingAssets://"))
        {
            // �p�G�O StreamingAssets �귽�A�եι����[����k
            CacheFromStreamingAssets(TrimAddress(address, "StreamingAssets://"), address, onComplete);
        }
        else
        {
            // �p�G�a�}�L�ġA���Lĵ�i�ê�^
            Debug.LogWarning($"Invalid address: {address}");
            LoadingSprites.Remove(address);
            onComplete?.Invoke(null);
        }
    }

    // �q Resources �귽�[�� Sprite
    private void CacheFromResources(string resourceAddress, string originalAddress, System.Action<Sprite> onComplete)
    {
        // ���ձq�귽���[���Ҧ� Sprite
        Sprite[] sprites = Resources.LoadAll<Sprite>(resourceAddress.Split('|')[0]);
        // �ھڦW�٤ǰt���w�� Sprite
        Sprite sprite = sprites.FirstOrDefault(s => s.name == resourceAddress.Split('|').Last());

        if (sprite == null)
        {
            // �p�G�����A���Lĵ�i�æ^�ժ�^ null
            Debug.LogWarning($"Sprite not found in Resources: {originalAddress}");
            LoadingSprites.Remove(originalAddress);
            onComplete?.Invoke(null);
            return;
        }

        // �N�[���� Sprite �K�[��w�s��
        SpriteCache[originalAddress] = sprite;
        LoadingSprites.Remove(originalAddress);
        Debug.Log($"Sprite cached from Resources: {originalAddress}");
        onComplete?.Invoke(sprite);
    }

    // �q StreamingAssets �귽�[�� Sprite
    private void CacheFromStreamingAssets(string filePath, string address, System.Action<Sprite> onComplete)
    {
        // �������㪺�����|
        string fullPath = Path.Combine(Application.streamingAssetsPath, filePath);
        if (!File.Exists(fullPath))
        {
            // �p�G��󤣦s�b�A���Lĵ�i�æ^�ժ�^ null
            Debug.LogWarning($"File not found in StreamingAssets: {filePath}");
            LoadingSprites.Remove(address);
            onComplete?.Invoke(null);
            return;
        }

        // �ϥΨ�{�[�� Sprite
        StartCoroutine(LoadSpriteFromStreamingAssets(fullPath, address, sprite =>
        {
            LoadingSprites.Remove(address);
            onComplete?.Invoke(sprite);
        }));
    }

    // �q URL �U���ýw�s Sprite
    private IEnumerator DownloadAndCacheSprite(string url, System.Action<Sprite> onComplete)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                // �p�G�U�����ѡA���L���~�æ^�ժ�^ null
                Debug.LogError($"Failed to download sprite from URL: {url}, Error: {request.error}");
                LoadingSprites.Remove(url);
                onComplete?.Invoke(null);
            }
            else
            {
                // �U�����\�A�N�Ϥ��ഫ�� Texture2D
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                texture.wrapMode = TextureWrapMode.Clamp;

                // �N Texture2D �ഫ�� Sprite �ýw�s
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                SpriteCache[url] = sprite;
                //Debug.Log($"Sprite cached from URL: {url}");
                LoadingSprites.Remove(url);
                onComplete?.Invoke(sprite);
            }
        }
    }

    // �q StreamingAssets �[�� Sprite
    private IEnumerator LoadSpriteFromStreamingAssets(string filePath, string address, System.Action<Sprite> onComplete)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(filePath))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                // �[�����\�A�N Texture2D �ഫ�� Sprite
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                SpriteCache[address] = sprite;
                Debug.Log($"Sprite cached from StreamingAssets: {address}");
                onComplete?.Invoke(sprite);
            }
            else
            {
                // �[�����ѡA���Lĵ�i�æ^�ժ�^ null
                Debug.LogWarning($"Failed to load sprite from StreamingAssets: {uwr.error}");
                onComplete?.Invoke(null);
            }
        }
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
            return address.Substring(prefix.Length); // �����e��
        }
        return ""; // ���ŦX����h��^�Ŧr��
    }
}
