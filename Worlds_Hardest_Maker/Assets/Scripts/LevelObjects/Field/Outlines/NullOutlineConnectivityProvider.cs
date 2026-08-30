// File: LevelObjects/Field/Outline/NullOutlineConnectivityProvider.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Always reports "no connector". Used as the default batch provider for single, isolated
///     preview ghosts (e.g. the placement preview following the mouse).
/// </summary>
public sealed class NullOutlineConnectivityProvider : IOutlineConnectivityProvider
{
    public static readonly NullOutlineConnectivityProvider Instance = new();

    private NullOutlineConnectivityProvider() { }

    public bool HasConnector(Vector2 position, Vector2 direction, IReadOnlyList<string> connectorTags, ISheet sheet) => false;
}