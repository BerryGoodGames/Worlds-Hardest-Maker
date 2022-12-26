using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// animation at random intervals
/// attach to gameObject holding animator
/// </summary>
public class IntervalRandomAnimation : MonoBehaviour
{
    public float intervalSeconds;
    public string animTriggerString;

    // value between 0 - 1, next trigger has to be in range of deviation
    [Range(0, 1)] public float limitDeviation;

    public bool triggerOnlyAtPlayMode;

    public string soundEffect;

    private int lastTrigger;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (triggerOnlyAtPlayMode && !GameManager.Instance.Playing) return;

        if(lastTrigger >= intervalSeconds / Time.fixedDeltaTime * limitDeviation)
        {
            CheckAnimationTrigger();
        }

        lastTrigger++;
    }

    private void CheckAnimationTrigger()
    {
        // check animation trigger
        float p = Time.fixedDeltaTime / intervalSeconds;

        if (Random.Range(0, 0.999f) >= p && lastTrigger < intervalSeconds / Time.fixedDeltaTime * (limitDeviation + 1)) return;

        anim.SetTrigger(animTriggerString);

        if (!soundEffect.Equals("")) AudioManager.Instance.Play(soundEffect);

        lastTrigger = 0;
    }
}
