using System;
using UnityEngine;

public class AnchorBallController : EntityController
{
    [HideInInspector] public AnchorController ParentAnchor;
    public bool IsParentAnchorNull { get; private set; }

    public override Data GetData() => new AnchorBallData(StartPosition);

    [HideInInspector] public Vector2 StartPosition;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (ParentAnchor == null) IsParentAnchorNull = true;
        
        StartPosition = transform.localPosition;

        if (LevelSessionManager.Instance.IsEdit)
        {
            EditModeManager.Instance.OnEdit += ResetPosition;
        }
    }

    public void ResetPosition()
    {
        transform.localPosition = StartPosition;
        rb.velocity = Vector2.zero;
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

        // unsubscribe
        if (LevelSessionManager.Instance.IsEdit)
        {
            EditModeManager.Instance.OnEdit -= ResetPosition;
        }
    }
}