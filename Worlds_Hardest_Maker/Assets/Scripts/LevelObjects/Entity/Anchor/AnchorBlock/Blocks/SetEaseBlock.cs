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
    
    protected override void SetControllerValues(AnchorBlockController c)
    {
        SetEaseBlockController controller = (SetEaseBlockController)c;
        controller.Input.value = GameManager.Instance.GetDropdownValue(SetEaseBlockController.GetOption(ease), controller.Input);
    }
    
    public override AnchorBlockData GetData() => new SetEaseBlockData(IsLocked, ease);
}