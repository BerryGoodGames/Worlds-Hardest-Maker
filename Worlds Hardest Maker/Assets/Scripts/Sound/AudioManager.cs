using System;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public float transitionTime = 1;
    public AudioMixerSnapshot defaultState;
    public AudioMixerSnapshot filteredState;
    [Space]
    public Sound[] sounds;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.AudioClip;
            s.source.outputAudioMixerGroup = s.Output;
            s.source.mute = s.mute;
            s.source.loop = s.loop;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            if(s.playOnAwake)
            {
                s.source.Play();
            }
        }
    }
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null)
        {
            Debug.LogWarning($"The sound name {name} was not found!");
            return;
        }
        s.source.Play();
    }
    public void MusicFiltered(bool filtered)
    {
        if(filtered)
        {
            filteredState.TransitionTo(transitionTime);
        }
        else
        {
            defaultState.TransitionTo(transitionTime);
        }
    }
}
