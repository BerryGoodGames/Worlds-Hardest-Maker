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
    private readonly Unit unit;

    public SetSpeedBlock(AnchorController anchor, float input, Unit unit) : base(anchor)
    {
        this.input = input;
        this.unit = unit;
    }

    public override void Execute()
    {
        Anchor.ApplySpeed = unit != Unit.TIME;
        Anchor.Speed = input;
        Anchor.FinishCurrentExecution();
    }

    public override void CreateAnchorBlockObject(Transform parent, bool insertable = true)
    {
        Transform connectorContainer = parent.GetChild(0);

        // create object
        GameObject block = Object.Instantiate(PrefabManager.Instance.SetSpeedBlockPrefab, parent);

        // set values in object
        SetSpeedBlockController controller = block.GetComponent<SetSpeedBlockController>();
        controller.SpeedInput.text = input.ToString();
        controller.UnitInput.value =
            GameManager.GetDropdownValue(SetSpeedBlockController.GetOption(unit), controller.UnitInput);
        controller.Movable = insertable;

        // create connector
        CreateAnchorConnector(connectorContainer, block.transform.GetSiblingIndex(), insertable);
    }

    public void Print()
    {
        Debug.Log((input, unit));
    }

    public override AnchorBlockData GetData() => new SetSpeedBlockData(input, unit);
}