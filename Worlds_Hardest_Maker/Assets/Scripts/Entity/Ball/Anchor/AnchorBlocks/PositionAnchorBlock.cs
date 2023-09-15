using UnityEngine;

public abstract class PositionAnchorBlock : AnchorBlock, IDurationBlock, ILinePreviewBlock
{
    public new PositionAnchorBlockController Controller => (PositionAnchorBlockController)base.Controller;

    public Vector2 Target;

    protected PositionAnchorBlock(AnchorController anchor, bool isLocked, Vector2 target) : base(anchor, isLocked) =>
        Target = target;

    public virtual bool HasCurrentlyDuration() => (Target - Anchor.GetPosition()).sqrMagnitude > 0;
    public abstract bool IsLinePreviewDashed();
}