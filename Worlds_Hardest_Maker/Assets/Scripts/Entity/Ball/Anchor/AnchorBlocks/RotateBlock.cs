using DG.Tweening;
using UnityEngine;

public class RotateBlock : AnchorBlock, IActiveAnchorBlock, IDurationBlock
{
    public const Type BlockType = Type.Rotate;
    public override Type ImplementedBlockType => BlockType;
    protected override GameObject Prefab => PrefabManager.Instance.RotateBlockPrefab;

    private readonly float iterations;

    public RotateBlock(AnchorController anchor, bool isLocked, float iterations) : base(anchor, isLocked) =>
        this.iterations = iterations;

    public override void Execute()
    {
        float duration;

        if (Anchor.RotationSpeedUnit is SetRotationBlock.Unit.Degrees or SetRotationBlock.Unit.Iterations)
        {
            float currentZ = Anchor.transform.localRotation.eulerAngles.z;
            float targetZ = currentZ + iterations * 360;
            float distance = targetZ - currentZ;

            duration = distance / Anchor.RotationTimeInput;
        }
        else
            duration = Anchor.RotationTimeInput;

        // negate rotation depending on direction
        int direction = Anchor.IsClockwise ? -1 : 1;

        Anchor.RotationTween.Kill();
        Anchor.RotationTween = Anchor.Rb.DORotate(iterations * 360 * direction, duration)
            .SetRelative()
            .SetEase(Anchor.Ease)
            .OnComplete(Anchor.FinishCurrentExecution);
    }

    protected override void SetControllerValues(AnchorBlockController c)
    {
        RotateBlockController controller = (RotateBlockController)c;
        controller.Input.text = iterations.ToString();
    }

    public bool HasCurrentlyDuration() => iterations > 0;

    public override AnchorBlockData GetData() => new RotateBlockData(IsLocked, iterations);
}