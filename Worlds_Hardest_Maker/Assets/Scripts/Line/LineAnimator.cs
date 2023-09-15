using System;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineAnimator : MonoBehaviour
{
    public LineRenderer LineRenderer { get; private set; }

    private void Awake() => LineRenderer = GetComponent<LineRenderer>();

    public void AnimatePoint(int lineRenderPoint, Vector2 pos, float duration, Ease ease = Ease.InOutSine) =>
        DOTween.To(() =>
                {
                    if(LineRenderer == null) return Vector2.zero;
                    return LineRenderer.GetPosition(lineRenderPoint);
                },
                x =>
                {
                    if (LineRenderer == null) return;
                    LineRenderer.SetPosition(lineRenderPoint, x);
                }, pos, duration)
            .SetEase(ease)
            .Play();

    public void AnimateAllPoints(List<Vector2> poses, float duration, Ease ease = Ease.InOutSine)
    {
        if (poses.Count != LineRenderer.positionCount)
        {
            throw new($"Tried to animate {poses.Count} line vertices but line has {LineRenderer.positionCount}");
        }

        for (int i = 0; i < LineRenderer.positionCount; i++)
        {
            AnimatePoint(i, poses[i], duration, ease);
        }
    }

    public void AnimateMove(Vector2 move, float duration, Ease ease = Ease.InOutSine)
    {
        Stopwatch sw = new();
        sw.Start();

        for (int i = 0; i < LineRenderer.positionCount; i++)
        {
            Vector2 pointPos = LineRenderer.GetPosition(i);

            AnimatePoint(i, pointPos + move, duration, ease);
        }

        sw.Stop();
        print(sw.Elapsed.TotalMilliseconds);
    }
}