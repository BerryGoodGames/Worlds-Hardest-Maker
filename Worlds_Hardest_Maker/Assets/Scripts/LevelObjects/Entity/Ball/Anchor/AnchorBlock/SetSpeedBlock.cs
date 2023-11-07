using UnityEngine;

public class SetSpeedBlock : AnchorBlock, IPassiveAnchorBlock
{
    public enum Unit
    {
        Speed,
        Time,
    }

    public const Type BlockType = Type.SetSpeed;
    public override Type ImplementedBlockType => BlockType;
    protected override GameObject Prefab => PrefabManager.Instance.SetSpeedBlockPrefab;

    private readonly float input;
    private readonly Unit unit;

    public SetSpeedBlock(AnchorController anchor, bool isLocked, float input, Unit unit) : base(anchor, isLocked)
    {
        this.input = input;
        this.unit = unit;
    }

    public override void Execute()
    {
        Anchor.SpeedUnit = unit;
        Anchor.SpeedInput = input;
        Anchor.FinishCurrentExecution();
    }

    protected override void SetControllerValues(AnchorBlockController c)
    {
        SetSpeedBlockController controller = (SetSpeedBlockController)c;

        controller.SpeedInput.text = input.ToString();
        controller.UnitInput.value =
            GameManager.GetDropdownValue(SetSpeedBlockController.GetOption(unit), controller.UnitInput);
    }

    public void Print() => Debug.Log((input, unit));

    public override AnchorBlockData GetData() => new SetSpeedBlockData(IsLocked, input, unit);
}