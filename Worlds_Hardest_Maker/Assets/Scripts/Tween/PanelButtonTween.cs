using DG.Tweening;
using UnityEngine;

public class PanelButtonTween : MonoBehaviour
{
    [SerializeField] private RectTransform button;
    [Space][SerializeField] private AnimationCurve openEase;
    [SerializeField] private AnimationCurve closeEase;
    [Space][SerializeField] private float duration;
    public bool open;
    [SerializeField] private bool closesToRight;
    private float closedX;
    private float openedX;

    public void Toggle()
    {
        button.DOKill();

        // closed state -> x = closedX
        // opened state -> x = closedX + width
        button.DOAnchorPosX(open ? closedX : openedX, duration).SetEase(open ? closeEase : openEase);

        open = !open;
    }

    public void Set(bool open)
    {
        if(this.open != open) Toggle();
    }

    private void Start()
    {
        openedX = 0;
        closedX = (closesToRight ? 1 : -1) * button.rect.width;

        button.anchoredPosition = new(open ? openedX : closedX, button.anchoredPosition.y);
    }
}