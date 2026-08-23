using DG.Tweening;

public class StopRotatingBlock : AnchorBlock
{
    public StopRotatingBlock(bool isLocked) : base(isLocked) { }

    public override string TypeID => "StopRotating";

    public override void Execute(IAnchorBlockExecutionContext ctx)
    {
        ctx.RotationTween.Kill();
        ctx.FinishCurrentExecution();
    }
    
    public override void SetControllerValues(AnchorBlockController c) { }
    
    public override AnchorBlockData GetData() => new StopRotatingBlockData(IsLocked);
}