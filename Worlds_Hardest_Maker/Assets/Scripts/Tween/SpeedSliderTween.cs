using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SpeedSliderTween : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Image fill;
    [SerializeField] private Image handle;
    [SerializeField] private Text speedTxt;
    [Space] [SerializeField] private float duration;
    [Range(0, 1)] [SerializeField] private float alphaVisible = 1;
    [Range(0, 1)] [SerializeField] private float alphaInvisible;

    public Action OnSetVisible = null;
    public Action OnIsInvisible = null;

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