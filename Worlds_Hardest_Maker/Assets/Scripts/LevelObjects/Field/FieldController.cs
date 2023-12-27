using MyBox;
using UnityEngine.Serialization;

public class FieldController : EntityController
{
    // [DisplayInspector] [InitializationField] [MustBeAssigned] public FieldObject ScriptableObject;
    [DisplayInspector] [InitializationField] [MustBeAssigned] public FieldMode FieldMode;

    private void Start()
    {
        if (transform.parent != ReferenceManager.Instance.FieldContainer) transform.SetParent(ReferenceManager.Instance.FieldContainer);
    }

    public override EditMode EditMode => FieldMode;
    public override Data GetData() => new FieldData(this);
}