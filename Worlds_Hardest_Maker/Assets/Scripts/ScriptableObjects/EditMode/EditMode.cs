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
    public bool IsRotatable;
    public bool ShowFillPreview = true;
    [FormerlySerializedAs("DefaultAvailable")] [OverrideLabel("Can use in default/non anchor attach mode")] public bool DefaultSheetAvailable = true;
    [FormerlySerializedAs("AnchorAvailable")] [OverrideLabel("Can use in anchor attach mode")] public bool AnchorSheetAvailable = true;
    [FormerlySerializedAs("Copyable")] public bool IsCopyable = true;
    
    public override string ToString() => name;
}

[Serializable]
public struct EditModeAttributes
{
    [ReadOnly] public bool IsField;
    [ReadOnly] [ConditionalField(nameof(IsField))] public bool IsKeyDoor;
    [Space] [ReadOnly] public bool IsEntity;
    [ReadOnly] [ConditionalField(nameof(IsEntity))] public bool IsKey;
    [ConditionalField(nameof(IsEntity))] public bool IsAnchorRelated;
}