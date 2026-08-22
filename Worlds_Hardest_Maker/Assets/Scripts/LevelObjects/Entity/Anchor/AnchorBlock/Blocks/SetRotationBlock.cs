using System;

public class SetRotationBlock : AnchorBlock, IRotationUnitBlock
{
    private readonly float input;
    private readonly RotationUnit unit;
    
    public SetRotationBlock(bool isLocked, float input, RotationUnit unit) : base(isLocked)
    {
        this.input = input;
        this.unit = unit;
    }

    public override string TypeID => "SetRotation";

    public override void Execute(IAnchorBlockExecutionContext ctx)
    {
        ctx.RotationUnit = unit;
        ctx.RotationInput = input;
        ctx.FinishCurrentExecution();
    }
    
    protected override void SetControllerValues(AnchorBlockController c)
    {
        SetRotationBlockController controller = (SetRotationBlockController)c;
        controller.SpeedInput.text = input.ToString();
        controller.UnitInput.value =
            GameManager.Instance.GetDropdownValue(SetRotationBlockController.GetOption(unit), controller.UnitInput);
    }
    
    public override AnchorBlockData GetData() => new SetRotationBlockData(IsLocked, input, unit);

    public RotationUnit RotationUnit => unit;
    
    public static float GetSpeed(float input, RotationUnit unit)
    {
        return unit switch
        {
            RotationUnit.Iterations => input * 360,
            RotationUnit.Degrees => input,
            _ => throw new Exception("Cannot calculate rotation speed if given unit is Unit.Time"),
        };
    }
}