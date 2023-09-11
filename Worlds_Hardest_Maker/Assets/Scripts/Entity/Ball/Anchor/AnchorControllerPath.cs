using System;
using System.Collections.Generic;
using LuLib.Vector;
using UnityEngine;

public partial class AnchorController
{
    [Space] [SerializeField] private Transform lineContainer;
    [SerializeField] private Color lineColor;
    [SerializeField] private float lineWeight;

    public List<Vector2> RenderLines()
    {
        // clear lines
        foreach (Transform line in lineContainer)
        {
            Destroy(line.gameObject);
        }
        DrawManager.SetFill(lineColor);
        DrawManager.SetLayerID(spriteRenderer.sortingLayerID);
        DrawManager.SetOrderInLayer(spriteRenderer.sortingOrder - 1);
        DrawManager.SetRoundedCorners(true);
        DrawManager.SetWeight(lineWeight);

        List<Vector2> points = new() { transform.position };

        int index = 0;
        LinkedListNode<AnchorBlock> currentNode = Blocks.First;

        int loopIndex = -1;
        bool hasLooped = false;

        // loop through blocks
        while (true)
        {
            if (currentNode == null)
            {
                // arrived at end of chain
                // if loop index existent -> jump to loop index
                // only jump once to avoid recursion (if we already have looped, cancel jump)
                if (loopIndex == -1 || hasLooped) break;

                index = loopIndex;
                currentNode = Blocks.NodeAt(index);
                hasLooped = true;
                continue;
            }

            AnchorBlock currentBlock = currentNode.Value;

            // add new target to array if MoveBlock or MoveAndRotateBlock
            if (currentBlock.ImplementedBlockType is AnchorBlock.Type.Move or AnchorBlock.Type.MoveAndRotate or AnchorBlock.Type.Teleport)
            {
                Vector2 target = ((PositionAnchorBlock)currentBlock).Target;

                points.Add(target);

                int i = points.Count - 1;

                Vector2 currentVertex = points[i];
                Vector2 previousVertex = points[i - 1];

                if (currentBlock.ImplementedBlockType is AnchorBlock.Type.Move or AnchorBlock.Type.MoveAndRotate)
                {
                    DrawManager.DrawLine(previousVertex, currentVertex, lineContainer);
                }
                else
                {
                    DrawManager.DrawDashedLine(previousVertex, currentVertex, 0.2f, 0.2f, lineContainer);
                }

                // draw arrow head
                const float headLineLength = 0.15f;
                Vector2 delta = currentVertex - previousVertex;
                Vector2 halfPoint = previousVertex + delta / 2;
                Vector2 offset = delta.normalized * (headLineLength / 2);
                Vector2 start = halfPoint + offset;
                Vector2 endSideOffset = delta.normalized * Mathf.Sin(headLineLength);
                endSideOffset.Rotate(90);
                Vector2 end = halfPoint - offset;

                DrawManager.DrawLine(start, end + endSideOffset, lineContainer);
                DrawManager.DrawLine(start, end - endSideOffset, lineContainer);
            }


            // track loop index if LoopBlock
            if (currentBlock.ImplementedBlockType is AnchorBlock.Type.Loop)
            {
                // track loop index
                loopIndex = index;
            }

            index++;
            currentNode = currentNode.Next;
        }

        return points;
    }

    // public void RenderLines(List<Vector2> points)
    // {
    //     
    //
    //     
    //
    //     // new line for each vertex (without i = 0)
    //     for (int i = 1; i < points.Count; i++)
    //     {
    //         Vector2 currentVertex = points[i];
    //         Vector2 previousVertex = points[i - 1];
    //         
    //         DrawManager.DrawLine(previousVertex, currentVertex, lineContainer);
    //
    //         // draw arrow head
    //         const float headLineLength = 0.15f;
    //         Vector2 delta = currentVertex - previousVertex;
    //         Vector2 halfPoint = previousVertex + delta / 2;
    //         Vector2 offset = delta.normalized * (headLineLength / 2);
    //         Vector2 start = halfPoint + offset;
    //         Vector2 endSideOffset = delta.normalized * Mathf.Sin(headLineLength);
    //         endSideOffset.Rotate(90);
    //         Vector2 end = halfPoint - offset;
    //
    //         DrawManager.DrawLine(start, end + endSideOffset, lineContainer);
    //         DrawManager.DrawLine(start, end - endSideOffset, lineContainer);
    //     }
    // }

    // public void RenderLines() => RenderLines(GetPath());

    public void SetLinesActive(bool active) => lineContainer.gameObject.SetActive(active);
}