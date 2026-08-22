using DG.Tweening;
using UnityEngine;

public class RotateBlock : AnchorBlock, IDurationBlock
{
    private readonly float iterations;
    
    public bool HasCurrentlyDuration => iterations > 0;
    
    public RotateBlock(bool isLocked, float iterations) : base(isLocked) => this.iterations = iterations;

    public override string TypeID => "Rotate";

    public override void Execute(IAnchorBlockExecutionContext ctx)
    {
        float duration;
        
        if (ctx.RotationUnit is RotationUnit.Degrees or RotationUnit.Iterations)
        {
            float speed = SetRotationBlock.GetSpeed(ctx.RotationInput, ctx.RotationUnit);
            
            float currentZ = ctx.ZAngle;
            float targetZ = currentZ + iterations * 360;
            float distance = targetZ - currentZ;
            
            duration = distance / speed;
        }
        else duration = ctx.RotationInput;
        
        // negate rotation depending on direction
        int direction = ctx.IsClockwise ? -1 : 1;
        
        ctx.RotationTween.Kill();
        ctx.RotationTween.Kill();
        ctx.RotationTween = ctx.Transform.DORotate(iterations * 360 * direction * Vector3.forward, duration, RotateMode.FastBeyond360)
            .SetRelative()
            .SetEase(ctx.Ease)
            .OnComplete(ctx.FinishCurrentExecution);
    }
    
    protected override void SetControllerValues(AnchorBlockController c)
    {
        RotateBlockController controller = (RotateBlockController)c;
        controller.IterationsInput.text = iterations.ToString();
    }
    
    public override AnchorBlockData GetData() => new RotateBlockData(IsLocked, iterations);
}