using System;
using System.Collections.Generic;
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

        // loop through blocks
        while (currentNode != null)
        {
            if (visitedIndices[index]) break;

            ParseBlockForPath(ref currentNode, ref index, ref points, ref visitedIndices);
        }

        return points;
    }

    public void ParseBlockForPath(ref LinkedListNode<AnchorBlock> currentNode, ref int index, ref List<Vector2> points,
        ref bool[] visitedIndices)
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


        // jump if GoToBlock
        if (currentBlock.ImplementedBlockType == AnchorBlock.Type.GoTo)
        {
            visitedIndices[index] = true;

            GoToBlock goToBlock = (GoToBlock)currentBlock;
            index = goToBlock.Index;
            currentNode = Blocks.NodeAt(index);
        }
        else
        {
            index++;
            currentNode = currentNode.Next;
        }
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
        }
    }

    public void RenderLines() => RenderLines(GetPath());

    public void SetLinesActive(bool active) => lineContainer.gameObject.SetActive(active);
}