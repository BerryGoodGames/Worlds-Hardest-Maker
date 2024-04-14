using UnityEngine;

public class FieldController : LevelObjectController
{
    [HideInInspector] public FieldMode FieldMode;

    public override EditMode EditMode => FieldMode;
    public override Data GetData() => new FieldData(this);
}