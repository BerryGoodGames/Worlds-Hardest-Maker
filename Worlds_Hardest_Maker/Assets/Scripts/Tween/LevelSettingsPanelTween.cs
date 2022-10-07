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

    public void Toggle()
    {
        panel.DOKill();

        // closed state -> x = closedX
        // opened state -> x = closedX + width
        panel.DOAnchorPosX(open ? closedX : closedX + panel.rect.width, duration).SetEase(open ? closeEase : openEase);

        open = !open;
    }
}
