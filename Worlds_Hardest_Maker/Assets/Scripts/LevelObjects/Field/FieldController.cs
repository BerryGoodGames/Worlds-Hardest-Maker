using MyBox;

public class FieldController : EntityController
{
    [DisplayInspector] [InitializationField] [MustBeAssigned] public FieldObject ScriptableObject;

    private void Start()
    {
        if (transform.parent != ReferenceManager.Instance.FieldContainer) transform.SetParent(ReferenceManager.Instance.FieldContainer);
    }

    public override EditMode EditMode => ScriptableObject.EditMode;
    public override Data GetData() => new FieldData(this);
}