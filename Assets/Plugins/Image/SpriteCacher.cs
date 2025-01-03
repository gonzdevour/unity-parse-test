using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class SpriteCacher : MonoBehaviour
{
    // Sprite 緩存字典，每個地址對應一個已加載的 Sprite
    private Dictionary<string, Sprite> SpriteCache = new Dictionary<string, Sprite>();

    // 當前正在加載的地址集合，防止重複加載
    private HashSet<string> LoadingSprites = new HashSet<string>();

    // 地址對應的回調列表，用於支持多個回調請求
    private Dictionary<string, List<Action<Sprite>>> CallbackMap = new Dictionary<string, List<Action<Sprite>>>();

    private readonly string corsanywhereUrl = "https://playoneapps.com.tw/corsanywhere/";

    // 單例模式
    public static SpriteCacher Inst { get; private set; }

    private void Awake()
    {
        // 確保單例唯一性
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject); // 場景切換時保持不銷毀
    }

    /// <summary>
    /// 獲取指定地址的 Sprite，如果尚未加載則啟動加載流程
    /// </summary>
    /// <param name="address">資源地址</param>
    /// <param name="onComplete">加載完成時的回調</param>
    public void GetSprite(string address, Action<Sprite> onComplete)
    {
        Debug.Log($"呼叫GetSprite取得{address}");
        // 如果地址正在加載，將回調加入隊列
        if (LoadingSprites.Contains(address))
        {
            Debug.Log($"{address}在隊列中");
            if (CallbackMap.TryGetValue(address, out var callbacks))
            {
                callbacks.Add(onComplete);
            }
            else
            {
                Debug.Log($"新增{address}到隊列");
                CallbackMap[address] = new List<Action<Sprite>> { onComplete };
            }
            Debug.Log($"Sprite is still loading: {address}");
            return;
        }

        // 如果資源已經在緩存中，直接執行回調
        if (SpriteCache.TryGetValue(address, out Sprite cachedSprite))
        {
            Debug.Log($"{address}已經在緩存中");
            onComplete?.Invoke(cachedSprite);
            return;
        }

        // 開始新的加載過程
        LoadingSprites.Add(address);
        CallbackMap[address] = new List<Action<Sprite>> { onComplete };
        //Debug.Log($"{address}在路上了");
        // 根據地址類型進行加載
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

    // 從 Resources 資源加載 Sprite
    private void CacheFromResources(string resourceAddress, string originalAddress)
    {
        Debug.Log($"開始從Resources讀取 {resourceAddress}");

        Sprite sprite;

        if (resourceAddress.Contains("|"))
        {
            // 若包含 "|"，採用 multiple sprite 的讀取方式
            string[] resourceParts = resourceAddress.Split('|');
            if (resourceParts.Length < 2)
            {
                Debug.LogError("resourceAddress 格式錯誤，應包含 '|' 分隔的路徑和名稱！");
                OnSpriteLoaded(originalAddress, null);
                return;
            }

            string resourcePath = resourceParts[0];
            string spriteName = resourceParts[1];

            Debug.Log($"從 Resources 加載多張 Sprite，路徑: {resourcePath}，名稱: {spriteName}");
            Sprite[] sprites = Resources.LoadAll<Sprite>(resourcePath);

            sprite = sprites.FirstOrDefault(s => s.name == spriteName);
            if (sprite == null)
            {
                Debug.LogWarning($"未找到名稱為 {spriteName} 的 Sprite！");
            }
        }
        else
        {
            // 若不包含 "|"，採用單一圖片的讀取方式
            resourceAddress = resourceAddress.Split('.')[0];
            Debug.Log($"從 Resources 加載單張 Sprite，路徑: {resourceAddress}");
            sprite = Resources.Load<Sprite>(resourceAddress);

            if (sprite == null)
            {
                Debug.LogWarning($"未找到路徑為 {resourceAddress} 的單張 Sprite！");
            }
        }

        // 回調處理結果
        OnSpriteLoaded(originalAddress, sprite);
    }


    // 從 StreamingAssets 資源加載 Sprite
    private IEnumerator CacheFromStreamingAssets(string filePath, string address)
    {
        Debug.Log($"開始從sa讀取{filePath}");
        var sa = StreamingAssets.Inst;
        yield return sa.LoadImg(filePath, (texture) => 
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            OnSpriteLoaded(address, sprite);
        });
    }

    // 從 URL 下載並緩存 Sprite
    private IEnumerator DownloadAndCacheSprite(string url)
    {
        Debug.Log($"SpriteCacher開始下載{url}");
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
                Debug.Log($"SpriteCacher成功下載Texture2D，從:{url}");
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                texture.wrapMode = TextureWrapMode.Clamp;

                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                OnSpriteLoaded(url, sprite);
            }
        }
    }

    // 資源加載完成後執行的通用回調處理
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
            return address.Substring(prefix.Length);
        }
        return "";
    }
}
