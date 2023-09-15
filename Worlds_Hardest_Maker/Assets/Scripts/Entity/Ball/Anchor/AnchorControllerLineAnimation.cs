using System;
using System.Collections.Generic;
using UnityEngine;

public partial class AnchorController
{



    // private LineAnimator previewLine;
    // private LineAnimator previewArrow1;
    // private LineAnimator previewArrow2;

    // public void ActivatePreview(Vector2 start, bool dashed)
    // {
    //     if (previewLine != null) Destroy(previewLine.gameObject);
    //     if (previewArrow1 != null) Destroy(previewArrow1.gameObject);
    //     if (previewArrow2 != null) Destroy(previewArrow2.gameObject);
    //
    //     Vector2 end = Input.mousePosition;
    //
    //     // instantiate lines
    //     previewLine =
    //         dashed ?
    //             DrawManager.DrawDashedLine(start, end, dashedLineWidth, dashedLineSpacing, lineContainer).GetComponent<LineAnimator>() :
    //             DrawManager.DrawLine(start, end, lineContainer).GetComponent<LineAnimator>();
    //
    //     List<LineRenderer> arrowLines = DrawArrowHead(start, end);
    //     previewArrow1 = arrowLines[0].GetComponent<LineAnimator>();
    //     previewArrow2 = arrowLines[1].GetComponent<LineAnimator>();
    // }
    //
    // public void UpdatePreview(Vector2 end)
    // {
    //     (Vector2 arrowVertex1, Vector2 arrowVertex2, Vector2 arrowCenter) = GetArrowHeadPoints(end, previewLine.LineRenderer.GetPosition(0));
    //
    //     previewLine.AnimatePoint(1, end, lineAnimationDuration);
    //     previewArrow1.AnimateAllPoints(new() { arrowCenter, arrowVertex1 }, lineAnimationDuration);
    //     previewArrow2.AnimateAllPoints(new() { arrowCenter, arrowVertex2 }, lineAnimationDuration);
    // }


    // public Vector2 GetLinePreviewStartVertex(AnchorBlockController anchorBlockUI)
    // {
    //     // gets start vertex of line preview of position input block at index i
    //     List<AnchorBlock> anchorBlocks = ReferenceManager.Instance.MainChainController.GetAnchorBlocks(this);
    //
    //
    //     int j = 0;
    //     for (int i = 0; i < anchorBlocks.Count; i++)
    //     {
    //         AnchorBlock anchorBlock = anchorBlocks[i];
    //         if (anchorBlock is not PositionAnchorBlock positionBlock) continue;
    //
    //         if (j == 0 && anchorBlockUI.Block == anchorBlock) return Vector2.zero;
    //
    //         if (anchorBlocks[i + 1] == anchorBlockUI.Block)
    //         {
    //             // arrived at position anchor block which holds start of i position anchor block
    //             return positionBlock.Target;
    //         }
    //
    //         j++;
    //     }
    //
    //     throw new Exception($"Could not find position anchor block {anchorBlockUI}");
    // }
}
