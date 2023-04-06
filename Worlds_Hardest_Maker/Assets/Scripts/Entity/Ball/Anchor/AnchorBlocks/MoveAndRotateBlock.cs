using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAndRotateBlock : AnchorBlock
{
    public const Type BlockType = Type.MOVE_AND_ROTATE;
    public override Type ImplementedBlockType => Type.MOVE_AND_ROTATE;

    private readonly Vector2 target;

    private readonly float iterations;

    private readonly bool adaptRotation;

    #region Constructors

    public MoveAndRotateBlock(AnchorController anchor, Vector2 target, float iterations) : base(anchor)
    {
        this.target = target;
        this.iterations = iterations;
    }

    public MoveAndRotateBlock(AnchorController anchor, float x, float y, float iterations)  : this(anchor, new(x, y), iterations)
    {
    }

    #endregion

    public override void Execute(bool executeNext = true)
    {
        // get move duration
        float moveDuration;
        float dist = Vector2.Distance(target, Anchor.Rb.position);

        if (Anchor.ApplySpeed)
        {
            float speed = Anchor.Speed;

            moveDuration = dist / speed;
        }
        else
        {
            moveDuration = Anchor.Speed;
        }

        // get rotate duration
        float rotateDuration;
        if (adaptRotation)
        {
            rotateDuration = moveDuration;
        }
        else if (Anchor.ApplyAngularSpeed)
        {
            float currentZ = Anchor.transform.localRotation.eulerAngles.z;
            float targetZ = currentZ + iterations * 360;
            float distance = targetZ - currentZ;

            rotateDuration = distance / Anchor.AngularSpeed;
        }
        else
        {
            rotateDuration = Anchor.AngularSpeed;
        }
        

        Anchor.DOKill();
        Anchor.Rb.DOMove(target, moveDuration)
            .SetEase(Anchor.Ease)
            .OnComplete(() =>
            {
                if (rotateDuration < moveDuration || Math.Abs(rotateDuration - moveDuration) < 0.001)
                    Anchor.FinishCurrentExecution();
            });
        
        Anchor.Rb.DORotate(iterations * 360, rotateDuration)
            .SetRelative()
            .SetEase(Anchor.Ease)
            .OnComplete(() =>
            {
                if (rotateDuration > moveDuration)
                    Anchor.FinishCurrentExecution();
            });
    }

    public override void CreateAnchorBlockObject(Transform parent, bool insertable = true)
    {
        throw new System.NotImplementedException();
    }
}
