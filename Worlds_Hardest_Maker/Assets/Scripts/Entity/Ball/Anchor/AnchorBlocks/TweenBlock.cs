using DG.Tweening;

public class TweenBlock : AnchorBlock
{
    public const Type BlockType = Type.TWEEN;
    public override Type ImplementedBlockType => BlockType;

    private readonly Ease ease;

    public TweenBlock(AnchorController anchor, Ease ease) : base(anchor)
    {
        this.ease = ease;
    }

    public override void Execute(bool executeNext = true)
    {
        Anchor.Ease = ease;
        if (executeNext)
            Anchor.FinishCurrentExecution();
    }
}