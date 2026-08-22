using UnityEngine;

public class SetSpeedBlock : AnchorBlock
{
    public enum Unit
    {
        UnitsPerSecond,
        SecondsToFinish,
    }
    
    private readonly float input;
    private readonly Unit unit;
    
    public SetSpeedBlock(bool isLocked, float input, Unit unit) : base(isLocked)
    {
        this.input = input;
        this.unit = unit;
    }

    public override string TypeID => "SetSpeed";

    public override void Execute(IAnchorBlockExecutionContext ctx)
    {
        ctx.SpeedUnit = unit;
        ctx.SpeedInput = input;
        ctx.FinishCurrentExecution();
    }
    
    protected override void SetControllerValues(AnchorBlockController c)
    {
        SetSpeedBlockController controller = (SetSpeedBlockController)c;
        
        controller.SpeedInput.text = input.ToString();
        controller.UnitInput.value =
            GameManager.Instance.GetDropdownValue(SetSpeedBlockController.GetOption(unit), controller.UnitInput);
    }
    
    public void Print() => Debug.Log((input, unit));
    
    public override AnchorBlockData GetData() => new SetSpeedBlockData(IsLocked, input, unit);
}