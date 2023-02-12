using DG.Tweening;
using UnityEngine;

public class PanelTween : MonoBehaviour
{
    [SerializeField] private RectTransform panel;
    [Space] [SerializeField] private AnimationCurve openEase;
    [SerializeField] private AnimationCurve closeEase;
    [Space] [SerializeField] private float duration;
    public bool open;
    [SerializeField] private bool closesToRight;
    private float closedX;
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
        openedX = 0;
        closedX = (closesToRight ? 1 : -1) * panel.rect.width;
        
        panel.anchoredPosition = new(open ? openedX : closedX, panel.anchoredPosition.y);
    }
}