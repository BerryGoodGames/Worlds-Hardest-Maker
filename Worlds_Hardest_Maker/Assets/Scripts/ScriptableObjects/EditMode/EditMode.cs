using MyBox;
using UnityEngine;

public abstract class EditMode : ScriptableObject
{
    [Separator("General settings")] public string Tag;
    public string UIString;
    public string KeyboardShortcut;
    public GameObject Prefab;
    public WorldPositionType WorldPositionType = WorldPositionType.Grid;
    public bool IsDraggable;
    public bool IsRotatable;
    public bool ShowFillPreview = true;
    public bool IsAnchorRelated;
    [OverrideLabel("Can use in default/non anchor attach mode")] public bool DefaultSheetAvailable = true;
    [OverrideLabel("Can use in anchor attach mode")] public bool AnchorSheetAvailable = true;
    public bool IsCopyable = true;
    
    public override string ToString() => name;
}