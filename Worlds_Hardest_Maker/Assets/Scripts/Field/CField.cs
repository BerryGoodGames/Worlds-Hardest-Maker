using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CField : MonoBehaviour
{
    private void Start()
    {
        if(transform.parent != MGame.Instance.FieldContainer.transform)
        {
            transform.SetParent(MGame.Instance.FieldContainer.transform);
        }
    }
}
