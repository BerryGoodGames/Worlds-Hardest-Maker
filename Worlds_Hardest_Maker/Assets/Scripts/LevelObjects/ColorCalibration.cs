using System.Collections.Generic;
using MyBox;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ColorCalibration : MonoBehaviour
{
    [SerializeField] protected bool UseSprites;
    
    [SerializeField] [ConditionalField(nameof(UseSprites))] private Sprite sharingSprite;
    [SerializeField] [ConditionalField(nameof(UseSprites))] private Sprite uniqueSprite;

    [SerializeField] [ConditionalField(nameof(UseSprites), true)] protected bool UseColorPalette;
    
    [SerializeField] [ConditionalField(new[] { nameof(UseSprites), nameof(UseColorPalette), }, new [] { true, false, })] protected string ColorPaletteName;
    [SerializeField] [ConditionalField(new[] { nameof(UseSprites), nameof(UseColorPalette), }, new [] { true, false, })] protected uint SharingColorIndex;
    [SerializeField] [ConditionalField(new[] { nameof(UseSprites), nameof(UseColorPalette), }, new [] { true, false, })] protected uint UniqueColorIndex;
    
    [SerializeField] [ConditionalField(new[] { nameof(UseSprites), nameof(UseColorPalette), }, new [] { true, true, })] private Color sharingColor;
    [SerializeField] [ConditionalField(new[] { nameof(UseSprites), nameof(UseColorPalette), }, new [] { true, true, })] private Color uniqueColor;

    protected SpriteRenderer SpriteRenderer;
    
    public virtual void Apply(bool sharing)
    {
        if (UseSprites)
        {
            SpriteRenderer.sprite = sharing ? sharingSprite : uniqueSprite;
        }
        else
        {
            if (UseColorPalette)
            {        
                List<Color> colors = ColorPaletteManager.GetColorPalette("Start Goal Checkpoint").Colors;

                SpriteRenderer.color = colors[(int)(sharing ? SharingColorIndex : UniqueColorIndex)];
            }
            else
            {
                SpriteRenderer.color = sharing ? sharingColor : uniqueColor;
            }
        }
    }

    protected virtual void Awake() => SpriteRenderer = GetComponent<SpriteRenderer>();

    protected virtual void Reset()
    {
        ColorPaletteName = "Start Goal Checkpoint";
        SharingColorIndex = 4;
    }
}