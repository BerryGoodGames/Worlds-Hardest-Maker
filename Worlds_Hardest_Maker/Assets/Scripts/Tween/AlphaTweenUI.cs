using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class AlphaTweenUI : MonoBehaviour
{
    [Range(0, 1)][SerializeField] private float alphaVisible;
    [Range(0, 1)][SerializeField] private float alphaInvisible;
    [SerializeField] private float duration;
    [SerializeField] private float delay;
    [SerializeField] private bool startVisible;
    private RectTransform rt;

    private void Start()
    {
        rt = GetComponent<RectTransform>();

        SetVisibility(startVisible);
    }

    public void SetVisibility(bool visible)
    {
        print((rt, visible ? alphaVisible : alphaInvisible, duration));
        LeanTween.alpha(rt, visible ? alphaVisible : alphaInvisible, duration)
            .setDelay(delay);
    }
}
