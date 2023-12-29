using System;
using MyBox;
using UnityEngine;

public abstract class EditMode : ScriptableObject
{
    public EditModeAttributes Attributes;
    [Separator]
    public string Tag;
    public string UIString;
    public string KeyboardShortcut;
    public GameObject Prefab;
    public WorldPositionType WorldPositionType = WorldPositionType.Grid;
    public bool IsDraggable;
    public bool ShowFillPreview;
}

[Serializable]
public struct EditModeAttributes
{
    [ReadOnly] public bool IsField;
    [ReadOnly] [ConditionalField(nameof(IsField))] public bool IsKeydoor;
    [Space]
    [ReadOnly] public bool IsEntity;
    [ReadOnly] [ConditionalField(nameof(IsEntity))] public bool IsKey;
    [ConditionalField(nameof(IsEntity))] public bool IsAnchorRelated;
}