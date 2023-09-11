using DG.Tweening;
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
        Anchor.Rb.MovePosition(Target);
        Anchor.FinishCurrentExecution();
    }

    protected override void SetControllerValues(AnchorBlockController c) => throw new System.NotImplementedException();

    public override AnchorBlockData GetData() => new TeleportBlockData(IsLocked, Target);
}
