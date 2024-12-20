using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class SpriteCacher : MonoBehaviour
{
    private Dictionary<string, Sprite> SpriteCache = new Dictionary<string, Sprite>(); // �Ȧs Sprite
    private HashSet<string> LoadingSprites = new HashSet<string>(); // ���ܷ�e���b�[�����a�}

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
            // �Y address ���O���}�A�q Resources �[��
            // �ϥ� '|' �r�����Φr��
            string[] parts = address.Split('|');
            string path;
            string spriteName;
            if (parts.Length == 2)
            {
                path = parts[0];
                spriteName = parts[1];
                // ���wsprite�Ҧb���������|
                Sprite[] sprites = Resources.LoadAll<Sprite>(path);
                // �b���J���}�C���M��W�� adress ��Sprite
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
                // �Y�S��'|'�A�Τ��ε��G���O��q�A�ھڻݨD�M�w�n������B�z
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
                // �N�U�����Ϥ��ഫ�� Texture2D
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                texture.wrapMode = TextureWrapMode.Clamp;

                // �ͦ� Sprite �üȦs
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                SpriteCache[url] = sprite;
                Debug.Log($"Sprite cached from URL: {url}");
                onComplete?.Invoke(sprite);
            }
        }
        LoadingSprites.Remove(url); // �[�������Ჾ������
    }
}
