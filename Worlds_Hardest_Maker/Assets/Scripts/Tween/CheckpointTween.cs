using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CheckpointTween : MonoBehaviour, IResettable
{
    [SerializeField] private float duration;
    private SpriteRenderer sprite;

    private void ActivateTween()
    {
        sprite.DOKill();
        sprite.DOColor(GetActiveColor(), duration);
    }

    public void DeactivateTween()
    {
        sprite.DOKill();
        sprite.DOColor(GetInactiveColor(), duration);
    }

    private void TriggerTween()
    {
        sprite.DOKill();

        Sequence seq = DOTween.Sequence();
        seq.Append(sprite.DOColor(GetActiveColor(), duration * 0.5f))
            .Append(sprite.DOColor(GetInactiveColor(), duration * 0.5f));
    }

    public void Activate(bool reusable)
    {
        if (reusable) TriggerTween();
        else ActivateTween();
    }

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();

        ((IResettable)this).Subscribe();
    }

    private static Color GetActiveColor()
    {
        List<Color> palette = ColorPaletteManager.GetColorPalette("Start Goal Checkpoint").Colors;
        return LevelSessionEditManager.Instance.Playing && SettingsManager.Instance.OneColorSafeFields ? palette[5] : palette[3];
    }

    private static Color GetInactiveColor()
    {
        List<Color> palette = ColorPaletteManager.GetColorPalette("Start Goal Checkpoint").Colors;
        return LevelSessionEditManager.Instance.Playing && SettingsManager.Instance.OneColorSafeFields ? palette[4] : palette[2];
    }

    public void ResetState() => DeactivateTween();

    private void OnDestroy() => ((IResettable)this).Unsubscribe();
}