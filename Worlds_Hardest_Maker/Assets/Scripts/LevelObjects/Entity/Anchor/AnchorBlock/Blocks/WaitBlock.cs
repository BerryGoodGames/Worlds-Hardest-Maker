using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitBlock : AnchorBlock, IDurationBlock
{
    private static readonly Dictionary<TimeUnit, float> factors = new()
    {
        { TimeUnit.Seconds, 1 },
        { TimeUnit.Minutes, 60 },
        { TimeUnit.Hours, 3600 },
        { TimeUnit.Days, 86400 },
    };
    
    private readonly float input;
    
    private readonly TimeUnit unit;
    
    public bool HasCurrentlyDuration => input > 0;
    
    public WaitBlock(bool isLocked, float input, TimeUnit unit) : base(isLocked)
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
    
    public override void SetControllerValues(AnchorBlockController c)
    {
        WaitBlockController controller = (WaitBlockController)c;
        controller.DurationInput.text = input.ToString();
        string selectedLabel = WaitBlockController.unitOptions.GetLabel(unit);
        controller.UnitInput.value = GameManager.Instance.GetDropdownValue(selectedLabel, controller.UnitInput);
    }
    
    public override AnchorBlockData GetData() => new WaitBlockData(IsLocked, input, unit);
}