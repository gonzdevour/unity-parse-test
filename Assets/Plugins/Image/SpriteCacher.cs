using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class SpriteCacher : MonoBehaviour
{
    private Dictionary<string, Sprite> SpriteCache = new Dictionary<string, Sprite>(); // 暫存 Sprite
    private HashSet<string> LoadingSprites = new HashSet<string>(); // 跟蹤當前正在加載的地址

    public static SpriteCacher Inst { get; private set; }
    private void Awake()
    {
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void GetSprite(string address, System.Action<Sprite> onComplete)
    {
        // 如果地址已經在加載中，直接返回
        //if (LoadingSprites.Contains(address))
        //{
        //    Debug.Log($"Sprite is still loading: {address}");
        //    return;
        //}

        // 如果已經存在於緩存中，直接回調返回
        if (SpriteCache.TryGetValue(address, out Sprite cachedSprite))
        {
            onComplete?.Invoke(cachedSprite);
            return;
        }

        // 標記地址為加載中
        LoadingSprites.Add(address);

        // 根據地址類型進行不同的加載操作
        if (IsUrl(address))
        {
            // 如果是網絡地址，使用協程進行下載並緩存
            StartCoroutine(DownloadAndCacheSprite(address, onComplete));
        }
        else if (address.StartsWith("Resources://"))
        {
            // 如果是 Resources 資源，調用對應加載方法
            CacheFromResources(TrimAddress(address, "Resources://"), address, onComplete);
        }
        else if (address.StartsWith("StreamingAssets://"))
        {
            // 如果是 StreamingAssets 資源，調用對應加載方法
            CacheFromStreamingAssets(TrimAddress(address, "StreamingAssets://"), address, onComplete);
        }
        else
        {
            // 如果地址無效，打印警告並返回
            Debug.LogWarning($"Invalid address: {address}");
            LoadingSprites.Remove(address);
            onComplete?.Invoke(null);
        }
    }

    // 從 Resources 資源加載 Sprite
    private void CacheFromResources(string resourceAddress, string originalAddress, System.Action<Sprite> onComplete)
    {
        // 嘗試從資源中加載所有 Sprite
        Sprite[] sprites = Resources.LoadAll<Sprite>(resourceAddress.Split('|')[0]);
        // 根據名稱匹配指定的 Sprite
        Sprite sprite = sprites.FirstOrDefault(s => s.name == resourceAddress.Split('|').Last());

        if (sprite == null)
        {
            // 如果未找到，打印警告並回調返回 null
            Debug.LogWarning($"Sprite not found in Resources: {originalAddress}");
            LoadingSprites.Remove(originalAddress);
            onComplete?.Invoke(null);
            return;
        }

        // 將加載的 Sprite 添加到緩存中
        SpriteCache[originalAddress] = sprite;
        LoadingSprites.Remove(originalAddress);
        Debug.Log($"Sprite cached from Resources: {originalAddress}");
        onComplete?.Invoke(sprite);
    }

    // 從 StreamingAssets 資源加載 Sprite
    private void CacheFromStreamingAssets(string filePath, string address, System.Action<Sprite> onComplete)
    {
        // 拼接完整的文件路徑
        string fullPath = Path.Combine(Application.streamingAssetsPath, filePath);
        if (!File.Exists(fullPath))
        {
            // 如果文件不存在，打印警告並回調返回 null
            Debug.LogWarning($"File not found in StreamingAssets: {filePath}");
            LoadingSprites.Remove(address);
            onComplete?.Invoke(null);
            return;
        }

        // 使用協程加載 Sprite
        StartCoroutine(LoadSpriteFromStreamingAssets(fullPath, address, sprite =>
        {
            LoadingSprites.Remove(address);
            onComplete?.Invoke(sprite);
        }));
    }

    // 從 URL 下載並緩存 Sprite
    private IEnumerator DownloadAndCacheSprite(string url, System.Action<Sprite> onComplete)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                // 如果下載失敗，打印錯誤並回調返回 null
                Debug.LogError($"Failed to download sprite from URL: {url}, Error: {request.error}");
                LoadingSprites.Remove(url);
                onComplete?.Invoke(null);
            }
            else
            {
                // 下載成功，將圖片轉換為 Texture2D
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                texture.wrapMode = TextureWrapMode.Clamp;

                // 將 Texture2D 轉換為 Sprite 並緩存
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                SpriteCache[url] = sprite;
                //Debug.Log($"Sprite cached from URL: {url}");
                LoadingSprites.Remove(url);
                onComplete?.Invoke(sprite);
            }
        }
    }

    // 從 StreamingAssets 加載 Sprite
    private IEnumerator LoadSpriteFromStreamingAssets(string filePath, string address, System.Action<Sprite> onComplete)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(filePath))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                // 加載成功，將 Texture2D 轉換為 Sprite
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                SpriteCache[address] = sprite;
                Debug.Log($"Sprite cached from StreamingAssets: {address}");
                onComplete?.Invoke(sprite);
            }
            else
            {
                // 加載失敗，打印警告並回調返回 null
                Debug.LogWarning($"Failed to load sprite from StreamingAssets: {uwr.error}");
                onComplete?.Invoke(null);
            }
        }
    }

    // 判斷是否為 URL 地址
    private bool IsUrl(string address)
    {
        return address.StartsWith("http://") || address.StartsWith("https://");
    }

    // 去除地址的指定前綴
    private string TrimAddress(string address, string prefix)
    {
        if (address.StartsWith(prefix))
        {
            return address.Substring(prefix.Length); // 移除前綴
        }
        return ""; // 不符合條件則返回空字串
    }
}
