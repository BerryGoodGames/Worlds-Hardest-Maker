using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class ColorPalette
{
    [FormerlySerializedAs("name")] public string Name;
    [FormerlySerializedAs("colors")] public List<Color> Colors = new();
}