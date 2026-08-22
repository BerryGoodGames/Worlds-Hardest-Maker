using DG.Tweening;
using UnityEngine;

public class StopRotatingBlock : AnchorBlock
{
    public StopRotatingBlock(AnchorController anchor, bool isLocked) : base(anchor, isLocked) { }

    public override string TypeID => "StopRotation";

    public override void Execute()
    {
        Anchor.RotationTween.Kill();
        Anchor.FinishCurrentExecution();
    }
    
    protected override void SetControllerValues(AnchorBlockController c) { }
    
    public override AnchorBlockData GetData() => new StopRotatingBlockData(IsLocked);
}