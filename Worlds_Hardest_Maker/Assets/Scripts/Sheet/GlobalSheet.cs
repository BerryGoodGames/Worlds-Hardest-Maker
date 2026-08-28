using UnityEngine;

public sealed class GlobalSheet : ISheet
{
    public static GlobalSheet Instance = new();

    private GlobalSheet()
    {
    }

    public Transform Container { get; set; }
    public bool IsGlobal => true;
    
    public bool Equals(ISheet other) => other is GlobalSheet;
    public override bool Equals(object obj) => obj is ISheet other && Equals(other);
    public override int GetHashCode() => 0;
}