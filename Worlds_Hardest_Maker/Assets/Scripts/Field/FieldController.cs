public class FieldController : Controller
{
    private void Start()
    {
        if (transform.parent != ReferenceManager.Instance.fieldContainer)
        {
            transform.SetParent(ReferenceManager.Instance.fieldContainer);
        }
    }

    public override Data GetData()
    {
        return new FieldData(gameObject);
    }
}