using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class SpriteCacher : MonoBehaviour
{
    private Dictionary<string, Sprite> SpriteCache = new Dictionary<string, Sprite>(); // 暫存 Sprite
    private HashSet<string> LoadingSprites = new HashSet<string>(); // 跟蹤當前正在加載的地址

    public void GetSprite(string address, System.Action<Sprite> onComplete)
    {
        if (SpriteCache.TryGetValue(address, out Sprite sprite))
        {
            onComplete?.Invoke(sprite);
        }
        else
        {
            if (!LoadingSprites.Contains(address))
            {
                CacheSprite(address, onComplete);
            }
            else
            {
                Debug.Log($"Sprite is still loading: {address}");
            }
        }
    }

    public void CacheSprite(string address, System.Action<Sprite> onComplete)
    {
        if (IsUrl(address))
        {
            if (!LoadingSprites.Contains(address))
            {
                LoadingSprites.Add(address);
                StartCoroutine(DownloadAndCacheSprite(address, onComplete));
            }
        }
        else
        {
            // 若 address 不是網址，從 Resources 加載
            // 使用 '|' 字元分割字串
            string[] parts = address.Split('|');
            string path;
            string spriteName;
            if (parts.Length == 2)
            {
                path = parts[0];
                spriteName = parts[1];
                // 指定sprite所在的素材路徑
                Sprite[] sprites = Resources.LoadAll<Sprite>(path);
                // 在載入的陣列中尋找名為 adress 的Sprite
                Sprite sprite = null;
                foreach (var s in sprites)
                {
                    if (s.name == spriteName)
                    {
                        sprite = s;
                        break;
                    }
                }
                if (sprite != null)
                {
                    SpriteCache[address] = sprite;
                    Debug.Log($"Sprite cached from Resources: {address}");
                    onComplete?.Invoke(sprite);
                }
                else
                {
                    Debug.LogWarning($"Sprite not found in Resources: {address}");
                    onComplete?.Invoke(null);
                }
            }
            else
            {
                // 若沒有'|'，或分割結果不是兩段，根據需求決定要做什麼處理
                Debug.LogWarning("Address format is not as expected.");
            }
        }
    }

    private bool IsUrl(string address)
    {
        return address.StartsWith("http://") || address.StartsWith("https://");
    }

    private IEnumerator DownloadAndCacheSprite(string url, System.Action<Sprite> onComplete)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to download sprite from URL: {url}, Error: {request.error}");
                onComplete?.Invoke(null);
            }
            else
            {
                // 將下載的圖片轉換為 Texture2D
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                texture.wrapMode = TextureWrapMode.Clamp;

                // 生成 Sprite 並暫存
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                SpriteCache[url] = sprite;
                Debug.Log($"Sprite cached from URL: {url}");
                onComplete?.Invoke(sprite);
            }
        }
        LoadingSprites.Remove(url); // 加載完成後移除跟蹤
    }
}
