public class LoopBlock : AnchorBlock
{
    public LoopBlock(bool isLocked) : base(isLocked) { }

    public override string TypeID => "Loop";

    public override void Execute(IAnchorBlockExecutionContext ctx)
    {
        // set loop block node of anchor
        ctx.StoreCurrentLoopIndex();
        ctx.FinishCurrentExecution();
    }
    
    public override void SetControllerValues(AnchorBlockController c) { }
    
    public override AnchorBlockData GetData() => new LoopBlockData(IsLocked);
}