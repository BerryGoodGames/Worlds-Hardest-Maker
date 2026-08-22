using DG.Tweening;
using UnityEngine;

public class MoveBlock : PositionAnchorBlock
{
    public MoveBlock(AnchorController anchor, bool isLocked, Vector2 target) : base(anchor, isLocked, target) { }

    public override string TypeID => "Move";

    public override void Execute(IAnchorBlockExecutionContext ctx)
    {
        float duration;
        float dist = Vector2.Distance(TargetAbsolute, ctx.Position);
        
        if (ctx.SpeedUnit is SetSpeedBlock.Unit.UnitsPerSecond) duration = dist / ctx.SpeedInput;
        else duration = ctx.SpeedInput;

        ctx.TweenComponent.DOKill();
        ctx.Transform.DOMove(TargetAbsolute, duration)
            .SetEase(ctx.Ease)
            .OnComplete(ctx.FinishCurrentExecution);
    }
    
    public override void SetControllerValues(AnchorBlockController c)
    {
        MoveBlockController controller = (MoveBlockController)c;
        controller.PositionInput.SetPositionValues(TargetAbsolute);
    }
    
    public override AnchorBlockData GetData() => new MoveBlockData(IsLocked, Target);
}