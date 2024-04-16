using UnityEngine;

public class FieldController : LevelObjectController
{
    [HideInInspector] public FieldMode FieldMode;
    
    public Vector2 DeltaPosition { get; private set; }
    private Vector2 previousPosition;

    private void FixedUpdate()
    {
        Vector2 currentPosition = transform.position;
        if (previousPosition == default) previousPosition = transform.position;
        
        DeltaPosition = currentPosition - previousPosition;
        previousPosition = currentPosition;
    }

    public override EditMode EditMode => FieldMode;
    public override Data GetData() => new FieldData(this);
}