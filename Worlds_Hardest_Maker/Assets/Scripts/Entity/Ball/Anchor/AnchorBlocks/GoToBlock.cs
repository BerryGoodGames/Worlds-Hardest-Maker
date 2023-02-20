using System.Linq;

public class GoToBlock : AnchorBlock
{
    public const Type BlockType = Type.GO_TO;
    public override Type ImplementedBlockType => BlockType;

    private readonly int index;

    public GoToBlock(AnchorController anchor, int index) : base(anchor)
    {
        this.index = index;
    }

    public override void Execute(bool executeNext = true)
    {
        Anchor.CurrentExecutingBlock = Anchor.Blocks.ElementAt(index);
        if(executeNext)
            Anchor.CurrentExecutingBlock.Execute();
    }
}