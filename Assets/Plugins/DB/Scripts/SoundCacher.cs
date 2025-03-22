using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class SoundCacher : MonoBehaviour
{
    // Sprite �w�s�r��A�C�Ӧa�}�����@�Ӥw�[���� Sprite
    private Dictionary<string, AudioClip> audioClipCache = new();

    // ��e���b�[�����a�}���X�A����ƥ[��
    private HashSet<string> LoadingAudioClips = new HashSet<string>();

    // �a�}�������^�զC��A�Ω����h�Ӧ^�սШD
    private Dictionary<string, List<Action<AudioClip>>> CallbackMap = new();

    private readonly string corsanywhereUrl = "https://playoneapps.com.tw/corsanywhere/";

    // ��ҼҦ�
    public static SoundCacher Inst { get; private set; }

    private void Awake()
    {
        // �T�O��Ұߤ@��
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject); // ���������ɫO�����P��
    }

    public void GetAllAudioClipsInSA(Action onAllDone, string folderName = null)
    {
        var Mp3PathsInSA = StreamingAssets.GetAllAssetPaths(folderName: folderName, fileExt: ".mp3"); //�@�w�n�[.
        var WavPathsInSA = StreamingAssets.GetAllAssetPaths(folderName: folderName, fileExt: ".wav"); //�@�w�n�[.
        var OggPathsInSA = StreamingAssets.GetAllAssetPaths(folderName: folderName, fileExt: ".ogg"); //�@�w�n�[.
        var combinedList = Mp3PathsInSA .Concat(WavPathsInSA) .Concat(OggPathsInSA) .ToList(); // �X�֬���@�C��

        // ���b�G�p�G�S������a�}�A�N�����I�s onAllDone
        if (combinedList == null || combinedList.Count == 0)
        {
            onAllDone?.Invoke();
            return;
        }

        int totalRequests = combinedList.Count;
        int completedCount = 0;

        // �æ�a�@���e�X�Ҧ��ШD
        foreach (string path in combinedList)
        {
            GetAudioClip(path, (audioClip) =>
            {
                // ��� request �������ɭԡA��p�ƾ��[ 1
                completedCount++;

                // �Y�����������F�A�N�I�s onAllDone
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
    /// ������w�a�}�� AudioClip�A�p�G�|���[���h�Ұʥ[���y�{
    /// </summary>
    /// <param name="address">�귽�a�}</param>
    /// <param name="onComplete">�[�������ɪ��^�աA�p���ǤJ�^�իh�u�i��Cache</param>
    public void GetAudioClip(string address, Action<AudioClip> onComplete = null)
    {
        //Debug.Log($"�I�sGetSprite���o{address}");
        // �p�G�a�}���b�[���A�N�^�ե[�J���C
        if (LoadingAudioClips.Contains(address))
        {
            Debug.Log($"{address}�b���C��");
            if (CallbackMap.TryGetValue(address, out var callbacks))
            {
                callbacks.Add(onComplete);
            }
            else
            {
                Debug.Log($"�s�W{address}�춤�C");
                CallbackMap[address] = new List<Action<AudioClip>> { onComplete };
            }
            Debug.Log($"AudioClip is still loading: {address}");
            return;
        }

        // �p�G�귽�w�g�b�w�s���A��������^��
        if (audioClipCache.TryGetValue(address, out AudioClip cachedAudioClip))
        {
            Debug.Log($"{address}�w�g�b�w�s��");
            onComplete?.Invoke(cachedAudioClip);
            return;
        }

        // �}�l�s���[���L�{
        LoadingAudioClips.Add(address);
        CallbackMap[address] = new List<Action<AudioClip>> { onComplete };
        //Debug.Log($"{path}�b���W�F");
        // �ھڦa�}�����i��[��
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

    // �q Resources �귽�[�� Sprite
    private void CacheFromResources(string resourceAddress, string originalAddress)
    {
        Debug.Log($"�}�l�qResourcesŪ�� {resourceAddress}");

        AudioClip audioClip;
        resourceAddress = resourceAddress.Split('.')[0];
        Debug.Log($"�q Resources �[�� audioClip�A���|: {resourceAddress}");
        audioClip = Resources.Load<AudioClip>(resourceAddress);

        if (audioClip == null)
        {
            Debug.LogWarning($"�������|�� {resourceAddress} ��audioClip�I");
        }

        // �^�ճB�z���G
        OnAudioClipLoaded(originalAddress, audioClip);
    }


    // �q StreamingAssets �귽�[�� Sprite
    private IEnumerator CacheFromStreamingAssets(string filePath, string address)
    {
        //Debug.Log($"�}�l�qsaŪ��{filePath}");
        var sa = StreamingAssets.Inst;
        var audioType = GetAudioTypeFromAddress(address);
        yield return sa.LoadAudioClip(filePath, audioType, (audioClip) => 
        {
            OnAudioClipLoaded(address, audioClip);
        });
    }

    // �q URL �U���ýw�s Sprite
    private IEnumerator DownloadAndCacheAudioClip(string url)
    {
        Debug.Log($"SoundCacher�}�l�U��{url}");
        //Debug.Log($"corsanywhereUrl:{corsanywhereUrl + url}");
        var audioType = GetAudioTypeFromAddress(url);
        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(corsanywhereUrl + url, audioType))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"���T�U�����ѡG{url}, ���~�T���G{request.error}");
                OnAudioClipLoaded(url, null);
            }
            else
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(request);
                Debug.Log($"AudioCacher ���\�U�� AudioClip�A�q: {url}");
                OnAudioClipLoaded(url, audioClip);
            }
        }
    }

    // �귽�[����������檺�q�Φ^�ճB�z
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

    /// <summary>
    /// �ھ��ɮ׸��|���ɦW�۰ʧP�_������ AudioType
    /// </summary>
    /// <param name="address">�����ɮת�������|���ɦW</param>
    /// <returns>������ AudioType�A�Y�L�k�P�_�h�� AudioType.UNKNOWN</returns>
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
