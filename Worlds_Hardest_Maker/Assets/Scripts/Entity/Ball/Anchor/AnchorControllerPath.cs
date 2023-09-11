using System;
using System.Collections.Generic;
using LuLib.Vector;
using UnityEngine;

public partial class AnchorController
{
    [Space] [SerializeField] private Transform lineContainer;
    [SerializeField] private Color lineColor;
    [SerializeField] private float lineWeight;

    private List<Vector2> GetPath()
    {
        List<Vector2> points = new();

        bool[] visitedIndices = new bool[Blocks.Count];

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

            if (visitedIndices[index]) break;

            ParseBlockForPath(ref currentNode, ref index, ref points, ref visitedIndices, ref loopIndex);
        }

        return points;
    }

    public void ParseBlockForPath(ref LinkedListNode<AnchorBlock> currentNode, ref int index, ref List<Vector2> points,
        ref bool[] visitedIndices, ref int loopIndex)
    {
        AnchorBlock currentBlock = currentNode.Value;

        // add new target to array if MoveBlock or MoveAndRotateBlock
        if (currentBlock.ImplementedBlockType is AnchorBlock.Type.Move or AnchorBlock.Type.MoveAndRotate)
        {
            Vector2 target = currentBlock.ImplementedBlockType switch
            {
                AnchorBlock.Type.Move => ((MoveBlock)currentBlock).Target,
                AnchorBlock.Type.MoveAndRotate => ((MoveAndRotateBlock)currentBlock).Target,
                _ => throw new Exception("Couldn't fetch the target of the anchor block")
            };

            points.Add(target);
        }


        // track loop index if LoopBlock
        if (currentBlock.ImplementedBlockType is AnchorBlock.Type.Loop)
        {
            // visitedIndices[index] = true;

            // LoopBlock loopBlock = (LoopBlock)currentBlock;
            // index = loopBlock.Index;
            // currentNode = Blocks.NodeAt(index);

            // track loop index
            loopIndex = index;
        }

        index++;
        currentNode = currentNode.Next;
    }

    public void RenderLines(List<Vector2> points)
    {
        // add anchor position at start
        points.Insert(0, transform.position);

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

        // new line for each vertex (without i = 0)
        for (int i = 1; i < points.Count; i++)
        {
            Vector2 currentVertex = points[i];
            Vector2 previousVertex = points[i - 1];
            
            DrawManager.DrawLine(previousVertex, currentVertex, lineContainer);

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
    }

    public void RenderLines() => RenderLines(GetPath());

    public void SetLinesActive(bool active) => lineContainer.gameObject.SetActive(active);
}