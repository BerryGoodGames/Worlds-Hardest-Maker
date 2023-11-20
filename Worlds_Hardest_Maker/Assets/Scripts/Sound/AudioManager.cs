using System;
using MyBox;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] [PositiveValueOnly] [InitializationField] private float transitionTime = 0.5f;

    [FormerlySerializedAs("DefaultState")] [SerializeField] [InitializationField] private AudioMixerSnapshot defaultState;

    [FormerlySerializedAs("FilteredState")] [SerializeField] [InitializationField] private AudioMixerSnapshot filteredState;

    [FormerlySerializedAs("Sounds")] [Space] [SerializeField] private Sound[] sounds;

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

    public void MusicFiltered(bool filtered) => (filtered ? filteredState : defaultState).TransitionTo(transitionTime);
}