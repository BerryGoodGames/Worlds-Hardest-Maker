using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
///     class for preview settings for a prefab -> more control over preview
///     attach to prefab the preview is for
/// </summary>
public class PreviewSprite : MonoBehaviour
{
    [FormerlySerializedAs("sprite")] public Sprite Sprite;
    [FormerlySerializedAs("color")] public Color Color = Color.white;
    [FormerlySerializedAs("scale")] public Vector2 Scale = Vector2.one;

    [FormerlySerializedAs("showWhenSelecting")]
    public bool ShowWhenSelecting;

    [FormerlySerializedAs("rotate")] public bool Rotate;
}