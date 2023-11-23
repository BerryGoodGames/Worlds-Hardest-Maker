using UnityEngine;

[CreateAssetMenu(fileName = "NewField", menuName = "ScriptableObjects/Field")]
public class FieldObject : ScriptableObject
{
    public GameObject Prefab;
    public string Tag;
    public EditMode EditMode;
    public FieldType FieldType;
    public bool IsRotatable;
    public bool IsSolid;
    public bool HasOutline;
}