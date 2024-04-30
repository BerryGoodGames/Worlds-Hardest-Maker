using MyBox;
using UnityEngine;
using UnityEngine.Serialization;

public class BallDragDrop : EntityDragDrop
{
    [FormerlySerializedAs("anchorBallController")] [AutoProperty] [SerializeField] private BallController ballController;

    protected override void OnMouseDrag()
    {
        if (LevelSessionEditManager.Instance.Playing || !KeyBinds.GetKeyBind("Editor_MoveEntity")) return;

        if (ballController.IsParentAnchorNull)
        {
            if (AnchorManager.Instance.SelectedAnchor == null) base.OnMouseDrag();
        }
        else if (ballController.ParentAnchor.Selected) { base.OnMouseDrag(); }

        ballController.StartPosition = transform.localPosition;
    }
}