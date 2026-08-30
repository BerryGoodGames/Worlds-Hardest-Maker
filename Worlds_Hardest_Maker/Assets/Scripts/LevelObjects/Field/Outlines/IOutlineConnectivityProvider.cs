// File: LevelObjects/Field/Outline/IOutlineConnectivityProvider.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Answers "is there a connecting neighbor in this direction?" for outline rendering.
///     Real fields, fill previews, and paste previews all implement/compose this differently,
///     but share the same geometry and rendering code.
/// </summary>
public interface IOutlineConnectivityProvider
{
    bool HasConnector(Vector2 position, Vector2 direction, IReadOnlyList<string> connectorTags, ISheet sheet);

    /// <summary>
    ///     Optional hook: called when a neighbor was found, so it can refresh its own outline too.
    ///     Only meaningful for scene-backed providers; no-op by default.
    /// </summary>
    void NotifyNeighbors(Vector2 position, Vector2 direction, ISheet sheet) { }
}