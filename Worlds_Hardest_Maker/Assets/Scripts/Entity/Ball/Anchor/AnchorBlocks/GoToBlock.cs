using System.Linq;

public class GoToBlock : AnchorBlock
{
    public const Type blockType = Type.GO_TO;
    public override Type ImplementedBlockType => blockType;

    private readonly int index;

    public GoToBlock(AnchorController anchor, int index) : base(anchor)
    {
        this.index = index;
    }

    public override void Execute(bool executeNext = true)
    {
        anchor.currentExecutingBlock = anchor.blocks.ElementAt(index);
        if(executeNext)
            anchor.currentExecutingBlock.Execute();
    }
}