using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : Singleton<AudioManager>
{
    [Header("音频设置")]
    public AudioSource audioSource;  
    [Range(0f, 1f)]
    [SerializeField] private float volume = .3f;
    public float Volume => volume;
    
    [SerializeField] private AudioClipRefsSO audioClipRefsSO;
    public AudioClipRefsSO AudioClipRefsSO => audioClipRefsSO;
    
    protected override void Awake()
    {
        base.Awake();
        
        // 初始化音量
        // audioSource = GetComponent<AudioSource>();
        volume = PlayerPrefs.GetFloat(Settings.PLAYER_PREFS_MUSIC_VOLUME_KEY, .3f);
        audioSource.volume = volume;
        audioSource.clip = audioClipRefsSO.gameMusic;
    }

    public void ChangeVolume(float volume)
    {
        this.volume = volume;
        audioSource.volume = volume;
        PlayerPrefs.SetFloat(Settings.PLAYER_PREFS_MUSIC_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }
}
