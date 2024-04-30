using MyBox;
using UnityEngine;

public partial class AnchorManager : IManagerSelectable
{
    [field: SerializeField] [field: ReadOnly] public AnchorController SelectedAnchor { get; private set; }

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

        // SelectedAnchor.AttachFade.FadeOut();

        // disable "no anchor selected" screen
        ReferenceManager.Instance.AnchorNoAnchorSelectedScreen.SetVisible(false);

        AnchorBlockManager.LoadAnchorBlocks(anchor);

        ReferenceManager.Instance.MainCameraJumper.SetTarget("Anchor", anchor.gameObject);
        ReferenceManager.Instance.AnchorCameraJumping.CameraJumpToAnchor();

        if (!AnchorAttachManager.Instance.InAttachMode)
            PanelManager.Instance.SetPanelHidden(ReferenceManager.Instance.AnchorAttachButtonController, false, false);

        // play sfx
        AudioManager.Instance.Play("AnchorBlockButton");
    }

    public void DeselectAnchor()
    {
        if (SelectedAnchor == null) return;

        // SelectedAnchor.AttachFade.FadeIn();
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
            PanelManager.Instance.SetPanelHidden(ReferenceManager.Instance.LevelSettingsPanelController, false);

        PanelManager.Instance.SetPanelHidden(ReferenceManager.Instance.AnchorAttachButtonController, true);
        PanelManager.Instance.SetPanelHidden(ReferenceManager.Instance.AnchorAttachExitButtonController, true);
        AnchorAttachManager.Instance.InAttachMode = false;

        // play sfx
        AudioManager.Instance.Play("AnchorDeselect");
    }

    private static void CheckAnchorSelection()
    {
        // select anchor
        if (!Input.GetMouseButtonDown(0) || !KeyBinds.GetKeyBind("Editor_Modify")) return;

        Instance.Select(MouseManager.Instance.MouseWorldPosGrid);
        BallManager.Instance.Select(MouseManager.Instance.MouseWorldPosGrid);
    }
}