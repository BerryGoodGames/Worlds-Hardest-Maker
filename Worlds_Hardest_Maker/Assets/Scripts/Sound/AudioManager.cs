using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public float TransitionTime = 1;

    public AudioMixerSnapshot DefaultState;

    public AudioMixerSnapshot FilteredState;

    [Space] public Sound[] Sounds;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        foreach (Sound sound in Sounds)
        {
            sound.CreateSources(gameObject);
        }
    }

    public void Play(string name)
    {
        Sound sound = Array.Find(Sounds, sound => sound.Name == name);
        if (sound == null)
        {
            Debug.LogWarning($"The sound called {name} was not found!");
            return;
        }

        sound.Play();
    }

    public void MusicFiltered(bool filtered)
    {
        if (filtered) FilteredState.TransitionTo(TransitionTime);
        else DefaultState.TransitionTo(TransitionTime);
    }
}