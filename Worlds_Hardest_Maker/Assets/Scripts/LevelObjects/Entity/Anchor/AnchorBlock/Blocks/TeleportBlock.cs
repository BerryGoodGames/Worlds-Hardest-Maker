using UnityEngine;

public class TeleportBlock : PositionAnchorBlock
{
    public TeleportBlock(AnchorController anchor, bool isLocked, Vector2 target) : base(anchor, isLocked, target) { }

    public override string TypeID => "Teleport";

    public override void Execute()
    {
        Anchor.transform.position = TargetAbsolute;
        Anchor.FinishCurrentExecution();
    }
    
    protected override void SetControllerValues(AnchorBlockController c)
    {
        TeleportBlockController controller = (TeleportBlockController)c;
        controller.PositionInput.SetPositionValues(TargetAbsolute);
    }
    
    public override bool HasCurrentlyDuration => false;
    
    public override AnchorBlockData GetData() => new TeleportBlockData(IsLocked, Target);
}