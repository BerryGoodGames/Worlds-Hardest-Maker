using MyBox;
using UnityEngine;

public class AnchorBallDragDrop : BallDragDrop
{
    [AutoProperty] [SerializeField] private AnchorBallController anchorBallController;

    protected override void OnMouseDrag()
    {
        if (!anchorBallController.ParentAnchor.Selected) return;
        base.OnMouseDrag();
    }
}