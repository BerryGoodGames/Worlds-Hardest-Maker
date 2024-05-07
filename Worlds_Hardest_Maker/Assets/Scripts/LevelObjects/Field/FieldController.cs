using System.Collections;
using UnityEngine;

public class FieldController : LevelObjectController
{
    [HideInInspector] public FieldMode FieldMode;
    
    [HideInInspector] public Vector2 InitialPosition;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("PlayerCenterCollider")) return;
        
        OnPlayerEntered();
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("PlayerCenterCollider")
            || !PlayerManager.Instance.Player.CurrentPlatforms.Contains(this)) return;
        
        OnPlayerExited();
    }
    
    public void OnPlayerEntered()
    {
        if (FieldMode.IsSolid || !IsAttached || !FieldMode.CarryPlayer) return;
        
        PlayerController player = PlayerManager.Instance.Player;
        
        if (!player.CurrentPlatforms.Contains(this)) player.CurrentPlatforms.Add(this);
        
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
        
        if (player == null) return;
        
        player.CurrentPlatforms.Remove(this);
        if (player.CurrentPlatforms.Count == 0 && gameObject.activeInHierarchy) StartCoroutine(SetParentPlayer());
        
        return;
        
        IEnumerator SetParentPlayer()
        {
            yield return new WaitForEndOfFrame();
            
            player.transform.SetParent(ReferenceManager.Instance.PlayerContainer);
        }
    }
    
    private void Start()
    {
        IsAttached = TryGetComponent(out AnchorAttachment _);
        InitialPosition = transform.position;
    }
    
    public override EditMode EditMode => FieldMode;
    public override Data GetData() => new FieldData(this);
}