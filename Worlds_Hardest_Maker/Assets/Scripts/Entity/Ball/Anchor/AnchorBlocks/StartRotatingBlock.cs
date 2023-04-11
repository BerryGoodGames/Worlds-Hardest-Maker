using System.Windows.Forms;
using DG.Tweening;
using UnityEngine;

public class StartRotatingBlock : AnchorBlock
{
    public StartRotatingBlock(AnchorController anchor) : base(anchor)
    {
    }

    public const Type BlockType = Type.START_ROTATING;
    public override Type ImplementedBlockType => BlockType;
    protected override GameObject Prefab => PrefabManager.Instance.StartRotatingBlockPrefab;
    public override void Execute()
    {
        float duration;
        if (Anchor.ApplyAngularSpeed)
        {
            float currentZ = Anchor.transform.localRotation.eulerAngles.z;
            float targetZ = currentZ + 360;
            float distance = targetZ - currentZ;

            duration = distance / Anchor.AngularSpeed;
        }
        else
        {
            duration = Anchor.AngularSpeed;
        }

        if (Anchor.ApplyAngularSpeed)
        {
            Anchor.InfiniteRotationTween.Kill();
            Anchor.InfiniteRotationTween = Anchor.Rb.DORotate(360, duration).SetRelative().SetLoops(-1)
                .SetEase(Anchor.Ease);
        }

        Anchor.FinishCurrentExecution();
    }

    protected override void SetControllerValues(AnchorBlockController c) {}

    public override AnchorBlockData GetData() => new StartRotatingBlockData();
}