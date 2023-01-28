using System.Collections.Generic;

public class SetSpeedBlock : AnchorBlock
{
    public enum Unit
    {
        SPEED,
        TIME
    }

    public const Type blockType = Type.SET_SPEED;
    public override Type ImplementedBlockType => blockType;

    private readonly float input;
    private readonly Unit type;
    public SetSpeedBlock(AnchorController anchor, float input, Unit type) : base(anchor)
    {
        this.input = input;
        this.type = type;
    }

    public override void Execute(bool executeNext = true)
    {
        anchor.applySpeed = type != Unit.TIME;
        anchor.speed = input;
        if(executeNext)
            anchor.FinishCurrentExecution();
    }
}