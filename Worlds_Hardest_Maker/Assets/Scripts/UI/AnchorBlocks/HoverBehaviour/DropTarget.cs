using System;

public readonly struct DropTarget : IEquatable<DropTarget>
{
    public enum DropTargetType
    {
        None, InsertAfterBlock, AppendAtEnd
    }
    
    public static readonly DropTarget None = new(DropTargetType.None, -1);
    
    public DropTargetType Type { get; }
    public int BlockIndex { get; }

    private DropTarget(DropTargetType type, int blockIndex)
    {
        Type = type; BlockIndex = blockIndex;
    }
    
    public static DropTarget InsertAfter(int index) => new(DropTargetType.InsertAfterBlock, index);
    public static DropTarget AppendEnd() => new(DropTargetType.AppendAtEnd, -1);
    
    public bool Equals(DropTarget other) => Type == other.Type && BlockIndex == other.BlockIndex;
    public override bool Equals(object obj) => obj is DropTarget other && Equals(other);
    public override int GetHashCode() => HashCode.Combine((int)Type, BlockIndex);
    public static bool operator ==(DropTarget a, DropTarget b) => a.Equals(b);
    public static bool operator !=(DropTarget a, DropTarget b) => !a.Equals(b);
    
    public override string ToString()
    {
        return Type switch
        {
            DropTargetType.InsertAfterBlock => $"InsertAfter({BlockIndex})",
            DropTargetType.AppendAtEnd => "AppendEnd",
            _ => "None",
        };
    }
}