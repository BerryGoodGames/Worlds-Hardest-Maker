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
        if (FieldMode.IsSolid
            || !FieldMode.CarryPlayer
            || !other.CompareTag("PlayerCenterCollider")
            || (PlayerManager.Instance.Player.CurrentFloor != null && PlayerManager.Instance.Player.CurrentFloor.isAttached)) return;

        PlayerController player = PlayerManager.Instance.Player;
        
        player.CurrentFloor = this;
        player.transform.SetParent(transform);
        
        // fade out again
        if (player.IsAttached && LevelSessionEditManager.Instance.Editing && !AnchorAttachManager.Instance.InAttachMode)
        {
            AnchorAttachment attachment = GetComponent<AnchorAttachment>();
            AnchorAttachFade fade = attachment.Anchor.AttachFade;
            fade.FadeOut();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (FieldMode.IsSolid
            || !FieldMode.CarryPlayer
            || !other.CompareTag("PlayerCenterCollider")
            || PlayerManager.Instance.Player.CurrentFloor != this
            || LevelSessionEditManager.Instance.Editing) return;

        PlayerManager.Instance.Player.CurrentFloor = null;
        PlayerManager.Instance.Player.transform.SetParent(ReferenceManager.Instance.PlayerContainer);
    }

    private void Start() => isAttached = TryGetComponent(out AnchorAttachment _);

    public override EditMode EditMode => FieldMode;
    public override Data GetData() => new FieldData(this);
}