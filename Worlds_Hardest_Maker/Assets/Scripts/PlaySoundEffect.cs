using JetBrains.Annotations;
using UnityEngine;

public class PlaySoundEffect : MonoBehaviour
{
    [SerializeField] private SoundEffect soundEffect;

    [UsedImplicitly]
    public void Play() => AudioManager.Instance.Play(soundEffect);
}