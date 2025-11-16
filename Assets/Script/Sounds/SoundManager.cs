using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [Header("音频设置")]
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;
    public float Volume => volume;

    protected override void Awake()
    {
        base.Awake();
        
        // 初始化音量
        volume = PlayerPrefs.GetFloat(Settings.PLAYER_PREFS_SFX_VOLUME_KEY, 1f);
    }
    
    public void ChangeVolume(float volume)
    {
        this.volume = volume;
        
        PlayerPrefs.SetFloat(Settings.PLAYER_PREFS_SFX_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    public void PlaySound(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, transform.position, volume * 1.5f);
    }
    
}
