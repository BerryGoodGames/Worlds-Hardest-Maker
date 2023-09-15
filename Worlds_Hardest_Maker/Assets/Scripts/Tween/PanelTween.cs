using DG.Tweening;
using MyBox;
using UnityEngine;

public class PanelTween : MonoBehaviour
{
    ////////////////////////////////////
    // DO NOT USE ANY METHODS IN CODE //
    // USE PANEL MANAGER INSTEAD      //
    ////////////////////////////////////

    [SerializeField] private RectTransform panel;

    [Space] [SerializeField] private AnimationCurve openEase;
    [SerializeField] private AnimationCurve closeEase;

    [Space] [SerializeField] private float duration;

    [field: SerializeField]
    [field: ReadOnly]
    public bool Open { get; private set; }

    [SerializeField] private bool closesToRight;
    private float closedX;
    private float openedX;

    public void SetOpen(bool open, bool noAnimation = false)
    {
        // if (Open == open) return;

        panel.DOKill();

        // closed state -> x = closedX
        // opened state -> x = closedX + width = openedX
        if (noAnimation)
        {
            panel.anchoredPosition = new(open ? openedX : closedX, panel.anchoredPosition.y);
        }
        else
        {
            panel.DOAnchorPosX(open ? openedX : closedX, duration).SetEase(Open ? closeEase : openEase);
        }

        Open = open;
    }

    public void ToggleOpen(bool noAnimation = false) => SetOpen(!Open, noAnimation);

    private void Awake()
    {
        openedX = 0;
        closedX = (closesToRight ? 1 : -1) * panel.rect.width;
    }
}