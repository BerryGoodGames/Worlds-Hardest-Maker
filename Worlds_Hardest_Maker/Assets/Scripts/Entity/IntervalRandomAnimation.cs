using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
///     animation at random intervals
///     attach to gameObject holding animator
/// </summary>
public class IntervalRandomAnimation : MonoBehaviour
{
    [FormerlySerializedAs("intervalSeconds")] public float IntervalSeconds;
    [FormerlySerializedAs("animTriggerString")] public string AnimTriggerString;

    // value between 0 - 1, next trigger has to be in range of deviation
    [FormerlySerializedAs("limitDeviation")] [Range(0, 1)] public float LimitDeviation;

    [FormerlySerializedAs("triggerOnlyAtPlayMode")] public bool TriggerOnlyAtPlayMode;

    [FormerlySerializedAs("soundEffect")] public string SoundEffect;

    private int lastTrigger;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (TriggerOnlyAtPlayMode && !EditModeManager.Instance.Playing) return;

        if (lastTrigger >= IntervalSeconds / Time.fixedDeltaTime * LimitDeviation)
        {
            CheckAnimationTrigger();
        }

        lastTrigger++;
    }

    private void CheckAnimationTrigger()
    {
        // check animation trigger
        float p = Time.fixedDeltaTime / IntervalSeconds;

        if (Random.Range(0, 0.999f) >= p &&
            lastTrigger < IntervalSeconds / Time.fixedDeltaTime * (LimitDeviation + 1)) return;

        anim.SetTrigger(AnimTriggerString);

        if (!SoundEffect.Equals("")) AudioManager.Instance.Play(SoundEffect);

        lastTrigger = 0;
    }
}