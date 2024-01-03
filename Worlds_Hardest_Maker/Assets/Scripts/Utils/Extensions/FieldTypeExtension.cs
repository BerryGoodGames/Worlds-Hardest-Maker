using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FieldTypeExtension
{
    // private static FieldObject[] GetAllFieldObjects()
    // {
    //     return new[]
    //     {
    //         FieldManager.Instance.Wall,
    //         FieldManager.Instance.Start,
    //         FieldManager.Instance.Goal,
    //         FieldManager.Instance.Checkpoint,
    //         FieldManager.Instance.OneWay,
    //         FieldManager.Instance.Conveyor,
    //         FieldManager.Instance.Water,
    //         FieldManager.Instance.Ice,
    //         FieldManager.Instance.Void,
    //         FieldManager.Instance.GrayKeyDoor,
    //         FieldManager.Instance.RedKeyDoor,
    //         FieldManager.Instance.GreenKeyDoor,
    //         FieldManager.Instance.BlueKeyDoor,
    //         FieldManager.Instance.YellowKeyDoor,
    //     };
    // }
    
    // public static FieldObject GetFieldObject(this FieldMode mode) => GetAllFieldObjects()[(int)mode];

    public static FieldMode GetFieldMode(this string tag)
    {
        foreach (FieldMode fieldMode in EditModeManager.Instance.AllFieldModes)
        {
            if (tag.Equals(fieldMode.Tag)) return fieldMode;
        }
        return null;
    }
    
    public static bool IsSolidFieldTag(this string tag) => tag.GetFieldMode().IsSolid;
}