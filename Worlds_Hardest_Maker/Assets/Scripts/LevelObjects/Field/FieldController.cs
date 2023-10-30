public class FieldController : EntityController
{
    private void Start()
    {
        if (transform.parent != ReferenceManager.Instance.FieldContainer) transform.SetParent(ReferenceManager.Instance.FieldContainer);
    }

    public override Data GetData() => new FieldData(gameObject);
}