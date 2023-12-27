using UnityEngine;

public abstract class EditMode : ScriptableObject
{
    public string Tag;
    public GameObject Prefab;
    public WorldPositionType WorldPositionType;
    public string UIString;
}
