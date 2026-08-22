public class SetDirectionBlock : AnchorBlock
{
    private readonly bool isClockwise;
    
    public SetDirectionBlock(bool isLocked, bool isClockwise) : base(isLocked)
    {
        this.isClockwise = isClockwise;
    }

    public override string TypeID => "SetDirection";

    public override void Execute(IAnchorBlockExecutionContext ctx)
    {
        ctx.IsClockwise = isClockwise;
        ctx.FinishCurrentExecution();
    }
    
    protected override void SetControllerValues(AnchorBlockController c)
    {
        SetDirectionBlockController controller = (SetDirectionBlockController)c;
        controller.DirectionInput.IsClockwise = isClockwise;
    }
    
    public override AnchorBlockData GetData() => new SetDirectionBlockData(IsLocked, isClockwise);
}