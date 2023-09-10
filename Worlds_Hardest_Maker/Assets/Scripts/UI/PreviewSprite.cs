using UnityEngine;

/// <summary>
///     Preview settings for a prefab -> more control over preview
///     <para>Attach to prefab the preview is for</para>
/// </summary>
public class PreviewSprite : MonoBehaviour
{
    public Sprite Sprite;
    public Color Color = Color.white;
    public Vector2 Scale = Vector2.one;

    public bool Rotate;
}