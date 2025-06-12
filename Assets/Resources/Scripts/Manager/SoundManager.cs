using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public struct SoundData
{
    public float bgm;
    public float sfx;
}

public class SoundManager : MonoBehaviour
{
    static SoundManager instance;

    AudioSource audioSource;
    AudioMixer audioMixer;
    public List<AudioClip> audioClips;
    public List<Slider> sliderList;

    public float bgmvalue;
    public float sfxvalue;

    public SoundData soundData;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadData();
        sliderList[0].value = soundData.bgm;
        sliderList[1].value = soundData.sfx;

        gameObject.GetComponent<AudioSource>().clip = audioClips[0];
        gameObject.GetComponent<AudioSource>().Play();
    }

    void SaveData()
    {

    }

    void LoadData()
    {

    }

    public void BGMSoundController(float volum)
    {
        volum = sliderList[0].value;
        soundData.bgm = volum;
        SaveData();
    }

    public void SFXSoundController(float volum)
    {
        volum = sliderList[1].value;
        soundData.sfx = volum;
        SaveData();
    }

    private void OnDisable()
    {
        bgmvalue = sliderList[0].value;
        sfxvalue = sliderList[1].value;
    }
}
