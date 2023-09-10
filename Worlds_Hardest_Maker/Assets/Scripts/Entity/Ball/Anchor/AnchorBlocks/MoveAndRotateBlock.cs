using System;
using DG.Tweening;
using UnityEngine;

public class MoveAndRotateBlock : PositionAnchorBlock
{
    public const Type BlockType = Type.MoveAndRotate;
    public override Type ImplementedBlockType => Type.MoveAndRotate;
    protected override GameObject Prefab => PrefabManager.Instance.MoveAndRotateBlockPrefab;

    private readonly float iterations;

    private readonly bool adaptRotation;

    #region Constructors

    public MoveAndRotateBlock(AnchorController anchor, bool isLocked, Vector2 target, float iterations,
        bool adaptRotation) :
        base(anchor, isLocked, target)
    {
        this.iterations = iterations;
        this.adaptRotation = adaptRotation;
    }

    public MoveAndRotateBlock(AnchorController anchor, bool isLocked, float x, float y, float iterations,
        bool adaptRotation) : this(
        anchor, isLocked, new(x, y), iterations, adaptRotation)
    {
    }

    #endregion

    public override void Execute()
    {
        // get move duration
        float moveDuration;
        float dist = Vector2.Distance(Target, Anchor.Rb.position);

        if (Anchor.SpeedUnit is SetSpeedBlock.Unit.Speed)
        {
            float speed = Anchor.TimeInput;

            moveDuration = dist / speed;
        }
        else
            moveDuration = Anchor.TimeInput;

        // get rotate duration
        float rotateDuration;
        if (adaptRotation)
            rotateDuration = moveDuration;
        else if (Anchor.RotationSpeedUnit is SetRotationBlock.Unit.Degrees or SetRotationBlock.Unit.Iterations)
        {
            float currentZ = Anchor.transform.localRotation.eulerAngles.z;
            float targetZ = currentZ + iterations * 360;
            float distance = targetZ - currentZ;

            rotateDuration = distance / Anchor.RotationTimeInput;
        }
        else
            rotateDuration = Anchor.RotationTimeInput;

        Anchor.Rb.DOMove(Target, moveDuration)
            .SetEase(Anchor.Ease)
            .OnComplete(() =>
            {
                if (rotateDuration < moveDuration || Math.Abs(rotateDuration - moveDuration) < 0.001)
                    Anchor.FinishCurrentExecution();
            });

        Anchor.Rb.DORotate(iterations * 360, rotateDuration)
            .SetRelative()
            .SetEase(Anchor.Ease)
            .OnComplete(() =>
            {
                if (rotateDuration > moveDuration)
                    Anchor.FinishCurrentExecution();
            });
    }

    protected override void SetControllerValues(AnchorBlockController c)
    {
        MoveAndRotateBlockController controller = (MoveAndRotateBlockController)c;
        controller.PositionInput.SetPositionValues(Target);
        controller.InputIterations.text = iterations.ToString();
        controller.AdaptRotation.isOn = adaptRotation;
    }

    public override AnchorBlockData GetData() =>
        new MoveAndRotateBlockData(IsLocked, Target, iterations, adaptRotation);
}