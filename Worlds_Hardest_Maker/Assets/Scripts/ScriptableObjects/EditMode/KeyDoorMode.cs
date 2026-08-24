using MyBox;
using UnityEngine;

[CreateAssetMenu(fileName = "NewKeyDoorMode", menuName = "ScriptableObjects/EditMode/KeyDoorMode")]
public sealed class KeyDoorMode : FieldMode
{
    [Separator] public KeyColor KeyColor;
    
    protected override void Reset()
    {
        base.Reset();
        HasOutline = true;
        IsSolid = true;
    }
}