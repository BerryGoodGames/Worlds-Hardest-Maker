using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class PanelTween : MonoBehaviour
{
    [SerializeField] private RectTransform panel;
    [Space] [SerializeField] private AnimationCurve openEase;
    [SerializeField] private AnimationCurve closeEase;
    [Space] [SerializeField] private float duration;
    [FormerlySerializedAs("open")] public bool Open;
    [SerializeField] private bool closesToRight;
    private float closedX;
    private float openedX;

    public void Toggle()
    {
        panel.DOKill();

        // closed state -> x = closedX
        // opened state -> x = closedX + width
        panel.DOAnchorPosX(Open ? closedX : openedX, duration).SetEase(Open ? closeEase : openEase);

        Open = !Open;
    }

    public void Set(bool open)
    {
        if (Open != open) Toggle();
    }

    private void Start()
    {
        openedX = 0;
        closedX = (closesToRight ? 1 : -1) * panel.rect.width;

        panel.anchoredPosition = new(Open ? openedX : closedX, panel.anchoredPosition.y);
    }
}