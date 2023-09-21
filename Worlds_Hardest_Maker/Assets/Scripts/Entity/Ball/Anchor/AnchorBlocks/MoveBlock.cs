using DG.Tweening;
using UnityEngine;

public class MoveBlock : PositionAnchorBlock, IActiveAnchorBlock
{
    public override Type ImplementedBlockType => Type.Move;
    protected override GameObject Prefab => PrefabManager.Instance.MoveBlockPrefab;

    #region Constructors

    public MoveBlock(AnchorController anchor, bool isLocked, Vector2 target) : base(anchor, isLocked, target)
    {
    }

    #endregion

    public override void Execute()
    {
        float duration;
        float dist = Vector2.Distance(Target, Anchor.transform.position);

        if (Anchor.SpeedUnit is SetSpeedBlock.Unit.Speed)
            duration = dist / Anchor.SpeedInput;
        else
            duration = Anchor.SpeedInput;

        Anchor.DOKill();
        Anchor.transform.DOMove(Target, duration)
            .SetEase(Anchor.Ease)
            .OnComplete(Anchor.FinishCurrentExecution);
    }

    protected override void SetControllerValues(AnchorBlockController c)
    {
        MoveBlockController controller = (MoveBlockController)c;
        controller.PositionInput.SetPositionValues(Target);
    }

    public override AnchorBlockData GetData() => new MoveBlockData(IsLocked, Target);
    public override bool IsLinePreviewDashed() => false;
}