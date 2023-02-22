using DG.Tweening;
using UnityEngine;

public class MoveToBlock : AnchorBlock
{
    public const Type BlockType = Type.MOVE_TO;
    public override Type ImplementedBlockType => BlockType;

    private readonly Vector2 target;

    private readonly RotateBlock rotateBlock;

    #region Constructors

    public MoveToBlock(AnchorController anchor, Vector2 target, RotateBlock rotateBlock = null) : base(anchor)
    {
        this.target = target;
        this.rotateBlock = rotateBlock;
    }

    public MoveToBlock(AnchorController anchor, float x, float y, RotateBlock rotateBlock = null) : this(anchor,
        new(x, y), rotateBlock)
    {
    }

    #endregion

    public override void Execute(bool executeNext = true)
    {
        float duration;
        float dist = Vector2.Distance(target, Anchor.Rb.position);

        if (Anchor.ApplySpeed)
        {
            float speed = Anchor.Speed;

            duration = dist / speed;
        }
        else
        {
            duration = Anchor.Speed;
        }

        // TODO: rethink about other system
        if (rotateBlock != null)
        {
            rotateBlock.CustomTime = duration;
            rotateBlock.CustomIterations = Anchor.AngularSpeed / 360 * duration;
            rotateBlock.Execute(false);
        }

        Anchor.DOKill();
        Anchor.Rb.DOMove(target, duration)
            .SetEase(Anchor.Ease)
            .OnComplete(Anchor.FinishCurrentExecution);
    }
}