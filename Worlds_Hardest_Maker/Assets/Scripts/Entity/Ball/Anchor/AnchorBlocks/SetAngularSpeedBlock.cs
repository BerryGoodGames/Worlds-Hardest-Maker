public class SetAngularSpeedBlock : AnchorBlock
{
    public enum Unit
    {
        ITERATIONS,
        DEGREES,
        TIME
    }

    public const Type BlockType = Type.SET_ANGULAR_SPEED;
    public override Type ImplementedBlockType => BlockType;

    private readonly float input;
    private readonly Unit type;
    public SetAngularSpeedBlock(AnchorController anchor, float speed, Unit type) : base(anchor)
    {
        input = type switch
        {
            Unit.ITERATIONS => speed * 360,
            _ => speed
        };

        this.type = type;
    }

    public override void Execute(bool executeNext = true)
    {
        Anchor.ApplyAngularSpeed = type != Unit.TIME;
        Anchor.AngularSpeed = input;
        if (executeNext)
            Anchor.FinishCurrentExecution();
    }
}