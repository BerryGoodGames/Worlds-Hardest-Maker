using UnityEngine;

[CreateAssetMenu(fileName = "NewFieldMode", menuName = "ScriptableObjects/FieldMode")]
public class FieldMode : EditMode
{
    public bool HasOutline;
    public bool IsRotatable;
    public bool IsSolid;
}

