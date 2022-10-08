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
    [SerializeField] private bool open = false;
    [SerializeField] private float closedX;
    private float openedX;

    public void Toggle()
    {
        panel.DOKill();

        // closed state -> x = closedX
        // opened state -> x = closedX + width
        panel.DOAnchorPosX(open ? closedX : openedX, duration).SetEase(open ? closeEase : openEase);

        open = !open;
    }

    private void Start()
    {
        openedX = closedX + panel.rect.width;
        panel.anchoredPosition = new(open ? openedX : closedX, panel.anchoredPosition.y);
    }
}
