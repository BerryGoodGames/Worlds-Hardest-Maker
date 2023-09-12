using UnityEngine;

public abstract class PositionAnchorBlock : AnchorBlock, IDurationBlock
{
    public Vector2 Target;

    protected PositionAnchorBlock(AnchorController anchor, bool isLocked, Vector2 target) : base(anchor, isLocked) =>
        Target = target;

    public bool HasCurrentlyDuration() => (Target - Anchor.GetPosition()).sqrMagnitude > 0;
}