using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FieldTypeExtension
{
    public static GameObject GetPrefab(this MField.FieldType type)
    {
        // return prefab according to type
        return new GameObject[] {
            MGame.Instance.WallField,
            MGame.Instance.StartField,
            MGame.Instance.GoalField,
            MGame.Instance.CheckpointField,
            MGame.Instance.OneWayField,
            MGame.Instance.Conveyor,
            MGame.Instance.Water,
            MGame.Instance.Ice,
            MGame.Instance.Void,
            MGame.Instance.GrayKeyDoorField,
            MGame.Instance.RedKeyDoorField,
            MGame.Instance.GreenKeyDoorField,
            MGame.Instance.BlueKeyDoorField,
            MGame.Instance.YellowKeyDoorField,
        }[(int)type];
    }

    public static MField.FieldType GetFieldType(this string tag)
    {
        List<string> tags = new()
        {
            "WallField",
            "StartField",
            "GoalField",
            "CheckpointField",
            "OneWayField",
            "Conveyor",
            "Water",
            "Ice",
            "Void",
            "KeyDoorField",
            "RedKeyDoorField",
            "GreenKeyDoorField",
            "BlueKeyDoorField",
            "YellowKeyDoorField"
        };
        return (MField.FieldType)tags.IndexOf(tag);
    }

    public static bool IsField(this GameObject field)
    {
        return field.tag.GetFieldType() != (MField.FieldType)(-1);
    }

    public static bool IsSolidField(this GameObject field)
    {
        return field.IsField() && MField.SolidFields.Contains(field.tag.GetFieldType());
    }

    public static bool IsSolidFieldTag(this string tag)
    {
        return MField.SolidFields.Contains(tag.GetFieldType());
    }
}
