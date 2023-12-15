using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BGMStruct
{
    public string name;
    public int index;
    public AudioClip clip;
}
[Serializable]
public class SFXStruct
{
    public string name;
    public int index;
    public AudioClip clip;
}
public class SoundManager2 : GameObjectSingleton<SoundManager2>
{
    public AudioSource bgmAudioSource;
    public AudioSource sfxAudioSource;
    public List<BGMStruct> bgmSoundList;
    public List<SFXStruct> sfxSoundList;
   
    protected override void Awake()
    {
        base.Awake();
    }
    public void BgmPlaySound(string name, float volume = 1f)
    {
        int soundIndex = bgmSoundList.Find(t => t.name == name).index;
        bgmAudioSource.clip = bgmSoundList[soundIndex].clip;
        bgmAudioSource.volume = volume;
        bgmAudioSource.Play();

    }
    public void BgmStopSound(string name)
    {
        int soundIndex = bgmSoundList.Find(t => t.name == name).index;
        bgmAudioSource.clip = bgmSoundList[soundIndex].clip;
        bgmAudioSource.Pause();

    } 
    public void SfxPlaySound(string name, float volume = 1f)
    {
        int soundIndex = sfxSoundList.Find(t => t.name == name).index;
        sfxAudioSource.clip = sfxSoundList[soundIndex].clip;
        
        sfxAudioSource.PlayOneShot(sfxAudioSource.clip, volume);
    }
}
