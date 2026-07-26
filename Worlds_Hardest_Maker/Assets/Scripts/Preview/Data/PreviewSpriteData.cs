using MyBox;
using UnityEngine;

public struct PreviewSpriteData
{
    public static readonly PreviewSpriteData Delete = new PreviewSpriteData
    {
        IsDelete = true,
        Sprite = null,
        Color = Color.black.WithAlphaSetTo(0.8f),
        Scale = Vector2.one,
    };
    
    public bool IsDelete { get; init; }
    public Sprite Sprite { get; init; }
    public Color Color { get; init; }
    public Vector2 Scale { get; init; }
}