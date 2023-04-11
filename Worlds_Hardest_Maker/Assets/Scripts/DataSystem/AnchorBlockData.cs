using System;
using DG.Tweening;
using UnityEngine;

[Serializable]
public abstract class AnchorBlockData
{
    public abstract AnchorBlock GetBlock(AnchorController anchor);
}

[Serializable]
public class MoveBlockData : AnchorBlockData
{
    private readonly float[] target;

    public MoveBlockData(Vector2 target)
    {
        this.target = new[] { target.x, target.y };
    }

    public override AnchorBlock GetBlock(AnchorController anchor) => new MoveBlock(anchor, target[0], target[1]);
}

[Serializable]
public class GoToBlockData : AnchorBlockData
{
    private readonly int index;
    public GoToBlockData(int index) => this.index = index;

    public override AnchorBlock GetBlock(AnchorController anchor) => new GoToBlock(anchor, index);
}

[Serializable]
public class StartRotatingBlockData : AnchorBlockData
{
    public override AnchorBlock GetBlock(AnchorController anchor) => new StartRotatingBlock(anchor);
}

[Serializable]
public class StopRotatingBlockData : AnchorBlockData
{
    public override AnchorBlock GetBlock(AnchorController anchor) => new StopRotatingBlock(anchor);
}

[Serializable]
public class RotateBlockData : AnchorBlockData
{
    private readonly float iterations;
    public RotateBlockData(float iterations) => this.iterations = iterations;

    public override AnchorBlock GetBlock(AnchorController anchor) => new RotateBlock(anchor, iterations);
}

[Serializable]
public class MoveAndRotateBlockData : AnchorBlockData
{
    private readonly float[] target;
    private readonly float iterations;
    private readonly bool adaptRotation;

    public MoveAndRotateBlockData(Vector2 target, float iterations, bool adaptRotation)
    {
        this.target = new[] { target.x, target.y };
        this.iterations = iterations;
        this.adaptRotation = adaptRotation;
    }

    public override AnchorBlock GetBlock(AnchorController anchor) =>
        new MoveAndRotateBlock(anchor, target[0], target[1], iterations, adaptRotation);
}

[Serializable]
public class SetAngularSpeedBlockData : AnchorBlockData
{
    private readonly float speed;
    private readonly int type;

    public SetAngularSpeedBlockData(float speed, SetAngularSpeedBlock.Unit type)
    {
        this.speed = speed;
        this.type = (int)type;
    }

    public override AnchorBlock GetBlock(AnchorController anchor) =>
        new SetAngularSpeedBlock(anchor, speed, (SetAngularSpeedBlock.Unit)type);
}

[Serializable]
public class SetEaseBlockData : AnchorBlockData
{
    private readonly int ease;
    public SetEaseBlockData(Ease ease) => this.ease = (int)ease;

    public override AnchorBlock GetBlock(AnchorController anchor) => new SetEaseBlock(anchor, (Ease)ease);
}

[Serializable]
public class SetSpeedBlockData : AnchorBlockData
{
    private readonly float input;
    private readonly int type;

    public SetSpeedBlockData(float input, SetSpeedBlock.Unit type)
    {
        this.input = input;
        this.type = (int)type;
    }

    public override AnchorBlock GetBlock(AnchorController anchor) =>
        new SetSpeedBlock(anchor, input, (SetSpeedBlock.Unit)type);
}

[Serializable]
public class WaitBlockData : AnchorBlockData
{
    private readonly float input;
    private readonly int unit;

    public WaitBlockData(float input, WaitBlock.Unit unit)
    {
        this.input = input;
        this.unit = (int)unit;
    }

    public override AnchorBlock GetBlock(AnchorController anchor) => new WaitBlock(anchor, input, (WaitBlock.Unit)unit);
}