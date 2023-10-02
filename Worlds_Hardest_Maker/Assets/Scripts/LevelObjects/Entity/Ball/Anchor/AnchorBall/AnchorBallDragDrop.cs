using MyBox;
using UnityEngine;

public class AnchorBallDragDrop : EntityDragDrop
{
    [AutoProperty] [SerializeField] private AnchorBallController anchorBallController;

    protected override void OnMouseDrag()
    {
        if (anchorBallController.IsParentAnchorNull)
        {
            if (AnchorManager.Instance.SelectedAnchor == null) base.OnMouseDrag();
        }
        else if (anchorBallController.ParentAnchor.Selected) base.OnMouseDrag();
    }
}