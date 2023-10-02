using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FieldTypeExtension
{
    public static bool IsRotatable(this FieldType fieldType)
    {
        List<FieldType> rotatableFields = new(new[]
        {
            FieldType.OneWayField,
            FieldType.Conveyor
        });

        return rotatableFields.Contains(fieldType);
    }

    public static bool IsSolid(this FieldType fieldType) => FieldManager.SolidFields.Contains(fieldType);

    public static GameObject GetPrefab(this FieldType type) =>
        // return prefab according to type
        new[]
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
        field.IsField() && field.tag.GetFieldType().IsSolid();

    public static bool IsSolidFieldTag(this string tag) => tag.GetFieldType().IsSolid();
}