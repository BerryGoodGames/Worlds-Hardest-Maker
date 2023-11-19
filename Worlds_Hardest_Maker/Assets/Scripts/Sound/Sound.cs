using System;
using MyBox;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

[Serializable]
public class Sound
{
    public string Name;

    [FormerlySerializedAs("AudioClip")] [Space] [SerializeField] private AudioClip audioClip;

    [FormerlySerializedAs("Output")] [Space] [SerializeField] private AudioMixerGroup output;

    [FormerlySerializedAs("Mute")]
    [Separator("Settings")] 
    [SerializeField] private bool mute;
    [FormerlySerializedAs("PlayOnAwake")] [SerializeField] private bool playOnAwake;
    [FormerlySerializedAs("Loop")] [SerializeField] private bool loop;
    [SerializeField] private bool allowOverlap = true;

    [SerializeField] private float cooldown;
    [FormerlySerializedAs("Volume")] [Range(0f, 1f)][SerializeField] private float volume = 1;

    [FormerlySerializedAs("Pitch")] [Range(-3f, 3f)][SerializeField] private float pitch = 1;

    [HideInInspector] private AudioSource source;

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
    
    public void Play()
    {
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