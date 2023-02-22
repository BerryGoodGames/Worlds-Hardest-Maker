using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

[Serializable]
public class Sound
{
    [FormerlySerializedAs("name")] public string Name;

    [FormerlySerializedAs("audioClip")] [Space]
    public AudioClip AudioClip;

    [FormerlySerializedAs("output")] [Space]
    public AudioMixerGroup Output;

    [FormerlySerializedAs("mute")] public bool Mute;
    [FormerlySerializedAs("playOnAwake")] public bool PlayOnAwake;
    [FormerlySerializedAs("loop")] public bool Loop;

    [FormerlySerializedAs("volume")] [Range(0f, 1f)]
    public float Volume = 1;

    [FormerlySerializedAs("pitch")] [Range(-3f, 3f)]
    public float Pitch = 1;

    [FormerlySerializedAs("source")] [HideInInspector]
    public AudioSource Source;
}