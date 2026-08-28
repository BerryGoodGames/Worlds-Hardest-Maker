using UnityEngine;

public sealed class AnchorSheet : ISheet
{
    public AnchorController Anchor { get; }
    
    public AnchorSheet(AnchorController anchor)
    {
        Anchor = anchor;
    }

    public Transform Container => Anchor.AttachmentContainer;
    public bool IsGlobal => false;
    
    public bool Equals(ISheet other) => other is AnchorSheet a && a.Anchor == Anchor;
    public override bool Equals(object obj) => obj is ISheet other && Equals(other);
    public override int GetHashCode() => Anchor.GetHashCode();
}