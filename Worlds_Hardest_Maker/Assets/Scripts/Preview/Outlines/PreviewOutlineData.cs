// File: Preview/Data/PreviewOutlineData.cs
using System.Collections.Generic;
using UnityEngine;

public struct PreviewOutlineData
{
    public static readonly PreviewOutlineData None = new() { Enabled = false, };

    public bool Enabled { get; init; }
    public Color Color { get; init; }
    public float Weight { get; init; }
    public IReadOnlyList<string> ConnectorTags { get; init; }
}