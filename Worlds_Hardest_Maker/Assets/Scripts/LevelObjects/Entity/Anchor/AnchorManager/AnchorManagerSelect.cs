using MyBox;
using UnityEngine;
using VContainer;

public partial class AnchorManager
{
    private const float DOUBLE_CLICK_THRESHOLD = 0.4f;
    
    [field: SerializeField] [field: ReadOnly] public AnchorController SelectedAnchor { get; private set; }
    
    [HideInInspector] public float LastSelectClick = -1;

    [Inject] private IMouseService mouseService;
    
    public void Select(AnchorController anchor, bool toggleDeselect = true)
    {
        if (anchor == null) return;
        
        // stop if attaching to other anchor
        if (AnchorAttachManager.Instance.InAttachMode && !anchor.IsAttaching) return;
        
        if (SelectedAnchor != null)
        {
            UpdateBlockListInSelectedAnchor();
            
            SelectedAnchor.Animator.SetBool(selectedString, false);
            SelectedAnchor.SetLinesActive(false);
        }
        
        // deselect anchor if "selected" again by the user (but only if edit mode before was anchor or currently attaching)
        if (toggleDeselect && SelectedAnchor == anchor)
        {
            DeselectAnchor();
            return;
        }
        
        // continue only if in edit mode
        if (LevelSessionEditManager.Instance.IsPlaying) return;
        
        SelectedAnchor = anchor;
        anchor.Animator.SetBool(selectedString, true);
        anchor.SetLinesActive(true);
        
        // disable "no anchor selected" screen
        anchorNoAnchorSelectedScreen.SetVisible(false);
        
        AnchorBlockManager.Instance.LoadAnchorBlocks(anchor);
        
        mainCameraJumper.SetTarget("Anchor", anchor.gameObject);
        anchorCameraJumping.CameraJumpToAnchor();
        
        // play sfx
        audioService.Play("AnchorBlockButton");
        
        eventBus.Fire(new AnchorSelectedEvent());
    }
    
    public void DeselectAnchor()
    {
        if (SelectedAnchor == null) return;
        
        SelectedAnchor.Animator.SetBool(selectedString, false);
        SelectedAnchor.Animator.SetBool(playingString, LevelSessionEditManager.Instance.IsPlaying);
        SelectedAnchor.SetLinesActive(false);
        SelectedAnchor = null;
        
        // enable "no anchor selected" screen
        anchorNoAnchorSelectedScreen.SetVisible(true);
        
        mainCameraJumper.RemoveTarget("Anchor");
        
        // play sfx
        audioService.Play("AnchorDeselect");
        
        eventBus.Fire(new AnchorDeselectedEvent());
    }
    
    private void CheckAnchorSelection()
    {
        // select anchor
        if (!Input.GetMouseButtonDown(0) || !KeyBinds.GetKeyBind("Editor_Modify")) return;
        
        AnchorController clickedAnchor = anchorQueryService.Find(mouseService.MouseWorldPosGrid, PlaceManager.Instance.GetCurrentSheet());
        
        if (clickedAnchor == null) return;
        
        // check double click
        float currentTime = Time.time;
        float deltaClickTime = LastSelectClick < 0 ? 0 : currentTime - LastSelectClick;
        if (deltaClickTime < DOUBLE_CLICK_THRESHOLD && SelectedAnchor == clickedAnchor && !AnchorAttachManager.Instance.InAttachMode)
        {
            AnchorAttachManager.Instance.EnterAttachMode();
            audioService.Play("ButtonClick");
        }
        else
        {
            Select(clickedAnchor);
            
            LastSelectClick = currentTime;
        }
    }
}