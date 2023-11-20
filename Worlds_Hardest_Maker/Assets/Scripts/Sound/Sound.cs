using System;
using MyBox;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using Random = System.Random;

[Serializable]
public class Sound
{
    public string Name;

    [Space] [SerializeField] private AudioClip audioClip;

    [Space] [SerializeField] private AudioMixerGroup output;
    
    [Separator("Settings")] 
    [SerializeField] private bool mute;
    [SerializeField] private bool playOnAwake;
    [SerializeField] private bool loop;
    [SerializeField] private bool allowOverlap = true;

    [SerializeField] private float cooldown;
    [Range(0f, 1f)] [SerializeField] private float volume = 1;

    [Range(-3f, 3f)] [SerializeField] private float pitch = 1;

    private AudioSource source;

    public void CreateSources(GameObject sourceContainer)
    {
        source = sourceContainer.AddComponent<AudioSource>();
        source.clip = audioClip;
        source.outputAudioMixerGroup = output;
        source.mute = mute;
        source.loop = loop;
        source.volume = volume;
        source.pitch = pitch;
        if (playOnAwake) source.Play();
    }

    private TimeSpan nextPlay = DateTime.Now.TimeOfDay;

    public void Play(float pitchRandomizationDeviation = 0)
    {
        source.pitch = pitch + UnityEngine.Random.Range(-pitchRandomizationDeviation, pitchRandomizationDeviation);
        
        // check if on cooldown
        if (cooldown > 0)
        {
            if (nextPlay > DateTime.Now.TimeOfDay) return;
            
            nextPlay = DateTime.Now.TimeOfDay + TimeSpan.FromSeconds(cooldown);
        }
        
        if(allowOverlap)
            source.PlayOneShot(audioClip);
        else source.Play();
    }
}