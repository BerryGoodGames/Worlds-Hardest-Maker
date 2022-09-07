using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// animation at random intervals
/// attach to gameobject holding animator
/// </summary>
public class IntervalRandomAnimation : MonoBehaviour
{
    public float intervalSeconds;
    public string animTriggerString;
    // value between 0 - 1, next trigger has to be in range of deviation
    public float limitDeviation;

    public bool triggerAtPlayMode;

    private int lastTrigger = 0;

    private void FixedUpdate()
    {
        if (triggerAtPlayMode && !GameManager.Instance.Playing) return;

        if(lastTrigger >= intervalSeconds / Time.fixedDeltaTime * limitDeviation)
        {
            float p = Time.fixedDeltaTime / intervalSeconds;

            if (Random.Range(0, 0.999f) < p || lastTrigger >= intervalSeconds / Time.fixedDeltaTime * (limitDeviation + 1))
            {
                GetComponent<Animator>().SetTrigger(animTriggerString);

                lastTrigger = 0;
                return;
            }
        }

        lastTrigger++;
    }
}
