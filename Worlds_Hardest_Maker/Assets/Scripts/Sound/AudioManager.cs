using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [FormerlySerializedAs("transitionTime")]
    public float TransitionTime = 1;

    [FormerlySerializedAs("defaultState")] public AudioMixerSnapshot DefaultState;

    [FormerlySerializedAs("filteredState")]
    public AudioMixerSnapshot FilteredState;

    [FormerlySerializedAs("sounds")] [Space]
    public Sound[] Sounds;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        foreach (Sound s in Sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.AudioClip;
            s.Source.outputAudioMixerGroup = s.Output;
            s.Source.mute = s.Mute;
            s.Source.loop = s.Loop;
            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
            if (s.PlayOnAwake) s.Source.Play();
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(Sounds, sound => sound.Name == name);
        if (s == null)
        {
            Debug.LogWarning($"The sound name {name} was not found!");
            return;
        }

        s.Source.Play();
    }

    public void MusicFiltered(bool filtered)
    {
        if (filtered)
            FilteredState.TransitionTo(TransitionTime);
        else
            DefaultState.TransitionTo(TransitionTime);
    }
}