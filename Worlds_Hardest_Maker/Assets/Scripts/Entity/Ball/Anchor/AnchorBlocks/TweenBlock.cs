using DG.Tweening;

public class TweenBlock : AnchorBlock
{
    public const Type blockType = Type.TWEEN;
    public override Type ImplementedBlockType => blockType;

    private readonly Ease ease;

    public TweenBlock(AnchorController anchor, Ease ease) : base(anchor)
    {
        this.ease = ease;
    }

    public override void Execute(bool executeNext = true)
    {
        anchor.ease = ease;
        if(executeNext)
            anchor.FinishCurrentExecution();
    }
}