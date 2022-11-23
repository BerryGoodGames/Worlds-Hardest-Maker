using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldController : Controller
{
    private void Start()
    {
        if(transform.parent != GameManager.Instance.FieldContainer.transform)
        {
            transform.SetParent(GameManager.Instance.FieldContainer.transform);
        }
    }

    public override IData GetData()
    {
        return new FieldData(gameObject);
    }
}
