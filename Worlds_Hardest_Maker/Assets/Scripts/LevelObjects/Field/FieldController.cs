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
        if (!other.CompareTag("PlayerCenterCollider")) return;

        OnPlayerEntered();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("PlayerCenterCollider")
            || !PlayerManager.Instance.Player.CurrentFloors.Contains(this)) return;

        OnPlayerExited();
    }

    public void OnPlayerEntered()
    {
        if (FieldMode.IsSolid || !isAttached || !FieldMode.CarryPlayer) return;
        
        PlayerController player = PlayerManager.Instance.Player;
        
        if (!player.CurrentFloors.Contains(this)) player.CurrentFloors.Add(this);
        player.transform.SetParent(transform);
        
        // fade out again
        if (player.IsAttached && LevelSessionEditManager.Instance.Editing && !AnchorAttachManager.Instance.InAttachMode)
        {
            AnchorAttachment attachment = GetComponent<AnchorAttachment>();
            AnchorAttachFade fade = attachment.Anchor.AttachFade;
            fade.FadeOut();
        }
    }

    public void OnPlayerExited()
    {
        PlayerController player = PlayerManager.Instance.Player;
        print("Removing");
        player.CurrentFloors.Remove(this);
        if (player.CurrentFloors.Count == 0) player.transform.SetParent(ReferenceManager.Instance.PlayerContainer);
    }

    private void Start() => isAttached = TryGetComponent(out AnchorAttachment _);

    public override EditMode EditMode => FieldMode;
    public override Data GetData() => new FieldData(this);
}