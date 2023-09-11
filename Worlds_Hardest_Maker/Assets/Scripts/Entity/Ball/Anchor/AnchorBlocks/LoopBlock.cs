using UnityEngine;

public class LoopBlock : AnchorBlock
{
    public const Type BlockType = Type.Loop;
    public override Type ImplementedBlockType => BlockType;
    protected override GameObject Prefab => PrefabManager.Instance.GoToBlockPrefab;

    public LoopBlock(AnchorController anchor, bool isLocked) : base(anchor, isLocked)
    {
    }

    public override void Execute()
    {
        // set loop block node of anchor
        Anchor.StoreCurrentLoopIndex();
        Anchor.FinishCurrentExecution();
    }

    protected override void SetControllerValues(AnchorBlockController c)
    {
    }

    public override AnchorBlockData GetData() => new LoopBlockData(IsLocked);
}