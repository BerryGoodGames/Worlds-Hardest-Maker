using JetBrains.Annotations;
using UnityEngine;

public interface IPlayerQueryService
{
    bool IsThere(Vector2 position);
    bool IsThereInSheet(Vector2 position, [CanBeNull] AnchorController sheet);
}