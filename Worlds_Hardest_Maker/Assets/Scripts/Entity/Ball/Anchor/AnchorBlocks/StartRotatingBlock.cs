using DG.Tweening;
using UnityEngine;

public class StartRotatingBlock : AnchorBlock, IActiveAnchorBlock
{
    public StartRotatingBlock(AnchorController anchor, bool isLocked) : base(anchor, isLocked)
    {
    }

    public const Type BlockType = Type.StartRotating;
    public override Type ImplementedBlockType => BlockType;
    protected override GameObject Prefab => PrefabManager.Instance.StartRotatingBlockPrefab;

    public override void Execute()
    {
        if (Anchor.RotationTween is { hasLoops: true })
        {
            Anchor.FinishCurrentExecution();
            return;
        }

        float duration;
        if (Anchor.RotationSpeedUnit is SetRotationBlock.Unit.Degrees or SetRotationBlock.Unit.Iterations)
            duration = 360 / Anchor.RotationTimeInput;
        else
            duration = Anchor.RotationTimeInput;

        if (Anchor.RotationSpeedUnit is SetRotationBlock.Unit.Degrees or SetRotationBlock.Unit.Iterations)
        {
            // negate rotation depending on direction
            int direction = Anchor.IsClockwise ? -1 : 1;

            Anchor.RotationTween.Kill();
            Anchor.RotationTween = Anchor.Rb.DORotate(360 * direction, duration)
                .SetRelative()
                .SetLoops(-1)
                .SetEase(Ease.Linear);
        }

        Anchor.FinishCurrentExecution();
    }

    protected override void SetControllerValues(AnchorBlockController c)
    {
    }

    public override AnchorBlockData GetData() => new StartRotatingBlockData(IsLocked);
}