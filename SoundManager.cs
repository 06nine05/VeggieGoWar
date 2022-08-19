using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum Sound
    {
        BGM,
        Attack,
        Lose,
        Win,
        BossBGM,
        BossDie,
        Heal,
        Shield,
        ShieldBreak
    }
    
    public static SoundManager instance;

    //private SoundManager() { instance = this; }

    [SerializeField] private SoundClip[] soundClips;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        PlayBGM();
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Play sound
    /// </summary>
    /// <param name="audioSource"></param>
    /// <param name="sound"></param>
    public void Play(AudioSource audioSource, Sound sound)
    {
        Debug.Assert(audioSource != null, "audioSource cannot be null");

        audioSource.clip = GetAudioClip(sound);
        audioSource.Play();
    }

    public void PlayBGM()
    {
        Play(bgmSource, Sound.BGM);
    }

    public void PlayBossBGM()
    {
        Play(bgmSource, Sound.BossBGM);
    }

    public void PlayLose()
    {
        Play(bgmSource, Sound.Lose);
    }

    public void PlayWin()
    {
        Play(audioSource, Sound.BossDie);
        Play(bgmSource, Sound.Win);
    }
    

    public void Stop()
    {
        audioSource.Stop();
    }

    private void StopBGM()
    {
        bgmSource.Stop();
    }

    private AudioClip GetAudioClip(Sound sound)
    {
        foreach (var soundClip in soundClips)
        {
            if (soundClip.Sound == sound)
            {
                return soundClip.AudioClip;
            }
        }

        Debug.Assert(false, $"Cannot find sound {sound}");
        return null;
    }

    [Serializable]
    public struct SoundClip
    {
        public Sound Sound;
        public AudioClip AudioClip;
    }
}