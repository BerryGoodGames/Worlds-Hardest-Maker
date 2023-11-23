using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FieldTypeExtension
{
    private static FieldObject[] GetAllFieldObjects()
    {
        return new[]
        {
            FieldManager.Instance.Wall,
            FieldManager.Instance.Start,
            FieldManager.Instance.Goal,
            FieldManager.Instance.Checkpoint,
            FieldManager.Instance.OneWay,
            FieldManager.Instance.Conveyor,
            FieldManager.Instance.Water,
            FieldManager.Instance.Ice,
            FieldManager.Instance.Void,
            FieldManager.Instance.GrayKeyDoor,
            FieldManager.Instance.RedKeyDoor,
            FieldManager.Instance.GreenKeyDoor,
            FieldManager.Instance.BlueKeyDoor,
            FieldManager.Instance.YellowKeyDoor,
        };
    }
    
    public static FieldObject GetFieldObject(this FieldType type) => GetAllFieldObjects()[(int)type];

    public static FieldObject GetFieldObject(this string tag)
    {
        foreach (FieldObject fieldObject in GetAllFieldObjects())
        {
            if (tag.Equals(fieldObject.Tag)) return fieldObject;
        }
        return null;
    }
    
    public static bool IsSolidFieldTag(this string tag) => tag.GetFieldObject().IsSolid;
}