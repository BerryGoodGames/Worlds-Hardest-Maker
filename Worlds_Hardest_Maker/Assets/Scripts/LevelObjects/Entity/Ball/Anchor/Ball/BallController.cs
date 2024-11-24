using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class BallController : EntityController
{
    [Separator] [HideInInspector] public AnchorController ParentAnchor;
    public bool IsParentAnchorNull { get; private set; }
    
    public GameObject LevelObject => transform.parent.gameObject;
    
    public override EditMode EditMode => EditModeManager.Ball;
    
    public override Data GetData() => new BallData(StartWorldPosition);
    
    [HideInInspector] public Vector2 StartLocalPosition;
    [HideInInspector] public Vector2 StartWorldPosition;
    
    private Rigidbody2D rb;
    
    protected override void Start()
    {
        base.Start();
        
        rb = GetComponent<Rigidbody2D>();
        
        if (ParentAnchor == null) IsParentAnchorNull = true;
        
        StartLocalPosition = transform.parent.localPosition;
        StartWorldPosition = transform.parent.position;
        
        if (LevelSessionManager.Instance.IsEdit) PlayManager.Instance.OnSwitchToEdit += ResetPosition;
    }
    
    public void ResetPosition()
    {
        transform.parent.localPosition = StartLocalPosition;
        rb.linearVelocity = Vector2.zero;
    }
    
    private void OnDestroy()
    {
        BallManager.Instance.BallList.Remove(this);
        
        if (ParentAnchor != null)
        {
            ParentAnchor.Balls.Remove(transform.parent);
            
            // remove ball from parent anchor cache list
            ref Dictionary<AnchorController, List<BallController>> ballList = ref BallManager.Instance.BallListSheets;
            if (ballList.ContainsKey(ParentAnchor)) ballList[ParentAnchor].Remove(this);
        }
        else BallManager.Instance.BallListGlobal.Remove(this);
        
        Destroy(transform.parent.gameObject);
        
        // unsubscribe
        if (LevelSessionManager.Instance.IsEdit) PlayManager.Instance.OnSwitchToEdit -= ResetPosition;
    }
    
    public override void OnAnchorMove(Vector2 oldPos, Vector2 newPos)
    {
        StartLocalPosition = transform.parent.localPosition;
        StartWorldPosition = transform.parent.position;
    }
}