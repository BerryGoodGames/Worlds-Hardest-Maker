using UnityEngine;

public class SetRotationBlock : AnchorBlock, IPassiveAnchorBlock
{
    public enum Unit
    {
        Iterations,
        Degrees,
        Time
    }

    public const Type BlockType = Type.SetRotationSpeed;
    public override Type ImplementedBlockType => BlockType;
    protected override GameObject Prefab => PrefabManager.Instance.SetRotationSpeedBlockPrefab;

    private readonly float speed;
    private readonly Unit unit;

    public SetRotationBlock(AnchorController anchor, bool isLocked, float speed, Unit unit) : base(anchor, isLocked)
    {
        this.speed = unit switch
        {
            Unit.Iterations => speed * 360,
            _ => speed
        };

        this.unit = unit;
    }

    public override void Execute()
    {
        Anchor.RotationSpeedUnit = unit;
        Anchor.RotationTimeInput = speed;
        Anchor.FinishCurrentExecution();
    }

    public float GetSpeed(Unit unit) =>
        unit switch
        {
            Unit.Iterations => speed / 360,
            _ => speed
        };

    protected override void SetControllerValues(AnchorBlockController c)
    {
        SetRotationBlockController controller = (SetRotationBlockController)c;
        controller.SpeedInput.text = GetSpeed(unit).ToString();
        controller.UnitInput.value =
            GameManager.GetDropdownValue(SetRotationBlockController.GetOption(unit), controller.UnitInput);
    }

    public override AnchorBlockData GetData() => new SetRotationSpeedBlockData(IsLocked, speed, unit);
}