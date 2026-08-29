using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using VContainer;

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
    
    private EventBus eventBus;
    [Inject] private ILevelObjectRegistry<BallController> ballRegistry;
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        
        if (LevelSessionManager.Instance.IsEdit)
        {
            eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
        }
    }
    
    private void OnSwitchToEdit(SwitchToEditEvent evt) => ResetPosition();

    protected override void Start()
    {
        ballRegistry.Register(this);
        
        base.Start();
        
        rb = GetComponent<Rigidbody2D>();
        
        if (ParentAnchor == null) IsParentAnchorNull = true;
        
        StartLocalPosition = transform.parent.localPosition;
        StartWorldPosition = transform.parent.position;
    }
    
    public void ResetPosition()
    {
        transform.parent.localPosition = StartLocalPosition;
        rb.linearVelocity = Vector2.zero;
    }
    
    private void OnDestroy()
    {
        ballRegistry.Unregister(this);
        
        if (ParentAnchor != null)
        {
            ParentAnchor.Balls.Remove(transform.parent);
        }
        BallManager.Instance.RemoveFromSheetRegistry(this);
        
        Destroy(transform.parent.gameObject);
        
        // unsubscribe
        if (LevelSessionManager.Instance.IsEdit)
        {
            eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
        }
    }
    
    public override void OnAnchorMove(Vector2 oldPos, Vector2 newPos)
    {
        StartLocalPosition = transform.parent.localPosition;
        StartWorldPosition = transform.parent.position;
    }

    public override bool IsCopyableNow()
    {
        return IsParentAnchorNull && base.IsCopyableNow();
    }
}