using UnityEngine;

public abstract class PositionAnchorBlock : AnchorBlock, IDurationBlock
{
    public new PositionAnchorBlockController Controller => (PositionAnchorBlockController)base.Controller;

    public Vector2 Target { get; }
    public Vector2 TargetAbsolute => Target + Anchor.StartPosition;

    protected PositionAnchorBlock(AnchorController anchor, bool isLocked, Vector2 target) : base(anchor, isLocked) =>
        Target = target;

    public virtual bool HasCurrentlyDuration => (Target - Anchor.StartPosition).sqrMagnitude > 0;
}