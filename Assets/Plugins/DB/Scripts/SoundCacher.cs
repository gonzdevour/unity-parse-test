using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class SoundCacher : MonoBehaviour
{
    // Sprite 緩存字典，每個地址對應一個已加載的 Sprite
    private Dictionary<string, AudioClip> audioClipCache = new();

    // 當前正在加載的地址集合，防止重複加載
    private HashSet<string> LoadingAudioClips = new HashSet<string>();

    // 地址對應的回調列表，用於支持多個回調請求
    private Dictionary<string, List<Action<AudioClip>>> CallbackMap = new();

    private readonly string corsanywhereUrl = "https://playoneapps.com.tw/corsanywhere/";

    // 單例模式
    public static SoundCacher Inst { get; private set; }

    private void Awake()
    {
        // 確保單例唯一性
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject); // 場景切換時保持不銷毀
    }

    public void GetAllAudioClipsInSA(Action onAllDone, string folderName = null)
    {
        var Mp3PathsInSA = StreamingAssets.GetAllAssetPaths(folderName: folderName, fileExt: ".mp3"); //一定要加.
        var WavPathsInSA = StreamingAssets.GetAllAssetPaths(folderName: folderName, fileExt: ".wav"); //一定要加.
        var OggPathsInSA = StreamingAssets.GetAllAssetPaths(folderName: folderName, fileExt: ".ogg"); //一定要加.
        var combinedList = Mp3PathsInSA .Concat(WavPathsInSA) .Concat(OggPathsInSA) .ToList(); // 合併為單一列表

        // 防呆：如果沒有任何地址，就直接呼叫 onAllDone
        if (combinedList == null || combinedList.Count == 0)
        {
            onAllDone?.Invoke();
            return;
        }

        int totalRequests = combinedList.Count;
        int completedCount = 0;

        // 並行地一次送出所有請求
        foreach (string path in combinedList)
        {
            GetAudioClip(path, (audioClip) =>
            {
                // 單個 request 完成的時候，把計數器加 1
                completedCount++;

                // 若全部都完成了，就呼叫 onAllDone
                if (completedCount == totalRequests)
                {
                    onAllDone?.Invoke();
                }
            });
        }
    }

    public void PreloadBatch(Dictionary<string, string> UrlDict)
    {
        foreach (var url in UrlDict)
        {
            GetAudioClip(url.Value);
        }
    }

    /// <summary>
    /// 獲取指定地址的 AudioClip，如果尚未加載則啟動加載流程
    /// </summary>
    /// <param name="address">資源地址</param>
    /// <param name="onComplete">加載完成時的回調，如不傳入回調則只進行Cache</param>
    public void GetAudioClip(string address, Action<AudioClip> onComplete = null)
    {
        //Debug.Log($"呼叫GetSprite取得{address}");
        // 如果地址正在加載，將回調加入隊列
        if (LoadingAudioClips.Contains(address))
        {
            Debug.Log($"{address}在隊列中");
            if (CallbackMap.TryGetValue(address, out var callbacks))
            {
                callbacks.Add(onComplete);
            }
            else
            {
                Debug.Log($"新增{address}到隊列");
                CallbackMap[address] = new List<Action<AudioClip>> { onComplete };
            }
            Debug.Log($"AudioClip is still loading: {address}");
            return;
        }

        // 如果資源已經在緩存中，直接執行回調
        if (audioClipCache.TryGetValue(address, out AudioClip cachedAudioClip))
        {
            Debug.Log($"{address}已經在緩存中");
            onComplete?.Invoke(cachedAudioClip);
            return;
        }

        // 開始新的加載過程
        LoadingAudioClips.Add(address);
        CallbackMap[address] = new List<Action<AudioClip>> { onComplete };
        //Debug.Log($"{path}在路上了");
        // 根據地址類型進行加載
        if (IsUrl(address))
        {
            StartCoroutine(DownloadAndCacheAudioClip(address));
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
            LoadingAudioClips.Remove(address);
            CallbackMap.Remove(address);
            onComplete?.Invoke(null);
        }
    }

    // 從 Resources 資源加載 Sprite
    private void CacheFromResources(string resourceAddress, string originalAddress)
    {
        Debug.Log($"開始從Resources讀取 {resourceAddress}");

        AudioClip audioClip;
        resourceAddress = resourceAddress.Split('.')[0];
        Debug.Log($"從 Resources 加載 audioClip，路徑: {resourceAddress}");
        audioClip = Resources.Load<AudioClip>(resourceAddress);

        if (audioClip == null)
        {
            Debug.LogWarning($"未找到路徑為 {resourceAddress} 的audioClip！");
        }

        // 回調處理結果
        OnAudioClipLoaded(originalAddress, audioClip);
    }


    // 從 StreamingAssets 資源加載 Sprite
    private IEnumerator CacheFromStreamingAssets(string filePath, string address)
    {
        //Debug.Log($"開始從sa讀取{filePath}");
        var sa = StreamingAssets.Inst;
        var audioType = GetAudioTypeFromAddress(address);
        yield return sa.LoadAudioClip(filePath, audioType, (audioClip) => 
        {
            OnAudioClipLoaded(address, audioClip);
        });
    }

    // 從 URL 下載並緩存 Sprite
    private IEnumerator DownloadAndCacheAudioClip(string url)
    {
        Debug.Log($"SoundCacher開始下載{url}");
        //Debug.Log($"corsanywhereUrl:{corsanywhereUrl + url}");
        var audioType = GetAudioTypeFromAddress(url);
        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(corsanywhereUrl + url, audioType))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"音訊下載失敗：{url}, 錯誤訊息：{request.error}");
                OnAudioClipLoaded(url, null);
            }
            else
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(request);
                Debug.Log($"AudioCacher 成功下載 AudioClip，從: {url}");
                OnAudioClipLoaded(url, audioClip);
            }
        }
    }

    // 資源加載完成後執行的通用回調處理
    private void OnAudioClipLoaded(string address, AudioClip audioClip)
    {
        if (audioClip != null)
        {
            audioClipCache[address] = audioClip;
            //Debug.Log($"Sprite cached: {address}");
        }

        if (CallbackMap.TryGetValue(address, out var callbacks))
        {
            foreach (var callback in callbacks)
            {
                callback?.Invoke(audioClip);
            }
            CallbackMap.Remove(address);
            //Debug.Log($"Removed callback from CallbackMap: {address}");
        }

        LoadingAudioClips.Remove(address);
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

    /// <summary>
    /// 根據檔案路徑或檔名自動判斷對應的 AudioType
    /// </summary>
    /// <param name="address">音效檔案的完整路徑或檔名</param>
    /// <returns>對應的 AudioType，若無法判斷則為 AudioType.UNKNOWN</returns>
    public static AudioType GetAudioTypeFromAddress(string address)
    {
        string ext = Path.GetExtension(address).ToLower();

        return ext switch
        {
            ".mp3" => AudioType.MPEG,
            ".wav" => AudioType.WAV,
            ".ogg" => AudioType.OGGVORBIS,
            _ => AudioType.UNKNOWN
        };
    }
}
