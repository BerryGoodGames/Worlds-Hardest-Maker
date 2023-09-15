using System;
using System.Collections.Generic;
using LuLib.Vector;
using MyBox;
using UnityEngine;

public partial class AnchorController
{
    [Separator("Path settings")] [SerializeField] private Transform lineContainer;
    [SerializeField] private Color lineColor;
    [SerializeField] private float lineWeight;
    [SerializeField] private float dashedLineWidth;
    [SerializeField] private float dashedLineSpacing;
    [SerializeField] private float lineAnimationDuration;

    public void RenderLines()
    {
        ClearLines();

        // line settings
        DrawManager.SetFill(lineColor);
        DrawManager.SetLayerID(spriteRenderer.sortingLayerID);
        DrawManager.SetOrderInLayer(spriteRenderer.sortingOrder - 1);
        DrawManager.SetRoundedCorners(true);
        DrawManager.SetWeight(lineWeight);

        Vector2 previousVertex = transform.position;

        int index = 0;
        LinkedListNode<AnchorBlock> currentNode = Blocks.First;

        int loopIndex = -1;
        bool hasLooped = false;

        // loop through blocks
        while (currentNode != null)
        {
            AnchorBlock currentBlock = currentNode.Value;

            // handle current block
            ParseBlockForPath(ref currentBlock, ref previousVertex, ref loopIndex, ref index);

            // increment
            index++;
            currentNode = currentNode.Next;

            // If currentNode reaches the end, and we haven't looped, jump to loopIndex
            if (currentNode != null) continue;

            if (loopIndex == -1 || hasLooped)
            {
                break;
            }

            index = loopIndex;
            currentNode = Blocks.NodeAt(index);
            hasLooped = true;
        }
    }

    private void ParseBlockForPath(ref AnchorBlock currentBlock, ref Vector2 previousVertex, ref int loopIndex, ref int index)
    {
        switch (currentBlock.ImplementedBlockType)
        {
            // add new target to array if MoveBlock or MoveAndRotateBlock
            case AnchorBlock.Type.Move or AnchorBlock.Type.MoveAndRotate or AnchorBlock.Type.Teleport:
            {
                Vector2 currentVertex = ((PositionAnchorBlock)currentBlock).Target;

                if (ReferenceManager.Instance.MainChainController.Children[index] is PositionAnchorBlockController
                    controller)
                {
                    if (currentBlock.ImplementedBlockType is AnchorBlock.Type.Move or AnchorBlock.Type.MoveAndRotate)
                    {

                        controller.Line = DrawManager.DrawLine(previousVertex, currentVertex, lineContainer);
                    }
                    else
                    {
                        controller.Line = DrawManager.DrawDashedLine(previousVertex, currentVertex, 0.2f, 0.2f, true, lineContainer);
                    }
                }
                else
                {
                    DrawManager.DrawDashedLine(previousVertex, currentVertex, dashedLineWidth, dashedLineSpacing, lineContainer);
                }
                else throw new("controller was for some reason not a position block controller, this shouldn't happen");
                

                DrawArrowHead(currentVertex, previousVertex);

                previousVertex = currentVertex;
                break;
            }

            // track loop index if LoopBlock
            case AnchorBlock.Type.Loop:
                // track loop index
                loopIndex = index;
                break;
        }
    }

    private void ClearLines()
    {
        // clear lines
        foreach (Transform line in lineContainer)
        {
            Destroy(line.gameObject);
        }
    }

    private List<LineRenderer> DrawArrowHead(Vector2 currentVertex, Vector2 previousVertex)
    {
        (Vector2 arrowVertex1, Vector2 arrowVertex2, Vector2 arrowCenter) = GetArrowHeadPoints(currentVertex, previousVertex);

        return new()
        {
            DrawManager.DrawLine(arrowCenter, arrowVertex1, lineContainer),
            DrawManager.DrawLine(arrowCenter, arrowVertex2, lineContainer)
        };
    }

    private (Vector2 arrowVertex1, Vector2 arrowVertex2, Vector2 arrowCenter) GetArrowHeadPoints(Vector2 currentVertex, Vector2 previousVertex)
    {
        const float headLineLength = 0.15f;
        Vector2 delta = currentVertex - previousVertex;
        Vector2 halfPoint = previousVertex + delta / 2;
        Vector2 offset = delta.normalized * (headLineLength / 2);
        Vector2 start = halfPoint + offset;
        Vector2 endSideOffset = delta.normalized * Mathf.Sin(headLineLength);
        endSideOffset.Rotate(90);
        Vector2 end = halfPoint - offset;

        Vector2 arrowVertex1 = end + endSideOffset;
        Vector2 arrowVertex2 = end - endSideOffset;
        Vector2 arrowCenter = start;

        return (arrowVertex1, arrowVertex2, arrowCenter);
    }

    public void SetLinesActive(bool active) => lineContainer.gameObject.SetActive(active);
}