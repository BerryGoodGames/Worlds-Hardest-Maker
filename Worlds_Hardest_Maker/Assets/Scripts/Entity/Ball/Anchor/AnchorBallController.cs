using System;
using UnityEngine;

public class AnchorBallController : EntityController
{
    [HideInInspector] public AnchorController ParentAnchor;
    
    public override Data GetData() => new AnchorBallData(transform.localPosition);

    public override void Delete()
    {
        if ((ParentAnchor == null && AnchorManager.Instance.SelectedAnchor == null) || ParentAnchor.Selected) base.Delete();
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