using UnityEngine;

public class LoopBlock : AnchorBlock
{
    public const Type BlockType = Type.Loop;
    public override Type ImplementedBlockType => BlockType;
    protected override GameObject Prefab => PrefabManager.Instance.GoToBlockPrefab;

    // public readonly int Index;

    public LoopBlock(AnchorController anchor, bool isLocked) : base(anchor, isLocked)
    {
    }

    public override void Execute()
    {
        // TODO: Loop block execution
        // Anchor.CurrentExecutingBlock = Anchor.Blocks.ElementAt(Index);
        // Anchor.CurrentExecutingNode = Anchor.Blocks.Find(Anchor.CurrentExecutingBlock);
        //
        // Anchor.CurrentExecutingBlock.Execute();
        Anchor.FinishCurrentExecution();
    }

    protected override void SetControllerValues(AnchorBlockController c)
    {
    }

    // protected override void SetControllerValues(AnchorBlockController c)
    // {
    //     GoToBlockController controller = (GoToBlockController)c;
    //     controller.Input.text = Index.ToString();
    // }

    public override AnchorBlockData GetData() => new LoopBlockData(IsLocked);
}