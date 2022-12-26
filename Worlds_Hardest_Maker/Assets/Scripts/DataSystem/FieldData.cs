using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Field attributes: position, type
/// </summary>
[System.Serializable]
public class FieldData : IData
{
    public int[] Position;
    public string FieldType;
    public int Rotation;

    public FieldData(GameObject field)
    {
        Position = new int[2];
        Position[0] = (int)field.transform.position.x;
        Position[1] = (int)field.transform.position.y;
        Rotation = (int)field.transform.rotation.eulerAngles.z;
        
        FieldType typeEnum = (FieldType)FieldManager.GetFieldType(field);
        FieldType = typeEnum.ToString();
    }

    public override void ImportToLevel()
    {
        ImportToLevel(new(Position[0], Position[1]));
    }

    public override void ImportToLevel(Vector2 pos)
    {
        FieldType type = (FieldType)System.Enum.Parse(typeof(FieldType), FieldType);

        FieldManager.Instance.SetField(pos, type, Rotation);
    }

    public override EditMode GetEditMode()
    {
        return (EditMode)System.Enum.Parse(typeof(EditMode), FieldType);    
    }
}
