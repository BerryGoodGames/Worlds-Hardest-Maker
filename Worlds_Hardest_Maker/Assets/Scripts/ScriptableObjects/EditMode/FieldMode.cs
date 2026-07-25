using MyBox;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFieldMode", menuName = "ScriptableObjects/EditMode/FieldMode")]
public class FieldMode : EditMode
{
    [Separator("Field settings")] public bool HasOutline;
    public bool IsSolid;
    [Space] [ConditionalField(nameof(IsSolid), true)] public bool IsStartFieldForPlayer;
    [ConditionalField(nameof(IsSolid), true)] public bool IsSafeForPlayer;
    [ConditionalField(nameof(IsSolid), true)] public bool CarryPlayer = true;
    
    protected virtual void Reset()
    {
        Attributes.IsField = true;
        WorldPositionType = WorldPositionType.Matrix;
        IsDraggable = true;
    }
}