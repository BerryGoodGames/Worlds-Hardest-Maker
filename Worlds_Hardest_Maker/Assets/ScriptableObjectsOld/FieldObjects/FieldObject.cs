using UnityEngine;

[CreateAssetMenu(fileName = "NewField", menuName = "ScriptableObjectsOld/Field")]
public class FieldObject : ScriptableObject
{
    public GameObject Prefab;
    public string Tag;
    public EditMode EditMode;
    public FieldMode FieldMode;
    public bool IsRotatable;
    public bool IsSolid;
    public bool HasOutline;
}