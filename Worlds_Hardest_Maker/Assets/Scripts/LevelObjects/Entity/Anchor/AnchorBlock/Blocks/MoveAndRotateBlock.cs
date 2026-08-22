using System;
using DG.Tweening;
using UnityEngine;

public class MoveAndRotateBlock : PositionAnchorBlock
{
    private readonly float iterations;
    
    private readonly bool adaptRotation;
    
    public MoveAndRotateBlock(
        AnchorController anchor, bool isLocked, Vector2 target, float iterations,
        bool adaptRotation
    ) :
        base(anchor, isLocked, target)
    {
        this.iterations = iterations;
        this.adaptRotation = adaptRotation;
    }

    public override string TypeID => "MoveAndRotate";

    public override void Execute(IAnchorBlockExecutionContext ctx)
    {
        // get move duration
        float moveDuration;
        float dist = Vector2.Distance(TargetAbsolute, ctx.Position);
        
        if (ctx.SpeedUnit is SetSpeedBlock.Unit.UnitsPerSecond)
        {
            float speed = ctx.SpeedInput;
            
            moveDuration = dist / speed;
        }
        else moveDuration = ctx.SpeedInput;
        
        // get rotate duration
        float rotateDuration;
        if (adaptRotation) rotateDuration = moveDuration;
        else if (ctx.RotationUnit is RotationUnit.Degrees or RotationUnit.Iterations)
        {
            float speed = SetRotationBlock.GetSpeed(ctx.RotationInput, ctx.RotationUnit);
            
            float currentZ = ctx.ZAngle;
            float targetZ = currentZ + iterations * 360;
            float distance = targetZ - currentZ;
            
            rotateDuration = distance / speed;
        }
        else rotateDuration = ctx.RotationInput;
        
        ctx.Transform.DOMove(TargetAbsolute, moveDuration)
            .SetEase(ctx.Ease)
            .OnComplete(
                () =>
                {
                    if (rotateDuration < moveDuration || Math.Abs(rotateDuration - moveDuration) < 0.001) ctx.FinishCurrentExecution();
                }
            );
        
        // negate rotation depending on direction
        int direction = ctx.IsClockwise ? -1 : 1;
        
        ctx.RotationTween.Kill();
        ctx.RotationTween = ctx.Transform
            .DORotate(iterations * 360 * direction * Vector3.forward, rotateDuration, RotateMode.FastBeyond360)
            .SetRelative()
            .SetEase(ctx.Ease)
            .OnComplete(
                () =>
                {
                    if (rotateDuration > moveDuration) ctx.FinishCurrentExecution();
                }
            );
    }
    
    public override void SetControllerValues(AnchorBlockController c)
    {
        MoveAndRotateBlockController controller = (MoveAndRotateBlockController)c;
        controller.PositionInput.SetPositionValues(TargetAbsolute);
        controller.IterationsInput.text = iterations.ToString();
        controller.AdaptRotation.isOn = adaptRotation;
    }
    
    public override AnchorBlockData GetData() => new MoveAndRotateBlockData(IsLocked, Target, iterations, adaptRotation);
}