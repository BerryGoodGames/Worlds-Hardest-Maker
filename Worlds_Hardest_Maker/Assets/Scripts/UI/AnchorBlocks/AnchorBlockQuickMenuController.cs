using System;
using MyBox;
using UnityEngine;

[RequireComponent(typeof(AlphaTween))]
public class AnchorBlockQuickMenuController : QuickMenuController
{
    [ReadOnly] [SerializeField] private AnchorBlockController selectedAnchorBlock;

    private AlphaTween tween;

    public void OnClickDelete()
    {
        if (selectedAnchorBlock == null)
        {
            Debug.LogWarning("Tried to delete anchor block, but none was selected by quick menu");
            return;
        }

        if (selectedAnchorBlock.IsLocked) return;

        selectedAnchorBlock.Delete();
    }

    public void OnClickDuplicate()
    {
        if (selectedAnchorBlock == null)
        {
            Debug.LogWarning("Tried to duplicate anchor block, but none was selected by quick menu");
            return;
        }

        selectedAnchorBlock.Duplicate();
    }

    public void Activate(AnchorBlockController anchorBlock)
    {
        if(tween == null) tween = GetComponent<AlphaTween>();
        
        // open and position quick menu
        Vector2 mousePos = MouseManager.Instance.MouseCanvasPos;
        mousePos.y = MouseManager.Instance.MouseCanvasPos.y - GameManager.GetCanvasDimensions().y;
        
        selectedAnchorBlock = anchorBlock;
        ((RectTransform)transform).anchoredPosition = mousePos;
        tween.SetVisible(true);
    }
}