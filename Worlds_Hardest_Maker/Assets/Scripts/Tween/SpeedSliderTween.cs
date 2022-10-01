using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class SpeedSliderTween : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] Image fill;
    [SerializeField] Image handle;
    [SerializeField] Text speedTxt;
    [Space]
    [SerializeField] private float duration;
    [Range(0, 1)][SerializeField] private float alphaVisible = 1;
    [Range(0, 1)][SerializeField] private float alphaInvisible = 0;

    public Action onSetVisible = null;
    public Action onIsInvisible = null;

    private bool isVisible = true;

    private SpeedSliderAnim animController;

    private void TweenVis()
    {
        animController.Ungone();

        background.DOFade(alphaVisible, duration);
        fill.DOFade(alphaVisible, duration);
        handle.DOFade(alphaVisible, duration);
        speedTxt.DOFade(alphaVisible, duration);
    }

    private void TweenInvis()
    {
        background.DOFade(alphaInvisible, duration).OnComplete(() => animController.Gone());
        fill.DOFade(alphaInvisible, duration).OnComplete(() => animController.Gone());
        handle.DOFade(alphaInvisible, duration).OnComplete(() => animController.Gone());
        speedTxt.DOFade(alphaInvisible, duration).OnComplete(() => animController.Gone());
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
        animController = GetComponent<SpeedSliderAnim>();

        background.color = new(background.color.r, background.color.g, background.color.b, alphaInvisible);
        fill.color = new(fill.color.r, fill.color.g, fill.color.b, alphaInvisible);
        handle.color = new(handle.color.r, handle.color.g, handle.color.b, alphaInvisible);
        speedTxt.color = new(speedTxt.color.r, speedTxt.color.g, speedTxt.color.b, alphaInvisible);
    }
}
