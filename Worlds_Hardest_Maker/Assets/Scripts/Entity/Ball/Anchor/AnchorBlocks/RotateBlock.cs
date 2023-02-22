using DG.Tweening;

public class RotateBlock : AnchorBlock
{
    public const Type BlockType = Type.ROTATE;
    public override Type ImplementedBlockType => BlockType;

    private readonly float iterations;
    public float? CustomTime = null;
    public float? CustomIterations = null;

    public RotateBlock(AnchorController anchor, float iterations) : base(anchor)
    {
        this.iterations = iterations;
    }

    public override void Execute(bool executeNext = true)
    {
        float duration;

        if (Anchor.ApplyAngularSpeed)
        {
            float currentZ = Anchor.transform.localRotation.eulerAngles.z;
            float targetZ = currentZ + (CustomIterations ?? iterations) * 360;
            float distance = targetZ - currentZ;

            duration = distance / Anchor.AngularSpeed;
        }
        else
        {
            duration = CustomTime ?? Anchor.AngularSpeed;
        }

        Anchor.Rb.DORotate((CustomIterations ?? iterations) * 360, duration)
            .SetRelative()
            .SetEase(Anchor.Ease)
            .OnComplete(() =>
            {
                if (executeNext) Anchor.FinishCurrentExecution();
            });
    }
}