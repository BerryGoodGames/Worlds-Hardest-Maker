using MyBox;
using UnityEngine;
using VContainer;

public class BallDragDrop : EntityDragDrop
{
    [AutoProperty] [SerializeField] private BallController ballController;

    // TODO: check if injected
    [Inject] private IAnchorManager anchorManager;
    
    protected override void OnMouseDrag()
    {
        if (LevelSessionEditManager.Instance.IsPlaying || !KeyBinds.GetKeyBind("Editor_MoveEntity")) return;
        
        if (ballController.IsParentAnchorNull)
        {
            if (anchorManager.SelectedAnchor == null) base.OnMouseDrag();
        }
        else if (ballController.ParentAnchor.IsSelected) base.OnMouseDrag();
        
        ballController.StartLocalPosition = transform.parent.localPosition;
        ballController.StartWorldPosition = transform.parent.position;
    }
}