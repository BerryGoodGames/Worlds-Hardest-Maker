using System.Linq;
using UnityEngine;

public class GoToBlock : AnchorBlock
{
    public const Type BlockType = Type.GO_TO;
    public override Type ImplementedBlockType => BlockType;

    private readonly int index;

    public GoToBlock(AnchorController anchor, int index) : base(anchor)
    {
        this.index = index;
    }

    public override void Execute(bool executeNext = true)
    {
        Anchor.CurrentExecutingBlock = Anchor.Blocks.ElementAt(index);
        if (executeNext)
            Anchor.CurrentExecutingBlock.Execute();
    }

    public override void CreateAnchorBlockObject(Transform parent, bool insertable = true)
    {
        Transform connectorContainer = parent.GetChild(0);

        // create object
        GameObject block = GameManager.Instantiate(PrefabManager.Instance.GoToBlockPrefab, parent);

        // set values in object
        GoToBlockController controller = block.GetComponent<GoToBlockController>();
        controller.Input.text = index.ToString();
        controller.IsInsertable = insertable;

        // create connector
        CreateAnchorConnector(connectorContainer, block.transform.GetSiblingIndex(), insertable);
    }
}