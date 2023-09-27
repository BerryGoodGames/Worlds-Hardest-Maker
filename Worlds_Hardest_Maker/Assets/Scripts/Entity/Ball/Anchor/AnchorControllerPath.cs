using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using UnityEngine;

public partial class AnchorController
{
    [Separator("Path settings")] [SerializeField]
    private Transform lineContainer;

    [SerializeField] private Color lineColor;
    [SerializeField] private float lineWeight;

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

        bool[] hasRendered = new bool[Blocks.Count];

        List<(Vector2, Vector2)> lineList = new();

        int loopIndex = -1;
        bool hasLooped = false;
        bool isFirstPositionBlockAfterLoop = false;

        // loop through blocks
        while (currentNode != null)
        {
            AnchorBlock currentBlock = currentNode.Value;

            // handle current block
            ParseBlockForPath(ref currentBlock);

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
            isFirstPositionBlockAfterLoop = true;
        }

        return;

        void ParseBlockForPath(ref AnchorBlock anchorBlock)
        {
            if (anchorBlock is PositionAnchorBlock positionAnchorBlock)
            {
                // add new target to array if MoveBlock or MoveAndRotateBlock
                Vector2 currentVertex = positionAnchorBlock.TargetAbsolute;

                if (ReferenceManager.Instance.MainChainController.Children[index] is not PositionAnchorBlockController
                    controller)
                {
                    throw new("Controller was for some reason not a position block controller, this shouldn't happen");
                }

                // check if line already rendered
                if (!hasRendered[index] || !lineList.Contains((previousVertex, currentVertex)) ||
                    isFirstPositionBlockAfterLoop)
                {
                    AnchorPathLine line = Instantiate(PrefabManager.Instance.AnchorPathLine, Vector2.zero,
                        Quaternion.identity, lineContainer);

                    line.CreateArrowHead(previousVertex, currentVertex);
                    line.CreateArrowLine(previousVertex, currentVertex,
                        positionAnchorBlock.ImplementedBlockType is AnchorBlock.Type.Move
                            or AnchorBlock.Type.MoveAndRotate);
                    line.CreateBlur();

                    controller.Lines.Add(line);

                    lineList.Add((previousVertex, currentVertex));
                    hasRendered[index] = true;
                }

                previousVertex = currentVertex;

                isFirstPositionBlockAfterLoop = false;
            }

            // track loop index if LoopBlock
            else if (anchorBlock.ImplementedBlockType is AnchorBlock.Type.Loop)
            {
                // track loop index
                loopIndex = index;
            }
        }
    }

    private void ClearLines()
    {
        // clear references in blocks
        foreach (AnchorBlock anchorBlock in Blocks)
        {
            if (anchorBlock is not PositionAnchorBlock positionAnchorBlock) continue;

            PositionAnchorBlockController controller = positionAnchorBlock.Controller;

            // kill all glow tweens
            foreach (AnchorPathLine line in controller.Lines)
            {
                line.Blur.DOKill();
            }

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