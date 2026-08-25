using JetBrains.Annotations;
using UnityEngine;

public interface IPositionQueryService
{
    [CanBeNull] public T QueryPosition<T>(Vector2 position, 
        float radius, 
        LayerMask layer, 
        string tag, 
        [CanBeNull] AnchorController sheet) where T : Component;
    
    [CanBeNull] public T QueryPosition<T>(Vector2 position, 
        float radius, 
        LayerMask layer, 
        [CanBeNull] AnchorController sheet) where T : Component;
}