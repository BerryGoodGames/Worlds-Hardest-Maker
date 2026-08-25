using JetBrains.Annotations;
using UnityEngine;

public interface IKeyQueryService
{
    bool IsThereInSheet(Vector2 position, [CanBeNull] AnchorController sheet);
}