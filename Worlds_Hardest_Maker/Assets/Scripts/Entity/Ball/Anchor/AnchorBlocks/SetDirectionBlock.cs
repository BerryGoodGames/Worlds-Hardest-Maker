using UnityEngine;

public class SetDirectionBlock : AnchorBlock
{
    private bool isClockwise;

    public SetDirectionBlock(AnchorController anchor, bool isLocked, bool isClockwise) : base(anchor, isLocked) =>
        this.isClockwise = isClockwise;

    public override Type ImplementedBlockType => Type.SetDirection;
    protected override GameObject Prefab => PrefabManager.Instance.SetDirectionBlockPrefab;


    public override void Execute()
    {
        Anchor.IsClockwise = isClockwise;
        Anchor.FinishCurrentExecution();
    }

    protected override void SetControllerValues(AnchorBlockController c)
    {
        SetDirectionBlockController controller = (SetDirectionBlockController)c;
        controller.DirectionInput.IsClockwise = isClockwise;
    }

    public override AnchorBlockData GetData() => new SetDirectionBlockData(IsLocked, isClockwise);
}