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
    [SerializeField] private bool disableObjectWhenInvisible = false;
    [Range(0, 1)][SerializeField] private float alphaVisible = 1;
    [Range(0, 1)][SerializeField] private float alphaInvisible = 0;

    public Action onSetVisible = null;
    public Action onIsInvisible = null;

    private bool isVisible;
    

    private void TweenVis()
    {
        if (disableObjectWhenInvisible)
        {
            if (image != null) image.gameObject.SetActive(true);
            if (text != null) text.gameObject.SetActive(true);
            if (canvasGroup != null) canvasGroup.gameObject.SetActive(true);
        }

        onSetVisible?.Invoke();

        if (image != null) image.DOFade(alphaVisible, duration);
        if (text != null) text.DOFade(alphaVisible, duration);
        if (canvasGroup != null) canvasGroup.DOFade(alphaVisible, duration);
    }

    private void TweenInvis()
    {
        if (image != null) image.DOFade(alphaInvisible, duration).OnComplete(() =>
        {
            if (disableObjectWhenInvisible) image.gameObject.SetActive(false);
            onIsInvisible?.Invoke();
        });

        if (text != null) text.DOFade(alphaInvisible, duration).OnComplete(() => {
            if (disableObjectWhenInvisible) text.gameObject.SetActive(false);
            onIsInvisible?.Invoke();
        });

        if (canvasGroup != null) canvasGroup.DOFade(alphaInvisible, duration).OnComplete(() => {
            if (disableObjectWhenInvisible) canvasGroup.gameObject.SetActive(false);
            onIsInvisible?.Invoke();
        });
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
        isVisible = startVisible;

        if (image != null)
        {
            if (disableObjectWhenInvisible) image.gameObject.SetActive(startVisible);
            image.color = new(image.color.r, image.color.g, image.color.b, startVisible ? alphaVisible : alphaInvisible);
        }
        if (text != null)
        {
            if (disableObjectWhenInvisible) text.gameObject.SetActive(startVisible);
            text.color = new(image.color.r, image.color.g, image.color.b, startVisible ? alphaVisible : alphaInvisible);
        }
        if (canvasGroup != null)
        {
            if (disableObjectWhenInvisible) canvasGroup.gameObject.SetActive(startVisible);
            canvasGroup.alpha = startVisible ? alphaVisible : alphaInvisible;
        }
    }
}
