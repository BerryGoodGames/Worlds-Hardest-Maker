using DG.Tweening;
using UnityEngine;

public class SetEaseBlock : AnchorBlock
{
    private readonly Ease ease;
    
    public SetEaseBlock(bool isLocked, Ease ease) : base(isLocked)
    {
        this.ease = ease;
    }

    public override string TypeID => "SetEase";

    public override void Execute(IAnchorBlockExecutionContext ctx)
    {
        ctx.Ease = ease;
        ctx.FinishCurrentExecution();
    }
    
    public override void SetControllerValues(AnchorBlockController c)
    {
        SetEaseBlockController controller = (SetEaseBlockController)c;
        string selectedLabel = SetEaseBlockController.EaseOptions.GetLabel(ease);
        controller.Input.value = GameManager.Instance.GetDropdownValue(selectedLabel, controller.Input);
    }
    
    public override AnchorBlockData GetData() => new SetEaseBlockData(IsLocked, ease);
}