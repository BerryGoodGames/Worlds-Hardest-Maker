using DG.Tweening;

public class WaitBlock : AnchorBlock
{
    public const Type blockType = Type.WAIT;
    public override Type ImplementedBlockType => blockType;

    private readonly float waitTime;
    private float duration;

    public WaitBlock(AnchorController anchor, float waitTime) : base(anchor)
    {
        this.waitTime = waitTime;
    }

    public override void Execute(bool executeNext = true)
    {
        anchor.rb.DOMove(anchor.rb.position, waitTime)
            .OnComplete(() =>
            {
                if(executeNext)
                    anchor.FinishCurrentExecution();
            });
    }
}