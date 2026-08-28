using System.Collections.Generic;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;
using VContainer;

public class CheckpointController : MonoBehaviour, IResettable
{
    public bool Activated;
    private static bool reusableCheckpoints = true;
    
    [ReadOnly] public bool IsAttached;
    [ReadOnly] [CanBeNull] public ISheet Sheet;
    
    public static bool ReusableCheckpoints
    {
        get => reusableCheckpoints;
        set
        {
            if (value) ResetCheckpoints();
            
            reusableCheckpoints = value;
        }
    }
    
    private static readonly List<CheckpointController> activatedCheckpoints = new();
    
    private CheckpointTween anim;
    
    [Inject] private EventBus eventBus;
    [Inject] private IAudioService audioService;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject player = collision.gameObject;
        if (!player.CompareTag("Player")) return;
        
        // check if player wasn't on checkpoint before
        PlayerController controller = player.GetComponent<PlayerController>();
        
        bool alreadyOnField = controller.IsOnFieldInSheet(EditModeManager.Checkpoint, Sheet);
        
        if ((Activated && !reusableCheckpoints) || alreadyOnField) return;
        
        if (reusableCheckpoints) ResetCheckpoints();
        
        ChainActivate();
        
        controller.ActivateCheckpoint(this);
        
        audioService.Play("ActivateCheckpoint");
    }
    
    public void ChainActivate()
    {
        Activate();
        
        List<FieldController> neighbors = FieldManager.Instance.GetNeighborsInSheet(gameObject, Sheet);
        foreach (FieldController n in neighbors)
        {
            if (!n.TryGetComponent(out CheckpointController checkpoint) || checkpoint.Activated) continue;
            
            checkpoint.ChainActivate();
        }
    }
    
    public void Activate()
    {
        Activated = true;
        
        activatedCheckpoints.Add(this);
        
        anim.Activate(reusableCheckpoints);
    }
    
    public void Deactivate(bool remove = true)
    {
        Activated = false;
        
        if (remove) activatedCheckpoints.Remove(this);
        
        anim.DeactivateTween();
    }
    
    private static void ResetCheckpoints()
    {
        // deactivate every checkpoint
        foreach (CheckpointController controller in activatedCheckpoints)
        {
            if (controller != null) controller.Deactivate(false);
        }
        
        activatedCheckpoints.Clear();
    }
    
    private void Start()
    {
        anim = GetComponent<CheckpointTween>();
        
        IsAttached = TryGetComponent(out AnchorAttachment attachment);
        // TODO refactor: creation of new anchor sheet
        if (IsAttached) Sheet = new AnchorSheet(attachment.Anchor);
        
        ((IResettable)this).Subscribe(eventBus);
    }
    
    public void ResetState() => Activated = false;
    
    private void OnDestroy() => ((IResettable)this).Unsubscribe(eventBus);
}