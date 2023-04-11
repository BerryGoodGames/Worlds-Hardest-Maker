using System.Linq;
using UnityEngine;

public class GoToBlock : AnchorBlock
{
    public const Type BlockType = Type.GO_TO;
    public override Type ImplementedBlockType => BlockType;
    protected override GameObject Prefab => PrefabManager.Instance.GoToBlockPrefab;

    private readonly int index;

    public GoToBlock(AnchorController anchor, int index) : base(anchor) => this.index = index;

    public override void Execute()
    {
        Anchor.CurrentExecutingBlock = Anchor.Blocks.ElementAt(index);
        Anchor.CurrentExecutingBlock.Execute();
    }

    protected override void SetControllerValues(AnchorBlockController c)
    {
        GoToBlockController controller = (GoToBlockController)c;
        controller.Input.text = index.ToString();
    }

    public override AnchorBlockData GetData() => new GoToBlockData(index);
}