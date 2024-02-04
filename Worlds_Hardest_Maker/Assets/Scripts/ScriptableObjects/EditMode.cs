using System;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;

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

    public override string ToString() => name;
}

[Serializable]
public struct EditModeAttributes
{
    [ReadOnly] public bool IsField;
    [FormerlySerializedAs("IsKeydoor")] [ReadOnly] [ConditionalField(nameof(IsField))] public bool IsKeyDoor;
    [Space] [ReadOnly] public bool IsEntity;
    [ReadOnly] [ConditionalField(nameof(IsEntity))] public bool IsKey;
    [ConditionalField(nameof(IsEntity))] public bool IsAnchorRelated;
}