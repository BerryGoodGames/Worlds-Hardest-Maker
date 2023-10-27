using System;
using UnityEngine;

public class AnchorBallController : EntityController
{
    [HideInInspector] public AnchorController ParentAnchor;
    public bool IsParentAnchorNull { get; private set; }

    public override Data GetData() => new AnchorBallData(StartPosition);

    [HideInInspector] public Vector2 StartPosition;

    private void Start()
    {
        if (ParentAnchor == null) IsParentAnchorNull = true;
        
        StartPosition = transform.localPosition;
    }
    
    public override void Delete()
    {
        if (IsParentAnchorNull)
        {
            if (AnchorManager.Instance.SelectedAnchor == null) base.Delete();
        }
        else if (ParentAnchor.Selected) base.Delete();
    }

    private void OnDestroy()
    {
        AnchorBallManager.Instance.AnchorBallList.Remove(this);
        
        if (ParentAnchor != null)
        {
            ParentAnchor.Balls.Remove(transform.parent);
            AnchorBallManager.Instance.AnchorBallListLayers[ParentAnchor].Remove(this);
        }
        else
        {
            AnchorBallManager.Instance.AnchorBallListGlobal.Remove(this);
        }
        
        Destroy(transform.parent.gameObject);
    }
}