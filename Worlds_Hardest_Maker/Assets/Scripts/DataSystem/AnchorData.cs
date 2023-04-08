using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Anchor attributes: 
/// </summary>
[Serializable]
public class AnchorData : Data
{
    // (list of coordinates)
    private float[,] balls;

    private AnchorBlockData[] blocks;

    private float[] position;

    public AnchorData(AnchorController controller)
    {
        // init balls and blocks
        SaveBalls(controller);
        SaveBlocks(controller);
        
        // init start position
        position = new[]
        {
            controller.transform.position.x,
            controller.transform.position.y
        };
    }

    #region Saving / loading properties
    private void SaveBalls(AnchorController controller)
    {
        // init balls
        balls = new float[controller.Balls.Count, 2];
        for (int i = 0; i < controller.Balls.Count; i++)
        {
            AnchorBallController ball = controller.Balls[i];
            Vector2 ballPosition = ball.BallObject.transform.position;

            balls[i, 0] = ballPosition.x;
            balls[i, 1] = ballPosition.y;
        }
    }

    private void SaveBlocks(AnchorController controller)
    {
        // init blocks
        // loop through LinkedList
        blocks = new AnchorBlockData[controller.Blocks.Count];
        int j = 0;
        for (LinkedListNode<AnchorBlock> currentBlockNode = controller.Blocks.First; currentBlockNode != null; currentBlockNode = currentBlockNode.Next)
        {
            AnchorBlock currentBlock = currentBlockNode.Value;

            // assign data
            blocks[j] = currentBlock.GetData();

            j++;
        }
    }

    private List<Vector2> LoadBalls()
    {
        // returns every coordinate of the balls
        List<Vector2> posArr = new();

        for (int i = 0; i < balls.GetLength(0); i++)
        {
            Vector2 pos = new(balls[i, 0], balls[i, 1]);
            posArr.Add(pos);
        }

        return posArr;
    }

    private LinkedList<AnchorBlock> LoadBlocks()
    {
        LinkedList<AnchorBlock> blockArr = new();

        foreach (AnchorBlockData block in blocks)
        {
            AnchorBlock anchorBlock = block.GetBlock();
            blockArr.AddLast(anchorBlock);
        }

        return blockArr;
    }
    #endregion


    public override void ImportToLevel()
    {
        ImportToLevel(new Vector2(position[0], position[1]));
    }

    public override void ImportToLevel(Vector2 pos)
    {
        AnchorController anchor = AnchorManager.Instance.SetAnchor(pos);

        List<Vector2> ballPositions = LoadBalls();

        foreach (Vector2 ballPosition in ballPositions)
        {
            AnchorBallManager.SetAnchorBall(ballPosition, anchor);
        }

        anchor.Blocks = LoadBlocks();
    }

    public override EditMode GetEditMode()
    {
        return EditMode.ANCHOR;
    }
}