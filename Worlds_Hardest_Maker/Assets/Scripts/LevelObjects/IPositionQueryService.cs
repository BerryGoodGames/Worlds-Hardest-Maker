using JetBrains.Annotations;
using UnityEngine;

public interface IPositionQueryService
{
    public enum ComponentCheckingScope
    {
        Self, Parent, SelfOrParent
    }
    
    [CanBeNull]
    public T QueryPosition<T>(Vector2 position,
        float radius,
        LayerMask layer,
        string tag,
        ISheet sheet,
        SheetUtils.SheetCheckingScope sheetScope = SheetUtils.SheetCheckingScope.Self,
        ComponentCheckingScope componentScope = ComponentCheckingScope.Self) where T : Component;

    [CanBeNull]
    public T QueryPosition<T>(Vector2 position,
        float radius,
        LayerMask layer,
        ISheet sheet,
        SheetUtils.SheetCheckingScope sheetScope = SheetUtils.SheetCheckingScope.Self,
        ComponentCheckingScope componentScope = ComponentCheckingScope.Self) where T : Component;

    [CanBeNull]
    public T QueryPositionAny<T>(Vector2 position,
        float radius,
        LayerMask layer) where T : Component;
}