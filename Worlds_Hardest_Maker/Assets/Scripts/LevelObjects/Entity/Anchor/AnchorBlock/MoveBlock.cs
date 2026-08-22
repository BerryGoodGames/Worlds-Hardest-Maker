using DG.Tweening;
using UnityEngine;

public class MoveBlock : PositionAnchorBlock
{
    public MoveBlock(AnchorController anchor, bool isLocked, Vector2 target) : base(anchor, isLocked, target) { }

    public override string TypeID => "Move";

    public override void Execute()
    {
        float duration;
        float dist = Vector2.Distance(TargetAbsolute, Anchor.transform.position);
        
        if (Anchor.SpeedUnit is SetSpeedBlock.Unit.UnitsPerSecond) duration = dist / Anchor.SpeedInput;
        else duration = Anchor.SpeedInput;
        
        Anchor.DOKill();
        Anchor.transform.DOMove(TargetAbsolute, duration)
            .SetEase(Anchor.Ease)
            .OnComplete(Anchor.FinishCurrentExecution);
    }
    
    protected override void SetControllerValues(AnchorBlockController c)
    {
        MoveBlockController controller = (MoveBlockController)c;
        controller.PositionInput.SetPositionValues(TargetAbsolute);
    }
    
    public override AnchorBlockData GetData() => new MoveBlockData(IsLocked, Target);
}