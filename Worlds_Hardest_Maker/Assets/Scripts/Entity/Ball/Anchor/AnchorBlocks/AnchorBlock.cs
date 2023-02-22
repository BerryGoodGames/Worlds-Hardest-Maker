public abstract class AnchorBlock
{
    public enum Type
    {
        GO_TO,
        MOVE_TO,
        ROTATE,
        WAIT,
        TWEEN,
        SET_SPEED,
        SET_ANGULAR_SPEED
    }

    protected AnchorController Anchor;

    protected AnchorBlock(AnchorController anchor)
    {
        Anchor = anchor;
    }

    // ReSharper disable once UnusedMember.Global
    public abstract Type ImplementedBlockType { get; }

    public abstract void Execute(bool executeNext = true);
}