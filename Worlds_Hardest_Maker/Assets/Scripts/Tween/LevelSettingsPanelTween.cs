using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelSettingsPanelTween : MonoBehaviour
{
    [SerializeField] private RectTransform panel;
    [Space]
    [SerializeField] private AnimationCurve openEase;
    [SerializeField] private AnimationCurve closeEase;
    [Space]
    [SerializeField] private float duration;
    private float closedX;
    private float width;
    private bool open = false;

    public void Toggle()
    {
        panel.DOKill();
        if (open)
        {
            // close
            // closed state -> x = closedX
            panel.DOAnchorPosX(closedX, duration).SetEase(closeEase);
        }
        else
        {
            // open
            // opened state -> x = closedX + width
            panel.DOAnchorPosX(closedX + width, duration).SetEase(openEase);
        }
        open = !open;
    }

    private void Start()
    {
        closedX = panel.anchoredPosition.x;
        width = panel.rect.width;
    }
}
