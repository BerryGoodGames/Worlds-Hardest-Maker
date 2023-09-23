using System;
using UnityEngine;

public class AnchorBallController : EntityController
{
    [HideInInspector] public AnchorController ParentAnchor;

    public override Data GetData()
    {
        throw new NotImplementedException();
    }

    public override void Delete()
    {
        if (ParentAnchor.Selected)
        {
            base.Delete();
        }
    }

    private void OnDestroy()
    {
        if(ParentAnchor != null) ParentAnchor.Balls.Remove(transform);
    }
}