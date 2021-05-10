using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;
    private List<AudioSource> audioSources;
    private List<float> audioSourcesMax = new List<float>();
    private float volume = 10.0f;
    private bool playingNormalMusic;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume * (volume / 10);
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.spatialBlend = s.spatialBlend;
            audioSourcesMax.Add(s.volume);
        }
    }

    void Start()
    {
        Play("backgroundSound");
        playingNormalMusic = true;
        Play("waterDroplet");
    }
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Stop();
    }
    public int GetVolume()
    {
        return (int)volume * 10;
    }
    public void SetVolume(float newVolume)
    {
        volume = newVolume;
        int i = 0;
        foreach (Sound s in sounds)
        {
            s.source.volume = audioSourcesMax[i] * (volume / 10.0f);
            i++;
        }
    }
    public bool GetPlayingNormalMusic()
    {
        return playingNormalMusic;
    }
    public void SetPlayingNormalMusic(bool toggle)
    {
        playingNormalMusic = toggle;
    }

}
