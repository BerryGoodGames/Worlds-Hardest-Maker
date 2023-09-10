using UnityEngine;

public abstract class PositionAnchorBlock : AnchorBlock
{
    public Vector2 Target;

    protected PositionAnchorBlock(AnchorController anchor, bool isLocked, Vector2 target) : base(anchor, isLocked) =>
        Target = target;
}