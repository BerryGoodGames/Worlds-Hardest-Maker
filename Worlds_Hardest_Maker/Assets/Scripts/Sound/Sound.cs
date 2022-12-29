using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

[Serializable]
public class Sound
{
    public string name;

    [FormerlySerializedAs("AudioClip")] [Space]
    public AudioClip audioClip;

    [FormerlySerializedAs("Output")] [Space]
    public AudioMixerGroup output;

    public bool mute;
    public bool playOnAwake;
    public bool loop;
    [Range(0f, 1f)] public float volume = 1;
    [Range(-3f, 3f)] public float pitch = 1;
    [HideInInspector] public AudioSource source;
}