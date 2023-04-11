using UnityEngine;
using UnityEngine.UI;

public class SetAngularSpeedBlock : AnchorBlock
{
    public enum Unit
    {
        ITERATIONS,
        DEGREES,
        TIME
    }

    public const Type BlockType = Type.SET_ANGULAR_SPEED;
    public override Type ImplementedBlockType => BlockType;
    protected override GameObject Prefab => PrefabManager.Instance.SetAngularSpeedBlockPrefab;

    private readonly float speed;
    private readonly Unit unit;

    public SetAngularSpeedBlock(AnchorController anchor, float speed, Unit unit) : base(anchor)
    {
        this.speed = unit switch
        {
            Unit.ITERATIONS => speed * 360,
            _ => speed
        };

        this.unit = unit;
    }

    public override void Execute()
    {
        Anchor.ApplyAngularSpeed = unit != Unit.TIME;
        Anchor.AngularSpeed = speed;
        Anchor.FinishCurrentExecution();
    }

    public float GetSpeed(Unit unit)
    {
        return unit switch
        {
            Unit.ITERATIONS => speed * 360,
            _ => speed
        };
    }

    protected override void SetControllerValues(AnchorBlockController c)
    {
        SetAngularSpeedBlockController controller = (SetAngularSpeedBlockController)c;
        controller.SpeedInput.text = GetSpeed(Unit.DEGREES).ToString();
        controller.UnitInput.value =
            GameManager.GetDropdownValue(SetAngularSpeedBlockController.GetOption(unit), controller.UnitInput);
    }

    public override AnchorBlockData GetData() => new SetAngularSpeedBlockData(speed, unit);
}