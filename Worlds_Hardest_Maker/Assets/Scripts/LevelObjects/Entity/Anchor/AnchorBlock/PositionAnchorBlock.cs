using UnityEngine;

public abstract class PositionAnchorBlock : AnchorBlock, IDurationBlock
{
    private AnchorController anchor;
    
    public new PositionAnchorBlockController Controller => (PositionAnchorBlockController)base.Controller;
    
    public Vector2 Target { get; }
    public Vector2 TargetAbsolute => Target + anchor.StartPosition;
    
    protected PositionAnchorBlock(AnchorController anchor, bool isLocked, Vector2 target) : base(isLocked)
    {
        this.anchor = anchor;
        Target = target;
    }

    public virtual bool HasCurrentlyDuration => (Target - anchor.StartPosition).sqrMagnitude > 0;
}