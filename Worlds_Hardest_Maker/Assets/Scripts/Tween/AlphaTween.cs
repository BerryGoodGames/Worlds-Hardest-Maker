using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlphaTween : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text text;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [Space] [SerializeField] private bool startVisible = true;
    [SerializeField] private float duration;
    [SerializeField] private bool disableObjectWhenInvisible;
    [Range(0, 1)] [SerializeField] private float alphaVisible = 1;
    [Range(0, 1)] [SerializeField] private float alphaInvisible;

    public event Action OnSetVisible;
    public event Action OnIsInvisible;

    public bool IsVisible { get; private set; }

    private Tween TweenVis()
    {
        if (disableObjectWhenInvisible)
        {
            if (image != null) image.gameObject.SetActive(true);
            if (text != null) text.gameObject.SetActive(true);
            if (canvasGroup != null) canvasGroup.gameObject.SetActive(true);
            if (spriteRenderer != null) spriteRenderer.gameObject.SetActive(true);
        }

        OnSetVisible?.Invoke();

        if (image != null) return image.DOFade(alphaVisible, duration);
        if (text != null) return text.DOFade(alphaVisible, duration);
        if (canvasGroup != null) return canvasGroup.DOFade(alphaVisible, duration);
        if (spriteRenderer != null) return spriteRenderer.DOFade(alphaVisible, duration);

        return null;
    }

    private Tween TweenInvis()
    {
        if (image != null)
        {
            return image.DOFade(alphaInvisible, duration).OnComplete(
                () =>
                {
                    if (IsVisible) return;
                    if (disableObjectWhenInvisible) image.gameObject.SetActive(false);
                    OnIsInvisible?.Invoke();
                }
            );
        }

        if (text != null)
        {
            return text.DOFade(alphaInvisible, duration).OnComplete(
                () =>
                {
                    if (IsVisible) return;
                    if (disableObjectWhenInvisible) text.gameObject.SetActive(false);
                    OnIsInvisible?.Invoke();
                }
            );
        }

        if (canvasGroup != null)
        {
            return canvasGroup.DOFade(alphaInvisible, duration).OnComplete(
                () =>
                {
                    if (IsVisible) return;
                    if (disableObjectWhenInvisible) canvasGroup.gameObject.SetActive(false);
                    OnIsInvisible?.Invoke();
                }
            );
        }

        if (spriteRenderer != null)
        {
            return spriteRenderer.DOFade(alphaInvisible, duration).OnComplete(
                () =>
                {
                    if (IsVisible) return;
                    if (disableObjectWhenInvisible) spriteRenderer.gameObject.SetActive(false);
                    OnIsInvisible?.Invoke();
                }
            );
        }

        return null;
    }

    public Tween SetVisible(bool vis)
    {
        Tween tween = null;
        if (IsVisible && !vis)
            // the frame setting to invisible
            tween = TweenInvis();

        if (!IsVisible && vis)
            // the frame setting to visible
            tween = TweenVis();

        IsVisible = vis;

        return tween;
    }

    // method compatible with buttons
    public void SetVisibleRaw(bool vis) => SetVisible(vis);

    private void Awake()
    {
        IsVisible = startVisible;

        if (image != null)
        {
            if (disableObjectWhenInvisible) image.gameObject.SetActive(startVisible);
            image.color = new(
                image.color.r, image.color.g, image.color.b,
                startVisible ? alphaVisible : alphaInvisible
            );
        }

        if (text != null)
        {
            if (disableObjectWhenInvisible) text.gameObject.SetActive(startVisible);
            text.color = new(text.color.r, text.color.g, text.color.b, startVisible ? alphaVisible : alphaInvisible);
        }

        if (canvasGroup != null)
        {
            if (disableObjectWhenInvisible) canvasGroup.gameObject.SetActive(startVisible);
            canvasGroup.alpha = startVisible ? alphaVisible : alphaInvisible;
        }

        if (spriteRenderer != null)
        {
            if (disableObjectWhenInvisible) spriteRenderer.gameObject.SetActive(startVisible);
            Color color = spriteRenderer.color;
            spriteRenderer.color = new(color.r, color.g, color.b, startVisible ? alphaVisible : alphaInvisible);
        }
    }

    private void Start()
    {
        if (!startVisible) OnIsInvisible?.Invoke();
    }

    private void OnDestroy()
    {
        image.DOKill();
        text.DOKill();
        canvasGroup.DOKill();
        spriteRenderer.DOKill();
    }
}