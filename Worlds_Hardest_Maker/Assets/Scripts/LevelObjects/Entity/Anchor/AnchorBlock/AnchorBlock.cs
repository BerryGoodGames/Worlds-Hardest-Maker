public abstract class AnchorBlock
{
    public readonly bool IsLocked;
    
    public AnchorBlockController Controller { get; set; }
    
    protected AnchorBlock(bool isLocked)
    {
        IsLocked = isLocked;
    }
    
    public abstract string TypeID { get; }
    
    public abstract void Execute(IAnchorBlockExecutionContext ctx);
    public abstract void SetControllerValues(AnchorBlockController c);
    public abstract AnchorBlockData GetData();
}