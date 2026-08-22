using DG.Tweening;
using UnityEngine;

public class StartRotatingBlock : AnchorBlock
{
    public StartRotatingBlock(bool isLocked) : base(isLocked) { }

    public override string TypeID => "StartRotating";

    public override void Execute(IAnchorBlockExecutionContext ctx)
    {
        // ignore if already infinitely rotating or no speed defined
        if (ctx.RotationTween is not { hasLoops: true, } &&
            ctx.RotationUnit is RotationUnit.Degrees or RotationUnit.Iterations)
        {
            float speed = SetRotationBlock.GetSpeed(ctx.RotationInput, ctx.RotationUnit);
            
            float duration = 360 / speed;
            
            // negate rotation depending on direction
            int direction = ctx.IsClockwise ? -1 : 1;
            
            ctx.RotationTween.Kill();
            ctx.RotationTween = ctx.Transform.DORotate(360 * direction * Vector3.forward, duration, RotateMode.FastBeyond360)
                .SetRelative()
                .SetLoops(-1)
                .SetEase(Ease.Linear);
        }
        
        ctx.FinishCurrentExecution();
    }
    
    protected override void SetControllerValues(AnchorBlockController c) { }
    
    public override AnchorBlockData GetData() => new StartRotatingBlockData(IsLocked);
}