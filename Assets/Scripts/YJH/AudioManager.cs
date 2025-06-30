using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource bgmSource;
    private List<AudioSource> sfxSources = new();
    private Dictionary<string, AudioClip> bgmClips = new();
    private Dictionary<string, AudioClip> sfxClips = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject); 
            return; 
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        //기본 AudioSource 생성
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 50f) / 100f;
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 50f) / 100f;
        bgmSource.volume = bgmVolume;

        for (int i = 0; i < 3; i++)
        {
            AudioSource sfx = gameObject.AddComponent<AudioSource>();
            sfx.volume = sfxVolume;
            sfxSources.Add(sfx);
        }

        //오디오 리소스 불러오기
        foreach (var clip in Resources.LoadAll<AudioClip>("Audio/BGM"))
        {
            bgmClips[clip.name] = clip;
        }

        foreach (var clip in Resources.LoadAll<AudioClip>("Audio/SFX"))
        {
            sfxClips[clip.name] = clip;
        }

        PlayBGM("OutGameBGM");
    }

    public void PlayBGM(string name)
    {
        if (!bgmClips.ContainsKey(name)) return;
        if (bgmSource.clip == bgmClips[name]) return;

        bgmSource.clip = bgmClips[name];
        bgmSource.Play();
    }

    public void PlaySFX(string name)
    {
        if (!sfxClips.ContainsKey(name)) return;
        AudioClip clip = sfxClips[name];

        foreach (var src in sfxSources)
        {
            if (!src.isPlaying)
            {
                src.clip = clip;
                src.Play();
                return;
            }
        }

        // 다 사용 중이면 새 소스 추가
        AudioSource newSrc = gameObject.AddComponent<AudioSource>();
        newSrc.volume = PlayerPrefs.GetFloat("SFXVolume", 50f) / 100f;
        newSrc.clip = clip;
        newSrc.Play();
        sfxSources.Add(newSrc);
    }

    public void SetBGMVolume(float v)
    {
        bgmSource.volume = v;
    }

    public void SetSFXVolume(float v)
    {
        foreach (var src in sfxSources)
        {
            src.volume = v;
        }
    }
    
}
