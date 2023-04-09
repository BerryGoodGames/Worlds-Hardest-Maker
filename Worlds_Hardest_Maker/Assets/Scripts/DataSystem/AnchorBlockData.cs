
using System;
using DG.Tweening;
using UnityEngine;

[Serializable]
public abstract class AnchorBlockData
{
    public abstract AnchorBlock GetBlock(AnchorController anchor);
}

public class MoveBlockData : AnchorBlockData
{
    private readonly float[] target;

    public MoveBlockData(Vector2 target)
    {
        this.target = new[] { target.x, target.y };
    }

    public override AnchorBlock GetBlock(AnchorController anchor)
    {
        return new MoveBlock(anchor, target[0], target[1]);
    }
}

public class GoToBlockData : AnchorBlockData
{
    private readonly int index;
    public GoToBlockData(int index)
    {
        this.index = index;
    }
    public override AnchorBlock GetBlock(AnchorController anchor)
    {
        return new GoToBlock(anchor, index);
    }
}

public class StartRotatingBlockData : AnchorBlockData
{
    public override AnchorBlock GetBlock(AnchorController anchor)
    {
        return new StartRotatingBlock(anchor);
    }
}

public class StopRotatingBlockData : AnchorBlockData
{
    public override AnchorBlock GetBlock(AnchorController anchor)
    {
        return new StopRotatingBlock(anchor);
    }
}

public class RotateBlockData : AnchorBlockData
{
    private readonly float iterations;
    public RotateBlockData(float iterations)
    {
        this.iterations = iterations;
    }
    public override AnchorBlock GetBlock(AnchorController anchor)
    {
        return new RotateBlock(anchor, iterations);
    }
}

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
    public override AnchorBlock GetBlock(AnchorController anchor)
    {
        return new MoveAndRotateBlock(anchor, target[0], target[1], iterations, adaptRotation);
    }
}

public class SetAngularSpeedBlockData : AnchorBlockData
{
    private readonly float speed;
    private readonly int type;
    public SetAngularSpeedBlockData(float speed, SetAngularSpeedBlock.Unit type)
    {
        this.speed = speed;
        this.type = (int)type;
    }
    public override AnchorBlock GetBlock(AnchorController anchor)
    {
        return new SetAngularSpeedBlock(anchor, speed, (SetAngularSpeedBlock.Unit)type);
    }
}

public class SetEaseBlockData : AnchorBlockData
{
    private readonly int ease;
    public SetEaseBlockData(Ease ease)
    {
        this.ease = (int)ease;
    }
    public override AnchorBlock GetBlock(AnchorController anchor)
    {
        return new SetEaseBlock(anchor, (Ease)ease);
    }
}

public class SetSpeedBlockData : AnchorBlockData
{
    private readonly float input;
    private readonly int type;
    public SetSpeedBlockData(float input, SetSpeedBlock.Unit type)
    {
        this.input = input;
        this.type = (int)type;
    }
    public override AnchorBlock GetBlock(AnchorController anchor)
    {
        return new SetSpeedBlock(anchor, input, (SetSpeedBlock.Unit)type);
    }
}

public class WaitBlockData : AnchorBlockData
{
    private readonly float waitTime;
    public WaitBlockData(float waitTime)
    {
        this.waitTime = waitTime;
    }
    public override AnchorBlock GetBlock(AnchorController anchor)
    {
        return new WaitBlock(anchor, waitTime, WaitBlock.Unit.SECONDS);
    }
}