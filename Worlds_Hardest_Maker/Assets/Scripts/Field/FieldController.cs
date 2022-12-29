public class FieldController : Controller
{
    private void Start()
    {
        if (transform.parent != ReferenceManager.Instance.FieldContainer)
        {
            transform.SetParent(ReferenceManager.Instance.FieldContainer);
        }
    }

    public override IData GetData()
    {
        return new FieldData(gameObject);
    }
}