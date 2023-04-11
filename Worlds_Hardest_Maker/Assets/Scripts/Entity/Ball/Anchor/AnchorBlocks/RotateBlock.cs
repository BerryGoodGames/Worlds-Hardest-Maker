using DG.Tweening;
using UnityEngine;

public class RotateBlock : AnchorBlock
{
    public const Type BlockType = Type.ROTATE;
    public override Type ImplementedBlockType => BlockType;
    protected override GameObject Prefab => PrefabManager.Instance.RotateBlockPrefab;

    private readonly float iterations;

    public RotateBlock(AnchorController anchor, float iterations) : base(anchor) => this.iterations = iterations;

    public override void Execute()
    {
        float duration;

        if (Anchor.ApplyAngularSpeed)
        {
            float currentZ = Anchor.transform.localRotation.eulerAngles.z;
            float targetZ = currentZ + iterations * 360;
            float distance = targetZ - currentZ;

            duration = distance / Anchor.AngularSpeed;
        }
        else
        {
            duration = Anchor.AngularSpeed;
        }

        Anchor.Rb.DORotate(iterations * 360, duration)
            .SetRelative()
            .SetEase(Anchor.Ease)
            .OnComplete(Anchor.FinishCurrentExecution);
    }

    protected override void SetControllerValues(AnchorBlockController c)
    {
        RotateBlockController controller = (RotateBlockController)c;
        controller.Input.text = iterations.ToString();
    }

    public override AnchorBlockData GetData() => new RotateBlockData(iterations);
}