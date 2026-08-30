// File: LevelObjects/Field/Outline/CompositeOutlineConnectivityProvider.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Combines multiple providers: a preview ghost connects if it matches EITHER something already
///     placed in the scene OR a sibling ghost in the same preview batch.
/// </summary>
public class CompositeOutlineConnectivityProvider : IOutlineConnectivityProvider
{
    private readonly IReadOnlyList<IOutlineConnectivityProvider> providers;

    public CompositeOutlineConnectivityProvider(params IOutlineConnectivityProvider[] providers)
    {
        this.providers = providers;
    }

    public bool HasConnector(Vector2 position, Vector2 direction, IReadOnlyList<string> connectorTags, ISheet sheet)
    {
        foreach (IOutlineConnectivityProvider provider in providers)
        {
            if (provider.HasConnector(position, direction, connectorTags, sheet)) return true;
        }

        return false;
    }

    public void NotifyNeighbors(Vector2 position, Vector2 direction, ISheet sheet)
    {
        foreach (IOutlineConnectivityProvider provider in providers) provider.NotifyNeighbors(position, direction, sheet);
    }
}