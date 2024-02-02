using UnityEngine;

/// <summary>
///     Triggers animation at random intervals
///     <para>Attach to gameObject holding animator</para>
/// </summary>
[RequireComponent(typeof(Animator))]
public class IntervalRandomAnimation : MonoBehaviour
{
    public float IntervalSeconds;

    public string AnimTriggerString;

    // value between 0 - 1, next trigger has to be in range of deviation
    [Range(0, 1)] public float LimitDeviation;

    public bool TriggerOnlyAtPlayMode;

    public SoundEffect SoundEffect;

    private int lastTrigger;

    private Animator anim;

    private void Awake() => anim = GetComponent<Animator>();

    private void FixedUpdate()
    {
        if (TriggerOnlyAtPlayMode && !LevelSessionEditManager.Instance.Playing) return;

        if (lastTrigger >= IntervalSeconds / Time.fixedDeltaTime * LimitDeviation) CheckAnimationTrigger();

        lastTrigger++;
    }

    private void CheckAnimationTrigger()
    {
        // check animation trigger
        float p = Time.fixedDeltaTime / IntervalSeconds;

        if (Random.Range(0, 0.999f) >= p &&
            lastTrigger < IntervalSeconds / Time.fixedDeltaTime * (LimitDeviation + 1)) return;

        anim.SetTrigger(AnimTriggerString);

        AudioManager.Instance.Play(SoundEffect);

        lastTrigger = 0;
    }
}