using JetBrains.Annotations;
using UnityEngine;
using Zenject;

public class PlaySoundEffect : MonoBehaviour
{
    [SerializeField] private SoundEffect soundEffect;
    
    private IAudioService audioService;
    
    [Inject]
    private void Construct(IAudioService audioService)
    {
        this.audioService = audioService;
    }

    [UsedImplicitly]
    public void Play() => audioService.Play(soundEffect);
}