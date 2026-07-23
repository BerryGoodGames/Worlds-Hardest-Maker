using System;
using MyBox;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class AudioManager : MonoBehaviour, IAudioService
{
    [SerializeField] [PositiveValueOnly] [InitializationField] private float transitionTime = 0.5f;
    
    [SerializeField] [InitializationField] private AudioMixerSnapshot defaultState;
    
    [SerializeField] [InitializationField] private AudioMixerSnapshot filteredState;
    
    [Space] [SerializeField] private Sound[] sounds;
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
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
    
    public void Play(SoundEffect sfx)
    {
        Sound sound = Array.Find(sounds, sound => sound.Name == sfx.Sound);
        
        if (sound == null)
        {
            Debug.LogWarning($"The sound called {sfx.Sound} was not found!");
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
    
    private void Awake()
    {
        DontDestroyOnLoad(this);
        
        sounds.ForEach(sound => sound.CreateSources(gameObject));
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt)
    {
        Play("Bell");
        MusicFiltered(false);
    }
    
    private void OnSwitchToEdit(SwitchToEditEvent evt)
    {
        Play("Bell");
        MusicFiltered(true);
    }
}