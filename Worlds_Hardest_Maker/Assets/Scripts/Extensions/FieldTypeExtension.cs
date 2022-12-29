using System.Collections.Generic;
using UnityEngine;

public static class FieldTypeExtension
{
    public static GameObject GetPrefab(this FieldType type)
    {
        // return prefab according to type
        return new[]
        {
            PrefabManager.Instance.wallField,
            PrefabManager.Instance.startField,
            PrefabManager.Instance.goalField,
            PrefabManager.Instance.checkpointField,
            PrefabManager.Instance.oneWayField,
            PrefabManager.Instance.conveyor,
            PrefabManager.Instance.water,
            PrefabManager.Instance.ice,
            PrefabManager.Instance.@void,
            PrefabManager.Instance.grayKeyDoorField,
            PrefabManager.Instance.redKeyDoorField,
            PrefabManager.Instance.greenKeyDoorField,
            PrefabManager.Instance.blueKeyDoorField,
            PrefabManager.Instance.yellowKeyDoorField
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

    public static bool IsField(this GameObject field)
    {
        return field.tag.GetFieldType() != (FieldType)(-1);
    }

    public static bool IsSolidField(this GameObject field)
    {
        return field.IsField() && FieldManager.solidFields.Contains(field.tag.GetFieldType());
    }

    public static bool IsSolidFieldTag(this string tag)
    {
        return FieldManager.solidFields.Contains(tag.GetFieldType());
    }
}