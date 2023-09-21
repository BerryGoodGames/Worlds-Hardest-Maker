using System;
using DG.Tweening;
using UnityEngine;

[Serializable]
public abstract class AnchorBlockData
{
    public bool IsLocked;

    protected AnchorBlockData(bool isLocked) => IsLocked = isLocked;

    public abstract AnchorBlock GetBlock(AnchorController anchor);
}

[Serializable]
public class MoveBlockData : AnchorBlockData
{
    private readonly float[] target;

    public MoveBlockData(bool isLocked, Vector2 target) : base(isLocked) => this.target = new[] { target.x, target.y };

    public override AnchorBlock GetBlock(AnchorController anchor) =>
        new MoveBlock(anchor, IsLocked, new(target[0], target[1]));
}

[Serializable]
public class TeleportBlockData : AnchorBlockData
{
    private readonly float[] target;

    public TeleportBlockData(bool isLocked, Vector2 target) : base(isLocked) =>
        this.target = new[] { target.x, target.y };

    public override AnchorBlock GetBlock(AnchorController anchor) =>
        new MoveBlock(anchor, IsLocked, new(target[0], target[1]));
}

[Serializable]
public class LoopBlockData : AnchorBlockData
{
    public LoopBlockData(bool isLocked) : base(isLocked)
    {
    }

    public override AnchorBlock GetBlock(AnchorController anchor) => new LoopBlock(anchor, IsLocked);
}

[Serializable]
public class StartRotatingBlockData : AnchorBlockData
{
    public StartRotatingBlockData(bool isLocked) : base(isLocked)
    {
    }

    public override AnchorBlock GetBlock(AnchorController anchor) => new StartRotatingBlock(anchor, IsLocked);
}

[Serializable]
public class StopRotatingBlockData : AnchorBlockData
{
    public StopRotatingBlockData(bool isLocked) : base(isLocked)
    {
    }

    public override AnchorBlock GetBlock(AnchorController anchor) => new StopRotatingBlock(anchor, IsLocked);
}

[Serializable]
public class RotateBlockData : AnchorBlockData
{
    private readonly float iterations;
    public RotateBlockData(bool isLocked, float iterations) : base(isLocked) => this.iterations = iterations;

    public override AnchorBlock GetBlock(AnchorController anchor) => new RotateBlock(anchor, IsLocked, iterations);
}

[Serializable]
public class MoveAndRotateBlockData : AnchorBlockData
{
    private readonly float[] target;
    private readonly float iterations;
    private readonly bool adaptRotation;

    public MoveAndRotateBlockData(bool isLocked, Vector2 target, float iterations, bool adaptRotation) : base(isLocked)
    {
        this.target = new[] { target.x, target.y };
        this.iterations = iterations;
        this.adaptRotation = adaptRotation;
    }

    public override AnchorBlock GetBlock(AnchorController anchor) =>
        new MoveAndRotateBlock(anchor, IsLocked, new(target[0], target[1]), iterations, adaptRotation);
}

[Serializable]
public class SetRotationBlockData : AnchorBlockData
{
    private readonly float input;
    private readonly int unit;

    public SetRotationBlockData(bool isLocked, float input, SetRotationBlock.Unit unit) : base(isLocked)
    {
        this.input = input;
        this.unit = (int)unit;
    }

    public override AnchorBlock GetBlock(AnchorController anchor) =>
        new SetRotationBlock(anchor, IsLocked, input, (SetRotationBlock.Unit)unit);
}

[Serializable]
public class SetDirectionBlockData : AnchorBlockData
{
    private readonly bool isClockwise;

    public SetDirectionBlockData(bool isLocked, bool isClockwise) : base(isLocked) => this.isClockwise = isClockwise;

    public override AnchorBlock GetBlock(AnchorController anchor) =>
        new SetDirectionBlock(anchor, IsLocked, isClockwise);
}

[Serializable]
public class SetEaseBlockData : AnchorBlockData
{
    private readonly int ease;
    public SetEaseBlockData(bool isLocked, Ease ease) : base(isLocked) => this.ease = (int)ease;

    public override AnchorBlock GetBlock(AnchorController anchor) => new SetEaseBlock(anchor, IsLocked, (Ease)ease);
}

[Serializable]
public class SetSpeedBlockData : AnchorBlockData
{
    private readonly float input;
    private readonly int type;

    public SetSpeedBlockData(bool isLocked, float input, SetSpeedBlock.Unit type) : base(isLocked)
    {
        this.input = input;
        this.type = (int)type;
    }

    public override AnchorBlock GetBlock(AnchorController anchor) =>
        new SetSpeedBlock(anchor, IsLocked, input, (SetSpeedBlock.Unit)type);
}

[Serializable]
public class WaitBlockData : AnchorBlockData
{
    private readonly float input;
    private readonly int unit;

    public WaitBlockData(bool isLocked, float input, WaitBlock.Unit unit) : base(isLocked)
    {
        this.input = input;
        this.unit = (int)unit;
    }

    public override AnchorBlock GetBlock(AnchorController anchor) =>
        new WaitBlock(anchor, IsLocked, input, (WaitBlock.Unit)unit);
}