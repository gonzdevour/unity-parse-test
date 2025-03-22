using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Audio;
using System.Net;

public class Sound : MonoBehaviour
{
    public static Sound Inst { get; private set; }

    [Header("AudioMixer 設定")]
    public AudioMixer audioMixer;
    public AudioMixerGroup bgmMixerGroup;
    public AudioMixerGroup sfxMixerGroup;

    [Header("Audio Source")]
    private AudioSource bgmSource;
    private AudioSource sfxSource;

    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (Inst != null && Inst != this) { Destroy(gameObject); return; }
        Inst = this;
        DontDestroyOnLoad(gameObject);

        // 建立 AudioSource
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.outputAudioMixerGroup = bgmMixerGroup;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = sfxMixerGroup;
    }

    #region 播放/淡入BGM

    public void PlayBGM(string clipAddress, float targetVolume = 1f, float fadeDuration = 5f, float delay = 0f)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeInBGM(clipAddress, targetVolume, fadeDuration, delay));
    }

    private IEnumerator FadeInBGM(string clipAddress, float targetVolume = 1f, float fadeDuration = 1f, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        bool isClipReady = false;
        AudioClip resultClip = null;
        SoundCacher.Inst.GetAudioClip(clipAddress, (clip) =>
        {
            resultClip = clip;
            isClipReady = true;
        });
        yield return new WaitUntil(()=>isClipReady);
        bgmSource.clip = resultClip;
        bgmSource.volume = 0f;
        bgmSource.Play();

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, targetVolume, timer / fadeDuration);
            yield return null;
        }
        bgmSource.volume = targetVolume;
    }

    public void StopBGM(float fadeDuration = 1f)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeOutBGM(fadeDuration));
    }

    private IEnumerator FadeOutBGM(float duration)
    {
        float startVolume = bgmSource.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.volume = 1f;
    }
    #endregion

    #region 播放 SFX
    public void PlaySFX(string clipAddress, float volume = 1f)
    {
        SoundCacher.Inst.GetAudioClip(clipAddress, (clip) =>
        {
            sfxSource.PlayOneShot(clip, volume);
        });
    }
    #endregion

    #region 音量控制（AudioMixer）
    public void SetMasterVolume(float normalized) => SetMixerVolume("MasterVolume", normalized);
    public void SetBGMVolume(float normalized) => SetMixerVolume("BGMVolume", normalized);
    public void SetSFXVolume(float normalized) => SetMixerVolume("SFXVolume", normalized);

    private void SetMixerVolume(string parameter, float normalized)
    {
        float dB = Mathf.Lerp(-80f, 0f, normalized); // -80 dB ~ 0 dB
        audioMixer.SetFloat(parameter, dB);
    }
    #endregion
}
