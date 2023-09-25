using System;
using DG.Tweening;
using UnityEngine;

public class MoveAndRotateBlock : PositionAnchorBlock, IActiveAnchorBlock
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

    #endregion

    public override void Execute()
    {
        // get move duration
        float moveDuration;
        float dist = Vector2.Distance(TargetAbsolute, Anchor.transform.position);

        if (Anchor.SpeedUnit is SetSpeedBlock.Unit.Speed)
        {
            float speed = Anchor.SpeedInput;

            moveDuration = dist / speed;
        }
        else
            moveDuration = Anchor.SpeedInput;

        // get rotate duration
        float rotateDuration;
        if (adaptRotation)
            rotateDuration = moveDuration;
        else if (Anchor.RotationSpeedUnit is SetRotationBlock.Unit.Degrees or SetRotationBlock.Unit.Iterations)
        {
            float speed = SetRotationBlock.GetSpeed(Anchor.RotationInput, Anchor.RotationSpeedUnit);

            float currentZ = Anchor.transform.localRotation.eulerAngles.z;
            float targetZ = currentZ + iterations * 360;
            float distance = targetZ - currentZ;

            rotateDuration = distance / speed;
        }
        else
            rotateDuration = Anchor.RotationInput;

        Anchor.transform.DOMove(TargetAbsolute, moveDuration)
            .SetEase(Anchor.Ease)
            .OnComplete(() =>
            {
                if (rotateDuration < moveDuration || Math.Abs(rotateDuration - moveDuration) < 0.001)
                    Anchor.FinishCurrentExecution();
            });

        // negate rotation depending on direction
        int direction = Anchor.IsClockwise ? -1 : 1;

        Anchor.RotationTween.Kill();
        Anchor.RotationTween = Anchor.Rb.DORotate(iterations * 360 * direction, rotateDuration)
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
        controller.PositionInput.SetPositionValues(TargetAbsolute);
        controller.IterationsInput.text = iterations.ToString();
        controller.AdaptRotation.isOn = adaptRotation;
    }

    public override AnchorBlockData GetData() =>
        new MoveAndRotateBlockData(IsLocked, Target, iterations, adaptRotation);
}