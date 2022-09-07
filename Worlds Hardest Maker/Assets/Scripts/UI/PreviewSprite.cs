using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class for preview settings for a prefab -> more control over preview
/// attach to prefab the preview is for
/// </summary>
public class PreviewSprite : MonoBehaviour
{
    public Sprite sprite;
    public Color color = Color.white;
    public Vector2 scale = Vector2.one;
    public bool showWhenFilling;
}
