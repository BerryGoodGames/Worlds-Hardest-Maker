using UnityEngine;

public class TeleportBlock : PositionAnchorBlock, IActiveAnchorBlock
{
    public TeleportBlock(AnchorController anchor, bool isLocked, Vector2 target) : base(anchor, isLocked, target)
    {
    }

    public override Type ImplementedBlockType => Type.Teleport;
    protected override GameObject Prefab => PrefabManager.Instance.TeleportBlockPrefab;

    public override void Execute()
    {
        Anchor.transform.position = Target;
        Anchor.FinishCurrentExecution();
    }

    protected override void SetControllerValues(AnchorBlockController c)
    {
        TeleportBlockController controller = (TeleportBlockController)c;
        controller.PositionInput.SetPositionValues(Target);
    }

    public override bool HasCurrentlyDuration() => false;
    public override bool IsLinePreviewDashed() => true;

    public override AnchorBlockData GetData() => new TeleportBlockData(IsLocked, Target);
}