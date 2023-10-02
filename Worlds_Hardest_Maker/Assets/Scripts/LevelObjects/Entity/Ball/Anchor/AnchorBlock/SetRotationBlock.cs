using System;
using UnityEngine;

public class SetRotationBlock : AnchorBlock, IPassiveAnchorBlock
{
    public enum Unit
    {
        Iterations,
        Degrees,
        Time
    }

    public const Type BlockType = Type.SetRotation;
    public override Type ImplementedBlockType => BlockType;
    protected override GameObject Prefab => PrefabManager.Instance.SetRotationSpeedBlockPrefab;

    private readonly float input;
    private readonly Unit unit;

    public SetRotationBlock(AnchorController anchor, bool isLocked, float input, Unit unit) : base(anchor, isLocked)
    {
        this.input = input;
        this.unit = unit;
    }

    public override void Execute()
    {
        Anchor.RotationSpeedUnit = unit;
        Anchor.RotationInput = input;
        Anchor.FinishCurrentExecution();
    }

    public Unit GetUnit() => unit;

    protected override void SetControllerValues(AnchorBlockController c)
    {
        SetRotationBlockController controller = (SetRotationBlockController)c;
        controller.SpeedInput.text = input.ToString();
        controller.UnitInput.value =
            GameManager.GetDropdownValue(SetRotationBlockController.GetOption(unit), controller.UnitInput);
    }

    public override AnchorBlockData GetData() => new SetRotationBlockData(IsLocked, input, unit);


    public static float GetSpeed(float input, Unit unit) =>
        unit switch
        {
            Unit.Iterations => input * 360,
            Unit.Degrees => input,
            _ => throw new Exception("Cannot calculate rotation speed if given unit is Unit.Time")
        };
}