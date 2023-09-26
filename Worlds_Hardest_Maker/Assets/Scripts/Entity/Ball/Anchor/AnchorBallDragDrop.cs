using MyBox;
using UnityEngine;

public class AnchorBallDragDrop : EntityDragDrop
{
    [AutoProperty] [SerializeField] private AnchorBallController anchorBallController;

    protected override void OnMouseDrag()
    {
        if (!anchorBallController.ParentAnchor || !anchorBallController.ParentAnchor.Selected) return;
        base.OnMouseDrag();
    }
}