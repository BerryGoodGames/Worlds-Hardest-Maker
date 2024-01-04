using System;
using MyBox;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] [PositiveValueOnly] [InitializationField] private float transitionTime = 0.5f;

    [SerializeField] [InitializationField] private AudioMixerSnapshot defaultState;

    [SerializeField] [InitializationField] private AudioMixerSnapshot filteredState;

    [Space] [SerializeField] private Sound[] sounds;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        sounds.ForEach(sound => sound.CreateSources(gameObject));
    }

    public void Play(string name)
    {
        Sound sound = Array.Find(sounds, sound => sound.Name == name);
        if (sound == null)
        {
            Debug.LogWarning($"The sound called {name} was not found!");
            return;
        }

        sound.Play();
    }

    public void Play(PlaceManager.PlaceSfx sfx)
    {
        Sound sound = Array.Find(sounds, sound => sound.Name == sfx.Sound);
        if (sound == null)
        {
            Debug.LogWarning($"The sound called {name} was not found!");
            return;
        }

        // randomize pitch
        if (sfx.PitchRandomization)
        {
            sound.Play(sfx.PitchDeviation);
            return;
        }

        sound.Play();
    }

    public void MusicFiltered(bool filtered) => (filtered ? filteredState : defaultState).TransitionTo(transitionTime);
}