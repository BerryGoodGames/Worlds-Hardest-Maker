using DG.Tweening;
using UnityEngine;

public class StopRotatingBlock : AnchorBlock
{
    public StopRotatingBlock(AnchorController anchor) : base(anchor)
    {
    }

    public const Type BlockType = Type.STOP_ROTATING;
    public override Type ImplementedBlockType => BlockType;
    protected override GameObject Prefab => PrefabManager.Instance.StopRotatingBlockPrefab;

    public override void Execute()
    {
        Anchor.InfiniteRotationTween.Kill();
        Anchor.FinishCurrentExecution();
    }

    protected override void SetControllerValues(AnchorBlockController c) {}

    public override AnchorBlockData GetData() => new StopRotatingBlockData();
}