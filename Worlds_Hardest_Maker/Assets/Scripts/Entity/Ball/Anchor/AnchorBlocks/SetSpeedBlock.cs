public class SetSpeedBlock : AnchorBlock
{
    public enum Unit
    {
        SPEED,
        TIME
    }

    public const Type BlockType = Type.SET_SPEED;
    public override Type ImplementedBlockType => BlockType;

    private readonly float input;
    private readonly Unit type;

    public SetSpeedBlock(AnchorController anchor, float input, Unit type) : base(anchor)
    {
        this.input = input;
        this.type = type;
    }

    public override void Execute(bool executeNext = true)
    {
        Anchor.ApplySpeed = type != Unit.TIME;
        Anchor.Speed = input;
        if (executeNext)
            Anchor.FinishCurrentExecution();
    }
}