using System;
using UnityEngine;

public class AnchorBallController : EntityController
{
    [HideInInspector] public AnchorController ParentAnchor;
    private bool parentAnchorNull;

    public override Data GetData() => new AnchorBallData(transform.localPosition);

    private void Start()
    {
        if (ParentAnchor == null) parentAnchorNull = true;
    }

    public override void Delete()
    {
        if (parentAnchorNull)
        {
            if(AnchorManager.Instance.SelectedAnchor == null) base.Delete();
        }
        else if(ParentAnchor.Selected) base.Delete();
    }

    private void OnDestroy()
    {
        if (ParentAnchor != null)
        {
            ParentAnchor.Balls.Remove(transform);
            AnchorBallManager.Instance.AnchorBallListLayers[ParentAnchor].Remove(this);
        }
        else
        {
            AnchorBallManager.Instance.AnchorBallListGlobal.Remove(this);
        }
    }
}