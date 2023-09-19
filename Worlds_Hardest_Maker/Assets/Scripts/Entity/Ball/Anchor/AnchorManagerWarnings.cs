using System;
using System.Collections.Generic;
using UnityEngine;

public partial class AnchorManager
{
    public void CheckStackOverflowWarnings()
    {
        // check if there are no blocks with any duration after loop block
        bool loopingDetected = false;
        bool isStackOverflow = true;

        AnchorBlock loopBlock = null;
        CheckWarningsForEach((_, currentNode) =>
        {
            AnchorBlock block = currentNode.Value;

            // detect loop block
            if (block.ImplementedBlockType is AnchorBlock.Type.Loop)
            {
                loopingDetected = true;
                loopBlock = block;

                // stack overflow if no block after loop block
                if (currentNode.Next == null) isStackOverflow = true;
                return false;
            }

            // check if any of following blocks have any duration, if yes then no stack overflow
            if (!loopingDetected || block is not IDurationBlock durationBlock) return false;
            if (!durationBlock.HasCurrentlyDuration()) return false;

            isStackOverflow = false;

            // break the loop
            return true;
        });

        if (loopingDetected)
        {
            AnchorBlockController loopBlockController = loopBlock.Controller;
            loopBlockController.SetWarning(isStackOverflow);
        }
    }

    public void CheckStartRotatingWarnings()
    {
        // check if start rotating block as seconds as time input -> one rotation takes [time input] amount of time
        bool canStartRotateWork = false;
        CheckWarningsForEach((i, currentNode) =>
        {
            AnchorBlock block = currentNode.Value;

            switch (block.ImplementedBlockType)
            {
                case AnchorBlock.Type.SetRotation:
                {
                    // update if start rotating blocks can work
                    SetRotationBlock setRotationBlock = (SetRotationBlock)block;
                    canStartRotateWork = setRotationBlock.GetUnit() != SetRotationBlock.Unit.Time;
                    break;
                }
                // update if it can't rotate
                case AnchorBlock.Type.StartRotating:
                    ReferenceManager.Instance.MainChainController.Children[i].SetWarning(!canStartRotateWork);
                    break;
            }

            return false;
        });
    }

    private void CheckWarningsForEach(Func<int, LinkedListNode<AnchorBlock>, bool> action)
    {
        LinkedListNode<AnchorBlock> currentNode = SelectedAnchor.Blocks.First;

        for (int i = 0; i < SelectedAnchor.Blocks.Count; i++)
        {
            if (currentNode == null)
            {
                Debug.LogWarning("The LinkedListNode should not be null here");
                break;
            }

            if (action.Invoke(i, currentNode)) break;

            currentNode = currentNode.Next;
        }
    }
}