using DG.Tweening;
using UnityEngine;

public class MoveToBlock : AnchorBlock
{
    public const Type blockType = Type.MOVE_TO;
    public override Type ImplementedBlockType => blockType;

    private readonly Vector2 target;

    private readonly RotateBlock rotateBlock;

    #region Constructors
    public MoveToBlock(AnchorController anchor, Vector2 target, RotateBlock rotateBlock = null) : base(anchor)
    {
        this.target = target;
        this.rotateBlock = rotateBlock;
    }
    public MoveToBlock(AnchorController anchor, float x, float y, RotateBlock rotateBlock = null) : this(anchor, new(x, y), rotateBlock)
    {
    }
    #endregion
    
    public override void Execute(bool executeNext = true)
    {
        float duration;
        float dist = Vector2.Distance(target, anchor.rb.position);

        if (anchor.applySpeed)
        {
            float speed = anchor.speed;
            
            duration = dist / speed;
        }
        else
        {
            duration = anchor.speed;
        }

        // TODO: rethink about other system
        if (rotateBlock != null)
        {
            rotateBlock.customTime = duration;
            rotateBlock.customIterations = anchor.angularSpeed / 360 * duration;
            rotateBlock.Execute(false);
        }

        anchor.DOKill();
        anchor.rb.DOMove(target, duration)
            .SetEase(anchor.ease)
            .OnComplete(anchor.FinishCurrentExecution);
    }
}