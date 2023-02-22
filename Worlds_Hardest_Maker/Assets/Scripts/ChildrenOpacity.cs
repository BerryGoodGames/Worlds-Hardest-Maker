using System.Collections;
using UnityEngine;

/// <summary>
///     sets opacity of children; fades children in / out
///     attach to parent object
/// </summary>
public class ChildrenOpacity : MonoBehaviour
{
    [SerializeField] [Range(0, 1)] private float opacity = 1;

    private SpriteRenderer[] children;

    private void Start()
    {
        UpdateChildren();
    }

    public void UpdateChildren()
    {
        children = transform.GetComponentsInChildren<SpriteRenderer>();
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

    public IEnumerator FadeOut(float endOpacity, float time)
    {
        UpdateChildren();
        if (endOpacity >= 1) yield break;

        while (opacity >= endOpacity)
        {
            opacity -= (1 - endOpacity) * Time.deltaTime / time;
            UpdateOpacity();

            yield return null;
        }

        opacity = endOpacity;
    }

    public IEnumerator FadeIn(float endOpacity, float time)
    {
        UpdateChildren();
        if (endOpacity <= 0) yield break;

        if (time <= 0)
        {
            opacity = endOpacity;
            UpdateOpacity();
            yield break;
        }

        while (opacity <= endOpacity)
        {
            opacity += endOpacity * Time.deltaTime / time;
            UpdateOpacity();

            yield return null;
        }

        opacity = endOpacity;
    }
}