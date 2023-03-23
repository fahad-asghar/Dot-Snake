using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioClip buttonClick1;
    public AudioClip buttonClick2;
    public List<AudioClip> win;
    public List<AudioClip> lose;
    public AudioClip snakeMove;
    public List<AudioClip> eatPoint;
    public List<AudioClip> snakeHit;
    public AudioClip snakeShorten;
    public AudioClip snakeBlink;
    public List<AudioClip> specialPoint;
    public List<AudioClip> healthPoint;
    public AudioClip popUp;
    public AudioClip swipe;
    public AudioClip notification;
   
    private void Awake()
    {
        instance = this;
    }

    public void Mute()
    {
        PlayerPrefs.SetInt("Mute", 1);
    }

    public void UnMute()
    {
        PlayerPrefs.SetInt("Mute", 0);
    }

    public void playSound(AudioClip audio)
    {
        if(PlayerPrefs.GetInt("Mute") == 1)
            return;

        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = audio;
        source.Play();

        Destroy(source, audio.length + 0.1f);
    }

    public void playSound(AudioClip audio, float volume)
    {
        if(PlayerPrefs.GetInt("Mute") == 1)
            return;

        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.volume = volume;
        source.clip = audio;
        source.Play();

        Destroy(source, audio.length + 0.1f);
    }
}