using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlphaUITween : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text text;
    [SerializeField] private CanvasGroup canvasGroup;
    [Space] [SerializeField] private readonly bool startVisible = true;
    [SerializeField] private float duration;
    [SerializeField] private bool disableObjectWhenInvisible;
    [Range(0, 1)] [SerializeField] private readonly float alphaVisible = 1;
    [Range(0, 1)] [SerializeField] private float alphaInvisible;

    public Action OnSetVisible = null;
    public Action OnIsInvisible = null;

    public bool IsVisible { get; private set; }

    private void TweenVis()
    {
        if (disableObjectWhenInvisible)
        {
            if (image != null) image.gameObject.SetActive(true);
            if (text != null) text.gameObject.SetActive(true);
            if (canvasGroup != null) canvasGroup.gameObject.SetActive(true);
        }

        OnSetVisible?.Invoke();

        if (image != null) image.DOFade(alphaVisible, duration);
        if (text != null) text.DOFade(alphaVisible, duration);
        if (canvasGroup != null) canvasGroup.DOFade(alphaVisible, duration);
    }

    private void TweenInvis()
    {
        if (image != null)
            image.DOFade(alphaInvisible, duration).OnComplete(() =>
            {
                if (IsVisible) return;
                if (disableObjectWhenInvisible) image.gameObject.SetActive(false);
                OnIsInvisible?.Invoke();
            });

        if (text != null)
            text.DOFade(alphaInvisible, duration).OnComplete(() =>
            {
                if (IsVisible) return;
                if (disableObjectWhenInvisible) text.gameObject.SetActive(false);
                OnIsInvisible?.Invoke();
            });

        if (canvasGroup != null)
            canvasGroup.DOFade(alphaInvisible, duration).OnComplete(() =>
            {
                if (IsVisible) return;
                if (disableObjectWhenInvisible) canvasGroup.gameObject.SetActive(false);
                OnIsInvisible?.Invoke();
            });
    }

    public void SetVisible(bool vis)
    {
        if (IsVisible && !vis)
            // the frame setting to invisible
            TweenInvis();

        if (!IsVisible && vis)
            // the frame setting to visible
            TweenVis();

        IsVisible = vis;
    }

    private void Start()
    {
        IsVisible = startVisible;

        if (image != null)
        {
            if (disableObjectWhenInvisible) image.gameObject.SetActive(startVisible);
            image.color = new(image.color.r, image.color.g, image.color.b,
                startVisible ? alphaVisible : alphaInvisible);
        }

        if (text != null)
        {
            if (disableObjectWhenInvisible) text.gameObject.SetActive(startVisible);
            text.color = new(image.color.r, image.color.g, image.color.b, startVisible ? alphaVisible : alphaInvisible);
        }

        if (canvasGroup == null) return;

        if (disableObjectWhenInvisible) canvasGroup.gameObject.SetActive(startVisible);
        canvasGroup.alpha = startVisible ? alphaVisible : alphaInvisible;
    }
}