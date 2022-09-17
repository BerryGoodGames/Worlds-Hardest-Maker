using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AlphaUITween : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMPro.TMP_Text text;
    [Space]
    [SerializeField] private float duration;
    [Range(0, 1)][SerializeField] private float alphaVisible;
    [Range(0, 1)][SerializeField] private float alphaInvisible;
    private bool isVisible = true;
    

    private void TweenVis()
    {
        if (image != null) image.DOFade(alphaVisible, duration);
        if (text != null) text.DOFade(alphaVisible, duration);
    }

    private void TweenInvis()
    {
        if (image != null) image.DOFade(alphaInvisible, duration);
        if (text != null) text.DOFade(alphaInvisible, duration);
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

    private void Start()
    {
        if (isVisible) TweenVis();
        else TweenInvis();
    }
}
