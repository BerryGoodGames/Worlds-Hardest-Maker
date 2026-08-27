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
    
    public bool Equals(ISheet other)
    {
        return other is AnchorSheet anchorSheet && anchorSheet.Anchor == Anchor;
    }
}