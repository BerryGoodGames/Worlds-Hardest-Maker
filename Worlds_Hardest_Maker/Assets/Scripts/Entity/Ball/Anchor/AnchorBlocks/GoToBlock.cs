using System.Linq;
using UnityEngine;

public class GoToBlock : AnchorBlock
{
    public const Type BlockType = Type.GoTo;
    public override Type ImplementedBlockType => BlockType;
    protected override GameObject Prefab => PrefabManager.Instance.GoToBlockPrefab;

    public readonly int Index;

    public GoToBlock(AnchorController anchor, bool isLocked, int index) : base(anchor, isLocked) => Index = index;

    public override void Execute()
    {
        Anchor.CurrentExecutingBlock = Anchor.Blocks.ElementAt(Index);
        Anchor.CurrentExecutingNode = Anchor.Blocks.Find(Anchor.CurrentExecutingBlock);

        Anchor.CurrentExecutingBlock.Execute();
    }

    protected override void SetControllerValues(AnchorBlockController c)
    {
        GoToBlockController controller = (GoToBlockController)c;
        controller.Input.text = Index.ToString();
    }

    public override AnchorBlockData GetData() => new GoToBlockData(IsLocked, Index);
}