using MyBox;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFieldMode", menuName = "ScriptableObjects/FieldMode")]
public class FieldMode : EditMode
{
    [Separator]
    public bool HasOutline;
    public bool IsRotatable;
    public bool IsSolid;

    protected virtual void Reset()
    {
        Attributes.IsField = true;
        WorldPositionType = WorldPositionType.Matrix;
        IsDraggable = true;
    }
}

