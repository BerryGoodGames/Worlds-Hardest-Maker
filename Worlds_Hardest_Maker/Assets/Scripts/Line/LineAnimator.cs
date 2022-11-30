using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineAnimator : MonoBehaviour
{
    private LineRenderer l;

    private void Start()
    {
        l = GetComponent<LineRenderer>();
    }

    public void AnimatePoint(int lineRenderPoint, Vector2 pos, float duration, Ease ease = Ease.InOutSine)
    {
        DOTween.To(() => (Vector2)l.GetPosition(lineRenderPoint), (x) => l.SetPosition(lineRenderPoint, x), pos, duration)
            .SetEase(ease)
            .Play();
    }
    public void AnimateAllPoints(List<Vector2> poses, float duration, Ease ease = Ease.InOutSine)
    {
        if (poses.Count != l.positionCount) throw new System.Exception($"Tried to animate {poses.Count} line vertecies but line has {l.positionCount}");

        for (int i = 0; i < l.positionCount; i++)
        {
            AnimatePoint(i, poses[i], duration, ease);
        }
    }
    public void AnimateMove(Vector2 move, float duration, Ease ease = Ease.InOutSine)
    {
        for(int i = 0; i < l.positionCount; i++)
        {
            Vector2 pointPos = l.GetPosition(i);

            AnimatePoint(i, pointPos + move, duration, ease);
        }
    }
}
