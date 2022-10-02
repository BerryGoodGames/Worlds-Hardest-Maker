using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FieldTypeExtension
{
    public static GameObject GetPrefab(this FieldManager.FieldType type)
    {
        // return prefab according to type
        return new GameObject[] {
            GameManager.Instance.WallField,
            GameManager.Instance.StartField,
            GameManager.Instance.GoalField,
            GameManager.Instance.CheckpointField,
            GameManager.Instance.OneWayField,
            GameManager.Instance.Water,
            GameManager.Instance.Ice,
            GameManager.Instance.Void,
            GameManager.Instance.GrayKeyDoorField,
            GameManager.Instance.RedKeyDoorField,
            GameManager.Instance.GreenKeyDoorField,
            GameManager.Instance.BlueKeyDoorField,
            GameManager.Instance.YellowKeyDoorField,
        }[(int)type];
    }

    public static FieldManager.FieldType GetFieldType(this string tag)
    {
        List<string> tags = new()
        {
            "WallField",
            "StartField",
            "GoalField",
            "CheckpointField",
            "OneWayField",
            "Water",
            "Ice",
            "Void",
            "KeyDoorField",
            "RedKeyDoorField",
            "GreenKeyDoorField",
            "BlueKeyDoorField",
            "YellowKeyDoorField"
        };
        return (FieldManager.FieldType)tags.IndexOf(tag);
    }

    public static bool IsField(this GameObject field)
    {
        return field.tag.GetFieldType() != (FieldManager.FieldType)(-1);
    }

    public static bool IsSolidField(this GameObject field)
    {
        return field.IsField() && FieldManager.SolidFields.Contains(field.tag.GetFieldType());
    }

    public static bool IsSolidFieldTag(this string tag)
    {
        return FieldManager.SolidFields.Contains(tag.GetFieldType());
    }
}
