using System.Collections.Generic;
using MyBox;
using UnityEngine;

[RequireComponent(typeof(CheckpointController))]
public class CheckpointColorCalibration : ColorCalibration
{
    [SerializeField] [ConditionalField(new[] { nameof(UseSprites), nameof(UseColorPalette), }, new [] { true, false, })] protected uint SharingColorIndexActivated;
    [SerializeField] [ConditionalField(new[] { nameof(UseSprites), nameof(UseColorPalette), }, new [] { true, false, })] protected uint UniqueColorIndexActivated;
    
    private CheckpointController checkpointController;
    
    public override void Apply(bool sharing)
    {
        List<Color> colors = ColorPaletteManager.GetColorPalette(ColorPaletteName).Colors;

        Color checkpointUnactivated = colors[(int)(sharing ? SharingColorIndex : UniqueColorIndex)];
        Color checkpointActivated = colors[(int)(sharing ? SharingColorIndexActivated : UniqueColorIndexActivated)];

        SpriteRenderer.color = checkpointController.Activated ? checkpointActivated : checkpointUnactivated;
    }

    protected override void Reset()
    {
        base.Reset();
        UseSprites = false;
        UseColorPalette = true;
        UniqueColorIndex = 2;
    }

    protected override void Awake()
    {
        base.Awake();
        checkpointController = GetComponent<CheckpointController>();
    }
}