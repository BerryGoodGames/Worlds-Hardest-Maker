public class LoopBlock : AnchorBlock
{
    public LoopBlock(AnchorController anchor, bool isLocked) : base(anchor, isLocked) { }

    public override string TypeID => "Loop";

    public override void Execute()
    {
        // set loop block node of anchor
        Anchor.StoreCurrentLoopIndex();
        Anchor.FinishCurrentExecution();
    }
    
    protected override void SetControllerValues(AnchorBlockController c) { }
    
    public override AnchorBlockData GetData() => new LoopBlockData(IsLocked);
}