using System;
using MyBox;
using UnityEngine;

public abstract class EditMode : ScriptableObject
{
    [OverrideLabel("Class attributes")] public EditModeAttributes Attributes;
    [Separator("General settings")] public string Tag;
    public string UIString;
    public string KeyboardShortcut;
    public GameObject Prefab;
    public WorldPositionType WorldPositionType = WorldPositionType.Grid;
    public bool IsDraggable;
    public bool ShowFillPreview = true;
}

[Serializable]
public struct EditModeAttributes
{
    [ReadOnly] public bool IsField;
    [ReadOnly] [ConditionalField(nameof(IsField))] public bool IsKeydoor;
    [Space] [ReadOnly] public bool IsEntity;
    [ReadOnly] [ConditionalField(nameof(IsEntity))] public bool IsKey;
    [ConditionalField(nameof(IsEntity))] public bool IsAnchorRelated;
}