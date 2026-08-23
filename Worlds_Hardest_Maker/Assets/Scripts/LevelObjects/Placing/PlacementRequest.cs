using JetBrains.Annotations;
using UnityEngine;

public readonly struct PlacementRequest
{
    public Vector2 Position { get; init; }
    public int Rotation { get; init; }
    public EditMode EditMode { get; init; }
    [CanBeNull] public AnchorController Sheet { get; init; }
}