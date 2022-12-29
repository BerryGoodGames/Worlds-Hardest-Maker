using System;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class Sound
{
    public string name;
    [Space] public AudioClip AudioClip;
    [Space] public AudioMixerGroup Output;
    public bool mute;
    public bool playOnAwake;
    public bool loop;
    [Range(0f, 1f)] public float volume = 1;
    [Range(-3f, 3f)] public float pitch = 1;
    [HideInInspector] public AudioSource source;
}