using UnityEngine;

public class SetSpeedBlock : AnchorBlock
{
    public enum Unit
    {
        SPEED,
        TIME
    }

    public const Type BlockType = Type.SET_SPEED;
    public override Type ImplementedBlockType => BlockType;

    private readonly float input;
    private readonly Unit type;

    public SetSpeedBlock(AnchorController anchor, float input, Unit type) : base(anchor)
    {
        this.input = input;
        this.type = type;
    }

    public override void Execute(bool executeNext = true)
    {
        Anchor.ApplySpeed = type != Unit.TIME;
        Anchor.Speed = input;
        if (executeNext)
            Anchor.FinishCurrentExecution();
    }

    public override void CreateAnchorBlockObject(Transform parent, bool insertable = true)
    {
        Transform connectorContainer = parent.GetChild(0);

        // create object
        GameObject block = GameManager.Instantiate(PrefabManager.Instance.SetSpeedBlockPrefab, parent);

        // set values in object
        SetSpeedBlockController controller = block.GetComponent<SetSpeedBlockController>();
        controller.Input.text = input.ToString();
        controller.IsInsertable = insertable;

        // create connector
        CreateAnchorConnector(connectorContainer, block.transform.GetSiblingIndex(), insertable);
    }
}