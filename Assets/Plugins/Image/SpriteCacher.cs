using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Collections;
using System.Collections.Generic;

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
        if (IsUrl(address)) // �q�����[��
        {
            if (!LoadingSprites.Contains(address))
            {
                LoadingSprites.Add(address);
                StartCoroutine(DownloadAndCacheSprite(address, onComplete));
            }
        }
        else // �qResources��StreamingAssets�[��
        {
            string addressFromResources = TrimAddress(address, "Resources://");
            string addressFromStreamingAssets = TrimAddress(address, "StreamingAssets://");

            // �qResources�[��
            if (!string.IsNullOrEmpty(addressFromResources))
            {
                if (IsMultipleSprite(addressFromResources)) //�p�G�O�����Ϥ�
                {
                    // �ϥ� '|' �r�����Φr��
                    string[] parts = addressFromResources.Split('|');
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
                            //Debug.LogWarning($"Sprite not found in Resources: {address}");
                            onComplete?.Invoke(null);
                        }
                    }
                    else
                    {
                        Sprite sprite = Resources.Load<Sprite>(addressFromResources);
                        if (sprite != null)
                        {
                            SpriteCache[address] = sprite;
                            Debug.Log($"Sprite cached from Resources: {address}");
                            onComplete?.Invoke(sprite);
                        }
                        else
                        {
                            //Debug.LogWarning($"Sprite not found in Resources: {address}");
                            onComplete?.Invoke(null);
                        }
                    }
                } else
                {
                    //�p�G���O�����Ϥ�
                    Sprite sprite = Resources.Load<Sprite>(addressFromResources);
                    if (sprite != null)
                    {
                        SpriteCache[address] = sprite;
                        Debug.Log($"Sprite cached from Resources: {address}");
                        onComplete?.Invoke(sprite);
                    }
                    else
                    {
                        //Debug.LogWarning($"Sprite not found in Resources: {address}");
                        onComplete?.Invoke(null);
                    }
                }
            }
            // �qStreamingAssets�[��
            else if (!string.IsNullOrEmpty(addressFromStreamingAssets))
            {
                if (!LoadingSprites.Contains(address))
                {
                    LoadingSprites.Add(address);

                    string filePath = Path.Combine(Application.streamingAssetsPath, addressFromStreamingAssets);
                    if (File.Exists(filePath))
                    {
                        StartCoroutine(LoadSpriteFromStreamingAssets(filePath, address, sprite =>
                        {
                            LoadingSprites.Remove(address);
                            onComplete?.Invoke(sprite);
                        }));
                    }
                    else
                    {
                        //Debug.LogWarning($"File not found in StreamingAssets: {filePath}");
                        LoadingSprites.Remove(address);
                        onComplete?.Invoke(null);
                    }
                }
            }
        }
    }

    private bool IsUrl(string address)
    {
        return address.StartsWith("http://") || address.StartsWith("https://");
    }

    private string TrimAddress(string address, string prefix)
    {
        if (address.StartsWith(prefix))
        {
            return address.Substring(prefix.Length); // �����e��
        }
        return ""; // ���ŦX����h��^�Ŧr��
    }

    private bool IsMultipleSprite(string address)
    {
        return address.Contains("|");
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

    // filePath�O���ɸ��|�Aaddress�O��l�ۭq���|�Ҧp"StreamingAssets://filePath"�Aaddress�OSpriteCache��key
    private IEnumerator LoadSpriteFromStreamingAssets(string filePath, string address, System.Action<Sprite> onComplete)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(filePath))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                SpriteCache[address] = sprite;
                Debug.Log($"Sprite cached from StreamingAssets: {address}");
                onComplete?.Invoke(sprite);
            }
            else
            {
                Debug.LogWarning($"Failed to load sprite from StreamingAssets: {uwr.error}");
                onComplete?.Invoke(null);
            }
        }
    }
}
