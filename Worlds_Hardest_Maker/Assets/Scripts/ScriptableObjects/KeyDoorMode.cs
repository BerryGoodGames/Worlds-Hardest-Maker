using MyBox;
using UnityEngine;

[CreateAssetMenu(fileName = "NewKeyDoorMode", menuName = "ScriptableObjects/KeyDoorMode")]
public sealed class KeyDoorMode : FieldMode
{
    [Separator] public KeyColor KeyColor;

    protected override void Reset()
    {
        base.Reset();
        Attributes.IsKeyDoor = true;
        HasOutline = true;
        IsSolid = true;
    }
}