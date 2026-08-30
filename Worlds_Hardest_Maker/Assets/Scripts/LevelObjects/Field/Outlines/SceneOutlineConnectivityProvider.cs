// File: LevelObjects/Field/Outline/SceneOutlineConnectivityProvider.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
///     Resolves outline connectivity against real, placed level objects via physics queries.
///     This is the only provider real <see cref="FieldOutline"/> instances use.
/// </summary>
public class SceneOutlineConnectivityProvider : IOutlineConnectivityProvider
{
    public bool HasConnector(Vector2 position, Vector2 direction, IReadOnlyList<string> connectorTags, ISheet sheet)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(position, direction, 1);

        foreach (RaycastHit2D hit in hits)
        {
            if (!SheetUtils.Exists(hit.collider, sheet)) continue;
            if (connectorTags.Contains(hit.collider.tag)) return true;
        }

        return false;
    }

    public void NotifyNeighbors(Vector2 position, Vector2 direction, ISheet sheet)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(position, direction, 1);

        foreach (RaycastHit2D hit in hits)
        {
            if (!SheetUtils.Exists(hit.collider, sheet)) continue;
            if (hit.transform.TryGetComponent(out FieldOutline neighborOutline)) neighborOutline.UpdateOutline();
        }
    }
}