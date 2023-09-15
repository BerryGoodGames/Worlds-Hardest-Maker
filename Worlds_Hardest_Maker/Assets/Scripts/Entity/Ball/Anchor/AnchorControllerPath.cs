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
        print("Path update");
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

                if (ReferenceManager.Instance.MainChainController.Children[index] is PositionAnchorBlockController controller)
                {
                    AnchorPathLine line = Instantiate(PrefabManager.Instance.AnchorPathLine, Vector2.zero,
                        Quaternion.identity, lineContainer);

                    line.CreateArrowHead(previousVertex, currentVertex);
                    line.CreateArrowLine(previousVertex, currentVertex, currentBlock.ImplementedBlockType is AnchorBlock.Type.Move or AnchorBlock.Type.MoveAndRotate);
                    line.CreateBlur();

                    controller.Lines.Add(line);
                }
                else throw new("controller was for some reason not a position block controller, this shouldn't happen");

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
        // clear references in blocks
        foreach (AnchorBlock anchorBlock in Blocks)
        {
            if (anchorBlock is not PositionAnchorBlock positionAnchorBlock) continue;

            PositionAnchorBlockController controller = positionAnchorBlock.Controller;
            controller.Lines.Clear();
        }

        // clear lines
        foreach (Transform line in lineContainer)
        {
            Destroy(line.gameObject);
        }
    }

    public void SetLinesActive(bool active) => lineContainer.gameObject.SetActive(active);
}