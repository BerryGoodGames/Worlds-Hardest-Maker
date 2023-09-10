using System;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class Sound
{
    public string Name;

    [Space] public AudioClip AudioClip;

    [Space] public AudioMixerGroup Output;

    public bool Mute;
    public bool PlayOnAwake;
    public bool Loop;

    [Range(0f, 1f)] public float Volume = 1;

    [Range(-3f, 3f)] public float Pitch = 1;

    [HideInInspector] public AudioSource Source;
}