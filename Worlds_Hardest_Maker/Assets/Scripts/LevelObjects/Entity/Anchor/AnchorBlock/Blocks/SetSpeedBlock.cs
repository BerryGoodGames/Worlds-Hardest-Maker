using UnityEngine;

public class SetSpeedBlock : AnchorBlock
{
    private readonly float input;
    private readonly MovementUnit unit;
    
    public SetSpeedBlock(bool isLocked, float input, MovementUnit unit) : base(isLocked)
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
    
    public override void SetControllerValues(AnchorBlockController c)
    {
        SetSpeedBlockController controller = (SetSpeedBlockController)c;
        
        controller.SpeedInput.text = input.ToString();
        string selectedLabel = SetSpeedBlockController.UnitOptions.GetLabel(unit);
        controller.UnitInput.value = GameManager.Instance.GetDropdownValue(selectedLabel, controller.UnitInput);
    }
    
    public void Print() => Debug.Log((input, unit));
    
    public override AnchorBlockData GetData() => new SetSpeedBlockData(IsLocked, input, unit);
}