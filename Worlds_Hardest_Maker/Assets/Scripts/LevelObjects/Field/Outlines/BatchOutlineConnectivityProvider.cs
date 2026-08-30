// File: LevelObjects/Field/Outline/BatchOutlineConnectivityProvider.cs
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
///     Resolves outline connectivity purely from an in-memory batch of positions (a fill selection
///     or a paste clipboard) — no colliders involved. This is what lets preview ghosts, which have
///     no scene presence at all, still render correct outlines against their siblings.
/// </summary>
public class BatchOutlineConnectivityProvider : IOutlineConnectivityProvider
{
    private readonly Dictionary<Vector2Int, string> relativePositionToTag;
    private readonly Func<Vector2> originProvider;

    /// <param name="relativePositionToTag">Positions relative to <paramref name="originProvider"/>, mapped to the tag placed there.</param>
    /// <param name="originProvider">
    ///     Returns the current world-space origin the relative positions are measured from.
    ///     Pass null for a batch that never moves (e.g. a fill selection already in world space).
    /// </param>
    public BatchOutlineConnectivityProvider(Dictionary<Vector2Int, string> relativePositionToTag, Func<Vector2> originProvider = null)
    {
        this.relativePositionToTag = relativePositionToTag;
        this.originProvider = originProvider ?? (() => Vector2.zero);
    }

    public bool HasConnector(Vector2 position, Vector2 direction, IReadOnlyList<string> connectorTags, ISheet sheet)
    {
        Vector2Int checkPosition = Vector2Int.RoundToInt(position + direction - originProvider());

        return relativePositionToTag.TryGetValue(checkPosition, out string tag) && connectorTags.Contains(tag);
    }
}