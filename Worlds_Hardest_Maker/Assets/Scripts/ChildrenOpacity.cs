using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

/// <summary>
///     Sets opacity of children with SpriteRenderer, fades children in / out
///     Attach to parent object
/// </summary>
public class ChildrenOpacity : MonoBehaviour
{
    [SerializeField] [Range(0, 1)] private float opacity = 1;

    private SpriteRenderer[] children;

    private void Start() => UpdateChildren();

    public void UpdateChildren()
    {
        children = GetComponentsInChildren<SpriteRenderer>();
        print(string.Join(" ", children.ToList()));
    }

    public void UpdateOpacity()
    {
        foreach (SpriteRenderer child in children)
        {
            Color newColor = child.color;
            newColor.a = opacity;
            child.color = newColor;
        }
    }

    public void SetOpacity(float newOpacity)
    {
        opacity = newOpacity;
        UpdateOpacity();
    }

    public void FadeTo(float endOpacity, float time)
    {
        UpdateChildren();
        DOTween.To(() => opacity, SetOpacity, endOpacity, time);
    }
}