using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Anchor attributes: balls (positions), blocks, position
/// </summary>
[Serializable]
public class AnchorData : Data
{
    // (list of coordinates)
    private AnchorBallData[] balls;

    private AnchorBlockData[] blocks;

    private readonly float[] position;

    public AnchorData(AnchorController controller)
    {
        // init balls and blocks
        SaveBalls(controller);
        SaveBlocks(controller);

        Vector2 controllerPosition = controller.StartPosition;

        // init start position
        position = new[]
        {
            controllerPosition.x,
            controllerPosition.y
        };
    }

    #region Saving / loading properties

    private void SaveBalls(AnchorController controller)
    {
        // init balls
        balls = new AnchorBallData[controller.Balls.Count];
        for (int i = 0; i < controller.Balls.Count; i++)
        {
            balls[i] = new(controller.Balls[i].GetChild(0).localPosition);
        }
    }

    private void SaveBlocks(AnchorController controller)
    {
        // init blocks
        // loop through LinkedList
        blocks = new AnchorBlockData[controller.Blocks.Count];
        int j = 0;
        for (LinkedListNode<AnchorBlock> currentBlockNode = controller.Blocks.First;
             currentBlockNode != null;
             currentBlockNode = currentBlockNode.Next)
        {
            AnchorBlock currentBlock = currentBlockNode.Value;

            // assign data
            blocks[j] = currentBlock.GetData();

            j++;
        }
    }

    private LinkedList<AnchorBlock> LoadBlocks(AnchorController anchor)
    {
        LinkedList<AnchorBlock> blockArr = new();

        foreach (AnchorBlockData blockData in blocks)
        {
            AnchorBlock anchorBlock = blockData.GetBlock(anchor);
            blockArr.AddLast(anchorBlock);
        }

        return blockArr;
    }

    #endregion


    public override void ImportToLevel() => ImportToLevel(new (position[0], position[1]));

    public override void ImportToLevel(Vector2 pos)
    {
        AnchorController anchor = AnchorManager.Instance.SetAnchor(pos);

        foreach (AnchorBallData ball in balls)
        {
            ball.ImportToLevel(anchor);
        }

        anchor.Blocks = LoadBlocks(anchor);
    }

    public override EditMode GetEditMode() => EditMode.Anchor;
}