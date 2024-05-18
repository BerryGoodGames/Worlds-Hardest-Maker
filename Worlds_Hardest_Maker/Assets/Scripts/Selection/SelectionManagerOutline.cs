using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public partial class SelectionManager
{
    public void InitSelectionOutline(Vector2 start)
    {
        // reset selection marking
        if (selectionOutline != null) Destroy(selectionOutline);
        
        // set selection start and end
        SelectionStart = start;
        SelectionEnd = start;
        
        // set new outline
        DrawManager.SetWeight(0.1f);
        DrawManager.SetFill(Color.black);
        
        DrawManager.SetLayerID(DrawManager.DefaultLayerID);
        DrawManager.SetOrderInLayer(0);
        selectionOutline = DrawManager.DrawRect(
            start.x + 0.5f,
            start.y + 0.5f,
            -1,
            -1,
            false, ReferenceManager.Instance.SelectionOutlineContainer
        ).gameObject;
        
        selectionOutlineAnim = selectionOutline.AddComponent<LineAnimator>();
    }
    
    public void AnimSelectionOutline(Vector2 start, Vector2 end)
    {
        if (selectionOutlineAnim == null) return;
        
        // set selection start and end
        SelectionStart = start;
        SelectionEnd = end;
        
        // get position and stuff
        float width = end.x - start.x;
        float height = end.y - start.y;
        
        float x = width > 0 ? start.x - 0.5f : start.x + 0.5f;
        float y = height > 0 ? start.y - 0.5f : start.y + 0.5f;
        float w = width > 0 ? width + 1 : width - 1;
        float h = height > 0 ? height + 1 : height - 1;
        
        List<Vector2> lineVertices = new(
            new Vector2[]
            {
                new(x, y),
                new(x + w, y),
                new(x + w, y + h),
                new(x, y + h),
                new(x, y),
            }
        );
        
        selectionOutlineAnim.AnimateAllPoints(lineVertices, .1f, Ease.OutSine);
    }
}
