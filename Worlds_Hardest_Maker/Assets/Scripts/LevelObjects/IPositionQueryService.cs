using JetBrains.Annotations;
using UnityEngine;

public interface IPositionQueryService
{
    [CanBeNull]
    public T QueryPosition<T>(Vector2 position,
        float radius,
        LayerMask layer,
        string tag,
        [CanBeNull] AnchorController sheet,
        SheetUtils.SheetCheckingScope scope = SheetUtils.SheetCheckingScope.Self) where T : Component;

    [CanBeNull]
    public T QueryPosition<T>(Vector2 position,
        float radius,
        LayerMask layer,
        [CanBeNull] AnchorController sheet,
        SheetUtils.SheetCheckingScope scope = SheetUtils.SheetCheckingScope.Self) where T : Component;

    [CanBeNull]
    public T QueryPositionAny<T>(Vector2 position,
        float radius,
        LayerMask layer) where T : Component;
}