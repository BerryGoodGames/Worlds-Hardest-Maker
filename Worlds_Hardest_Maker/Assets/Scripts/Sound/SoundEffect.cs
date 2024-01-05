using System;
using MyBox;
using UnityEngine;

[Serializable]
public class SoundEffect
{
    [SerializeField] public string Sound;
    [SerializeField] public bool PitchRandomization;
    [SerializeField] [PositiveValueOnly] [ConditionalField(nameof(PitchRandomization))] public float PitchDeviation;

    public SoundEffect(string sound, bool pitchRandomization = false, float pitchDeviation = 0)
    {
        Sound = sound;
        PitchRandomization = pitchRandomization;
        PitchDeviation = pitchDeviation;
    }
}