using UnityEngine;

public struct PreviewData
{
    public static readonly PreviewData Default = new PreviewData
    {
        Sprite = null,
        Color = Color.white,
        Scale = Vector2.one,
        ShouldRotate = true,
    };

    public Sprite Sprite { get; init; }
    public Color Color { get; init; }
    public Vector2 Scale { get; init; }
    public bool ShouldRotate { get; init; }
}