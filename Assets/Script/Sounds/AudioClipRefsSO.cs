using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AudioClipRefsSO : ScriptableObject
{
    [Tooltip("游戏音乐")] public AudioClip gameMusic;
    
    [Tooltip("入场音效")] public AudioClip entranceSound;
    
    [Tooltip("合成音效")] public List<AudioClip> mergeSoundList;
}
