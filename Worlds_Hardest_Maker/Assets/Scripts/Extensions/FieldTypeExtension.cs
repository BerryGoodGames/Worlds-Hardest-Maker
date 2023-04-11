using System.Collections.Generic;
using UnityEngine;

public static class FieldTypeExtension
{
    public static GameObject GetPrefab(this FieldType type)
    {
        // return prefab according to type
        return new[]
        {
            PrefabManager.Instance.WallField,
            PrefabManager.Instance.StartField,
            PrefabManager.Instance.GoalField,
            PrefabManager.Instance.CheckpointField,
            PrefabManager.Instance.OneWayField,
            PrefabManager.Instance.Conveyor,
            PrefabManager.Instance.Water,
            PrefabManager.Instance.Ice,
            PrefabManager.Instance.Void,
            PrefabManager.Instance.GrayKeyDoorField,
            PrefabManager.Instance.RedKeyDoorField,
            PrefabManager.Instance.GreenKeyDoorField,
            PrefabManager.Instance.BlueKeyDoorField,
            PrefabManager.Instance.YellowKeyDoorField
        }[(int)type];
    }

    public static FieldType GetFieldType(this string tag)
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
        return (FieldType)tags.IndexOf(tag);
    }

    public static bool IsField(this GameObject field) => field.tag.GetFieldType() != (FieldType)(-1);

    public static bool IsSolidField(this GameObject field) =>
        field.IsField() && FieldManager.SolidFields.Contains(field.tag.GetFieldType());

    public static bool IsSolidFieldTag(this string tag) => FieldManager.SolidFields.Contains(tag.GetFieldType());
}