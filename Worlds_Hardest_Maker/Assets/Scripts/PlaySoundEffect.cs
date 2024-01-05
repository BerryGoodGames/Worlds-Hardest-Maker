using UnityEngine;

public class PlaySoundEffect : MonoBehaviour
{
    [SerializeField] private SoundEffect soundEffect;

    public void Play() => AudioManager.Instance.Play(soundEffect);
}
