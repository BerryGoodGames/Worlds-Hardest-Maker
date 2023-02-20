using DG.Tweening;

public class WaitBlock : AnchorBlock
{
    public const Type BlockType = Type.WAIT;
    public override Type ImplementedBlockType => BlockType;

    private readonly float waitTime;
    private float duration;

    public WaitBlock(AnchorController anchor, float waitTime) : base(anchor)
    {
        this.waitTime = waitTime;
    }

    public override void Execute(bool executeNext = true)
    {
        Anchor.Rb.DOMove(Anchor.Rb.position, waitTime)
            .OnComplete(() =>
            {
                if(executeNext)
                    Anchor.FinishCurrentExecution();
            });
    }
}