using System;
using UnityEngine;

public class AnchorBlockQuickMenuController : QuickMenuController
{
    public RectTransform RectTransform;
    public AlphaUITween Tween;

    public AnchorBlockController SelectedAnchorBlock { get; set; } = null;
    

    public void OnClickDelete()
    {
        if (SelectedAnchorBlock == null)
        {
            Debug.LogWarning("Tried to delete anchor block, but none was selected by quick menu");
            return;
        }
        SelectedAnchorBlock.Delete();
    }

    public void OnClickDuplicate()
    {
        if (SelectedAnchorBlock == null)
        {
            Debug.LogWarning("Tried to duplicate anchor block, but none was selected by quick menu");
            return;
        }
        SelectedAnchorBlock.Duplicate();
    }
}