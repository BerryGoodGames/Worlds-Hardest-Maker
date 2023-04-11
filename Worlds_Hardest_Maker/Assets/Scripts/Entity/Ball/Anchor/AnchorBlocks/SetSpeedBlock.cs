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
    protected override GameObject Prefab => PrefabManager.Instance.SetSpeedBlockPrefab;

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

    protected override void SetControllerValues(AnchorBlockController c)
    {
        SetSpeedBlockController controller = (SetSpeedBlockController)c;

        controller.SpeedInput.text = input.ToString();
        controller.UnitInput.value =
        GameManager.GetDropdownValue(SetSpeedBlockController.GetOption(unit), controller.UnitInput);
    }

    public void Print()
    {
        Debug.Log((input, unit));
    }

    public override AnchorBlockData GetData() => new SetSpeedBlockData(input, unit);
}