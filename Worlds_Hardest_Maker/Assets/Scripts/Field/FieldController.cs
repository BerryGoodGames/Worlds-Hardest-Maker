using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldController : MonoBehaviour
{
    private void Start()
    {
        if(transform.parent != GameManager.Instance.FieldContainer.transform)
        {
            transform.SetParent(GameManager.Instance.FieldContainer.transform);
        }
    }
}
