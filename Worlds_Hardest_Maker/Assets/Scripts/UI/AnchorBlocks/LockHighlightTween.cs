using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using UnityEngine;

public class LockHighlightTween : MonoBehaviour
{
    [SerializeField] private float duration = 1f;
    [SerializeField] private float scale = 3;
    [SerializeField] private int shakes = 5;
    [SerializeField] private int shakeRotation = 45;

    [ButtonMethod]
    public void Highlight()
    {
        // scale
        Sequence scaleSequence = DOTween.Sequence();

        scaleSequence
                // scale up
                .Append(transform.DOScale(Vector3.one * scale, duration / 2).SetEase(Ease.InOutSine))
                // scale back down
                .Append(transform.DOScale(Vector3.one, duration / 2).SetEase(Ease.InOutSine));
        
        // shake
        Sequence shakeSequence = DOTween.Sequence();
        float singleShakeDuration = duration / shakes;
        float shakeRotationCopy = shakeRotation;

        for (int i = 0; i < shakes; i++)
        {
            shakeSequence
                // go into shake position
                .Append(transform.DORotate(new(0, 0, shakeRotationCopy), singleShakeDuration / 2).SetEase(Ease.OutSine))
                // return to normal position
                .Append(transform.DORotate(Vector3.zero, singleShakeDuration / 2).SetEase(Ease.InSine));

            // shake into other direction next time
            shakeRotationCopy *= -1;
        }
    }
}
