using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitBlock : AnchorBlock, IDurationBlock
{
    public enum Unit
    {
        Seconds,
        Minutes,
        Hours,
        Days,
    }
    
    private static readonly Dictionary<Unit, float> factors = new()
    {
        { Unit.Seconds, 1 },
        { Unit.Minutes, 60 },
        { Unit.Hours, 3600 },
        { Unit.Days, 86400 },
    };
    
    private readonly float input;
    
    private readonly Unit unit;
    
    public bool HasCurrentlyDuration => input > 0;
    
    public WaitBlock(bool isLocked, float input, Unit unit) : base(isLocked)
    {
        this.input = input;
        this.unit = unit;
    }

    public override string TypeID => "Wait";
    
    public override void Execute(IAnchorBlockExecutionContext ctx)
    {
        ctx.WaitCoroutine = ctx.CoroutineRunner.StartCoroutine(WaitCoroutine(ctx));
    }

    private IEnumerator WaitCoroutine(IAnchorBlockExecutionContext ctx)
    {
        yield return new WaitForSeconds(input * factors[unit]);
        
        ctx.FinishCurrentExecution();
    }
    
    protected override void SetControllerValues(AnchorBlockController c)
    {
        WaitBlockController controller = (WaitBlockController)c;
        controller.DurationInput.text = input.ToString();
        controller.UnitInput.value =
            GameManager.Instance.GetDropdownValue(WaitBlockController.GetOption(unit), controller.UnitInput);
    }
    
    public override AnchorBlockData GetData() => new WaitBlockData(IsLocked, input, unit);
}