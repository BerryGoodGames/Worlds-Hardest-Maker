using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldController : Controller
{
    private void Start()
    {
        if(transform.parent != ReferenceManager.Instance.FieldContainer)
        {
            transform.SetParent(ReferenceManager.Instance.FieldContainer);
        }
    }

    public override IData GetData() => new FieldData(gameObject);
}
