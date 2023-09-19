using MyBox;
using UnityEngine;

public class AnchorBlockQuickMenuController : QuickMenuController
{
    public RectTransform RectTransform;
    public AlphaTween Tween;

    [Space] [ReadOnly] public AnchorBlockController SelectedAnchorBlock;

    public void OnClickDelete()
    {
        if (SelectedAnchorBlock == null)
        {
            Debug.LogWarning("Tried to delete anchor block, but none was selected by quick menu");
            return;
        }

        if (SelectedAnchorBlock.IsLocked) return;

        SelectedAnchorBlock.Delete();
        ReferenceManager.Instance.AnchorBlockFitter.UpdateChildrenArray();
    }

    public void OnClickDuplicate()
    {
        if (SelectedAnchorBlock == null)
        {
            Debug.LogWarning("Tried to duplicate anchor block, but none was selected by quick menu");
            return;
        }

        SelectedAnchorBlock.Duplicate();
        ReferenceManager.Instance.AnchorBlockFitter.UpdateChildrenArray();
    }
}