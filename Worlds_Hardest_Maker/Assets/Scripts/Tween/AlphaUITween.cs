using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class AlphaUITween : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMPro.TMP_Text text;
    [SerializeField] private CanvasGroup canvasGroup;
    [Space]
    [SerializeField] private bool startVisible = true;
    [SerializeField] private float duration;
    [Range(0, 1)][SerializeField] private float alphaVisible = 1;
    [Range(0, 1)][SerializeField] private float alphaInvisible = 0;

    public Action onSetVisible = null;
    public Action onIsInvisible = null;

    private bool isVisible = true;
    

    private void TweenVis()
    {
        onSetVisible?.Invoke();

        if (image != null) image.DOFade(alphaVisible, duration);
        if (text != null) text.DOFade(alphaVisible, duration);
        if (canvasGroup != null) canvasGroup.DOFade(alphaVisible, duration);
    }

    private void TweenInvis()
    {
        if (image != null) image.DOFade(alphaInvisible, duration).OnComplete(() => onIsInvisible?.Invoke());
        if (text != null) text.DOFade(alphaInvisible, duration).OnComplete(() => onIsInvisible?.Invoke());
        if (canvasGroup != null) canvasGroup.DOFade(alphaInvisible, duration).OnComplete(() => onIsInvisible?.Invoke());
    }

    public void SetVisible(bool vis)
    {
        if (isVisible && !vis)
        {
            // the frame setting to invisible
            TweenInvis();
        }

        if (!isVisible && vis)
        {
            // the frame setting to visible
            TweenVis();
        }

        isVisible = vis;
    }

    public bool IsVisible()
    {
        return isVisible;
    }

    private void Start()
    {
        if (image != null) image.color = new(image.color.r, image.color.g, image.color.b, startVisible ? alphaVisible : alphaInvisible);
        if (text != null) text.color = new(image.color.r, image.color.g, image.color.b, startVisible ? alphaVisible : alphaInvisible);
        if (canvasGroup != null) canvasGroup.alpha = startVisible ? alphaVisible : alphaInvisible;
    }
}
