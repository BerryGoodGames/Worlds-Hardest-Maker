using MyBox;
using UnityEngine;

public partial class AnchorManager : IManagerSelectable
{
    private const float DOUBLE_CLICK_THRESHOLD = 0.4f;
    
    [field: SerializeField] [field: ReadOnly] public AnchorController SelectedAnchor { get; private set; }
    
    [HideInInspector] public float LastSelectClick = -1;
    
    public void Select(Vector2 pos)
    {
        AnchorController anchor = ((IManager<AnchorController>)this).Get(pos);
        
        Instance.Select(anchor);
    }
    
    public void Select(AnchorController anchor, bool toggleDeselect = true)
    {
        if (anchor == null) return;
        
        // stop if attaching to other anchor
        if (AnchorAttachManager.Instance.InAttachMode && !anchor.IsAttaching) return;
        
        bool switchedEditMode = false;
        // switch to edit mode to anchor if not already
        if (!LevelSessionEditManager.Instance.CurrentEditMode.Attributes.IsAnchorRelated)
        {
            LevelSessionEditManager.Instance.CurrentEditMode = EditModeManager.Anchor;
            switchedEditMode = true;
        }
        
        if (SelectedAnchor != null)
        {
            UpdateBlockListInSelectedAnchor();
            
            SelectedAnchor.Animator.SetBool(selectedString, false);
            SelectedAnchor.SetLinesActive(false);
        }
        
        // deselect anchor if "selected" again by the user (but only if edit mode before was anchor or currently attaching)
        if (toggleDeselect && SelectedAnchor == anchor && (!switchedEditMode || anchor.IsAttaching))
        {
            DeselectAnchor();
            return;
        }
        
        // continue only if in edit mode
        if (LevelSessionEditManager.Instance.Playing) return;
        
        SelectedAnchor = anchor;
        anchor.Animator.SetBool(selectedString, true);
        anchor.SetLinesActive(true);
        
        // disable "no anchor selected" screen
        ReferenceManager.Instance.AnchorNoAnchorSelectedScreen.SetVisible(false);
        
        AnchorBlockManager.LoadAnchorBlocks(anchor);
        
        ReferenceManager.Instance.MainCameraJumper.SetTarget("Anchor", anchor.gameObject);
        ReferenceManager.Instance.AnchorCameraJumping.CameraJumpToAnchor();
        
        if (!AnchorAttachManager.Instance.InAttachMode)
            PanelManager.Instance.SetPanelHidden(ReferenceManager.Instance.AnchorAttachButtonController, false, false);
        
        // play sfx
        audioService.Play("AnchorBlockButton");
    }
    
    public void DeselectAnchor()
    {
        if (SelectedAnchor == null) return;
        
        if (AnchorAttachManager.Instance.InAttachMode) AnchorAttachManager.Instance.ExitAttachMode();
        
        SelectedAnchor.Animator.SetBool(selectedString, false);
        SelectedAnchor.Animator.SetBool(playingString, LevelSessionEditManager.Instance.Playing);
        SelectedAnchor.SetLinesActive(false);
        SelectedAnchor = null;
        
        AnchorBlockManager.EmptyAnchorChains();
        
        // enable "no anchor selected" screen
        ReferenceManager.Instance.AnchorNoAnchorSelectedScreen.SetVisible(true);
        
        ReferenceManager.Instance.MainCameraJumper.RemoveTarget("Anchor");
        
        EditMode currentEditMode = LevelSessionEditManager.Instance.CurrentEditMode;
        if (!currentEditMode.Attributes.IsAnchorRelated)
        {
            PanelManager.Instance.SetPanelHidden(ReferenceManager.Instance.LevelSettingsPanelController, false, false);
            PanelManager.Instance.SetPanelHidden(ReferenceManager.Instance.TestingOptionsPanelController, false, false);
        }
        
        PanelManager.Instance.SetPanelHidden(ReferenceManager.Instance.AnchorAttachButtonController, true);
        PanelManager.Instance.SetPanelHidden(ReferenceManager.Instance.AnchorAttachExitButtonController, true);
        AnchorAttachManager.Instance.InAttachMode = false;
        
        // play sfx
        audioService.Play("AnchorDeselect");
    }
    
    private void CheckAnchorSelection()
    {
        // select anchor
        if (!Input.GetMouseButtonDown(0) || !KeyBinds.GetKeyBind("Editor_Modify")) return;
        
        AnchorController clickedAnchor = ((IManager<AnchorController>)Instance).Get(MouseManager.Instance.MouseWorldPosGrid);
        
        if (clickedAnchor == null) return;
        
        // check double click
        float currentTime = Time.time;
        float deltaClickTime = Instance.LastSelectClick < 0 ? 0 : currentTime - Instance.LastSelectClick;
        if (deltaClickTime < DOUBLE_CLICK_THRESHOLD && Instance.SelectedAnchor == clickedAnchor && !AnchorAttachManager.Instance.InAttachMode)
        {
            AnchorAttachManager.Instance.EnterAttachMode();
            audioService.Play("ButtonClick");
        }
        else
        {
            Instance.Select(clickedAnchor);
            
            Instance.LastSelectClick = currentTime;
        }
    }
}