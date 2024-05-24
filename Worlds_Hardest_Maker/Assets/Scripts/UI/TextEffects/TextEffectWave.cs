using MyBox;
using UnityEngine;

public class TextEffectWave : TextEffect
{
    [Separator("Wave settings")]
    [SerializeField] private float waveLength = 0.01f;
    [SerializeField] private float amplitude = 7f;
    
    protected override Vector2 AnimationOffset(float time)
    {
        return new Vector2(0, amplitude * Mathf.Sin(time / waveLength));
    }
}
