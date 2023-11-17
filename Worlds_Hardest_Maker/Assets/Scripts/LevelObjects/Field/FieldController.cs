using System;
using MyBox;
using UnityEngine;

public class FieldController : EntityController
{
    [MustBeAssigned][InitializationField][SerializeField] private EditMode editMode;

    public override EditMode EditMode => editMode;

    private void Start()
    {
        if (transform.parent != ReferenceManager.Instance.FieldContainer) transform.SetParent(ReferenceManager.Instance.FieldContainer);
    }

    public override Data GetData() => new FieldData(gameObject);
}