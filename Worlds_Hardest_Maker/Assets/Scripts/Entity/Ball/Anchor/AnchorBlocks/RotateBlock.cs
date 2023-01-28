using DG.Tweening;

public class RotateBlock : AnchorBlock
{
    public const Type blockType = Type.ROTATE;
    public override Type ImplementedBlockType => blockType;

    private readonly float iterations;
    public float? customTime = null;
    public float? customIterations = null;

    public RotateBlock(AnchorController anchor, float iterations) : base(anchor)
    {
        this.iterations = iterations;
    }

    public override void Execute(bool executeNext = true)
    {
        float duration;

        if (anchor.applyAngularSpeed)
        {
            float currentZ = anchor.transform.localRotation.eulerAngles.z;
            float targetZ = currentZ + (customIterations ?? iterations) * 360;
            float distance = targetZ - currentZ;

            duration = distance / anchor.angularSpeed;
        }
        else
        {
            duration = customTime ?? anchor.angularSpeed;
        }

        anchor.rb.DORotate((customIterations ?? iterations) * 360, duration)
            .SetRelative()
            .SetEase(anchor.ease)
            .OnComplete(() => { if(executeNext) anchor.FinishCurrentExecution(); });
    }
}