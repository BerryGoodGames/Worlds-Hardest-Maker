using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
///     Preview settings for a prefab -> more control over preview
///     <para>Attach to prefab the preview is for</para>
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