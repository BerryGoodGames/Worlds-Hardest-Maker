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
        // init balls
        balls = new float[controller.Balls.Count, 2];
        for (int i = 0; i < controller.Balls.Count; i++)
        {
            AnchorBallController ball = controller.Balls[i];
            Vector2 ballPosition = ball.BallObject.transform.position;

            balls[i, 0] = ballPosition.x; 
            balls[i, 1] = ballPosition.y;
        }

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

        // init start position
        position = new[]
        {
            controller.transform.position.x,
            controller.transform.position.y
        };
    }

    public override void ImportToLevel()
    {
        throw new NotImplementedException();
    }

    public override void ImportToLevel(Vector2 pos)
    {
        throw new NotImplementedException();
    }

    public override EditMode GetEditMode()
    {
        return EditMode.ANCHOR;
    }
}