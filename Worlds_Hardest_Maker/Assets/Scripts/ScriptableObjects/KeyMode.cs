using MyBox;
using UnityEngine;

[CreateAssetMenu(fileName = "NewKeyMode", menuName = "ScriptableObjects/KeyMode")]
public sealed class KeyMode : EntityMode
{
    [Separator]
    public KeyColor KeyColor;

    protected override void Reset()
    {
        base.Reset();
        Attributes.IsKey = true;
    }
}
