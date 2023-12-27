using UnityEngine;

public abstract class EditMode : ScriptableObject
{
    public string Tag;
    public GameObject Prefab;
    public WorldPositionType WorldPositionType;
    public string UIString;
    public bool IsDraggable;

    public bool IsField => GetType().IsSubclassOf(typeof(FieldMode));
    public bool IsKey => GetType().IsSubclassOf(typeof(KeyMode));

}
