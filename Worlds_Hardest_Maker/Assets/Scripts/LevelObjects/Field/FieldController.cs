using System;
using UnityEngine;

public class FieldController : LevelObjectController
{
    [HideInInspector] public FieldMode FieldMode;
    private bool isAttached;
    
    public Vector2 DeltaPosition { get; private set; }
    private Vector2 previousPosition;

    private void FixedUpdate()
    {
        Vector2 currentPosition = transform.position;
        if (previousPosition == default) previousPosition = transform.position;
        
        DeltaPosition = currentPosition - previousPosition;
        previousPosition = currentPosition;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!FieldMode.IsSolid 
            && FieldMode.CarryPlayer
            && other.CompareTag("PlayerCenterCollider") 
            && (PlayerManager.Instance.Player.CurrentFloor == null || !PlayerManager.Instance.Player.CurrentFloor.isAttached))
        {
            PlayerManager.Instance.Player.CurrentFloor = this;
            PlayerManager.Instance.Player.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!FieldMode.IsSolid 
            && FieldMode.CarryPlayer 
            && other.CompareTag("PlayerCenterCollider") 
            && PlayerManager.Instance.Player.CurrentFloor == this)
        {
            PlayerManager.Instance.Player.CurrentFloor = null;
            PlayerManager.Instance.Player.transform.SetParent(ReferenceManager.Instance.PlayerContainer);
        }
    }

    private void Start()
    {
        isAttached = TryGetComponent(out AnchorAttachment _);
    }

    public override EditMode EditMode => FieldMode;
    public override Data GetData() => new FieldData(this);
}