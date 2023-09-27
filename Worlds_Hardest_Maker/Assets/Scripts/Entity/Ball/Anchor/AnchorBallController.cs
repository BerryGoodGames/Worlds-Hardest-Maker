using System;
using UnityEngine;

public class AnchorBallController : EntityController
{
    [HideInInspector] public AnchorController ParentAnchor;
    public bool ParentAnchorNull { get; private set; }

    public override Data GetData() => new AnchorBallData(transform.localPosition);

    private void Start()
    {
        if (ParentAnchor == null) ParentAnchorNull = true;
    }

    public override void Delete()
    {
        if (ParentAnchorNull)
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