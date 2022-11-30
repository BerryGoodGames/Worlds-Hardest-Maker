using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Field attributes: position, type
/// </summary>
[System.Serializable]
public class FieldData : IData
{
    public int[] position;
    public string fieldType;
    public int rotation;

    public FieldData(GameObject field)
    {
        position = new int[2];
        position[0] = (int)field.transform.position.x;
        position[1] = (int)field.transform.position.y;
        rotation = (int)field.transform.rotation.eulerAngles.z;

        FieldManager.FieldType typeEnum = (FieldManager.FieldType)FieldManager.GetFieldType(field);
        fieldType = typeEnum.ToString();
    }

    public override void ImportToLevel()
    {
        ImportToLevel(new(position[0], position[1]));
    }

    public override void ImportToLevel(Vector2 pos)
    {
        FieldManager.FieldType type = (FieldManager.FieldType)System.Enum.Parse(typeof(FieldManager.FieldType), fieldType);

        FieldManager.Instance.SetField(pos, type, rotation);
    }

    public override GameManager.EditMode GetEditMode()
    {
        return (GameManager.EditMode)System.Enum.Parse(typeof(GameManager.EditMode), fieldType);    
    }
}
