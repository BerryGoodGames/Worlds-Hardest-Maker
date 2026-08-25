using JetBrains.Annotations;
using UnityEngine;

public interface ICoinQueryService
{
    bool IsThereInSheet(Vector2 position, [CanBeNull] AnchorController sheet);
}