using MyBox;
using UnityEngine;

public class TextEffectCharacterWobble : TextEffect
{
    [Separator("Wobble settings")]
    [SerializeField] private float xFrequency = 3.3f;
    [SerializeField] private float xAmplitude = 1f;
    [SerializeField] private float yFrequency = 2.5f;
    [SerializeField] private float yAmplitude = 1f;
    
    protected override Vector2 AnimationOffset(float time)
    {
        return new Vector2(xAmplitude * Mathf.Sin(time * xFrequency), yAmplitude * Mathf.Cos(time * yFrequency));
    }
}
