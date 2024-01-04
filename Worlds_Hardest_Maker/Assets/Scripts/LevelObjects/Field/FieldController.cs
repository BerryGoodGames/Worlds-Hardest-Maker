using UnityEngine;

public class FieldController : EntityController
{
    [HideInInspector] public FieldMode FieldMode;

    private void Start()
    {
        if (transform.parent != ReferenceManager.Instance.FieldContainer) transform.SetParent(ReferenceManager.Instance.FieldContainer);
    }

    public override EditMode EditMode => FieldMode;
    public override Data GetData() => new FieldData(this);
}