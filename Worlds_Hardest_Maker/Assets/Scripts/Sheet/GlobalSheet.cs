using UnityEngine;

public sealed class GlobalSheet : ISheet
{
    public static GlobalSheet Instance = new();

    private GlobalSheet()
    {
    }

    public Transform Container { get; set; }
    public bool IsGlobal => true;

    public bool Equals(ISheet other)
    {
        return other is GlobalSheet;
    }
}