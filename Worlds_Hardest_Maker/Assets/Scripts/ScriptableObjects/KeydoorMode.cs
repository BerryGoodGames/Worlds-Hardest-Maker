using MyBox;
using UnityEngine;

[CreateAssetMenu(fileName = "NewKeydoorMode", menuName = "ScriptableObjects/KeydoorMode")]
public sealed class KeydoorMode : FieldMode
{
    [Separator]
    public KeyColor KeyColor;

    protected override void Reset()
    {
        base.Reset();
        Attributes.IsKeydoor = true;
        HasOutline = true;
        IsSolid = true;
    }
}