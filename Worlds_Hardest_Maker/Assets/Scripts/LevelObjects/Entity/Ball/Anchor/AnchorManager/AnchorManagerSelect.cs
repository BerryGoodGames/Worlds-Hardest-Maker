using UnityEngine;

public partial class AnchorManager
{
    public AnchorController SelectedAnchor { get; private set; }

    public void SelectAnchor(Vector2 pos)
    {
        AnchorController anchor = GetAnchor(pos);

        Instance.SelectAnchor(anchor);
    }

    public void SelectAnchor(AnchorController anchor, bool toggleDeselect = true)
    {
        if (anchor == null) return;

        bool switchedEditMode = false;
        // switch to edit mode to anchor if not already on anchor or anchor ball
        if (!EditModeManagerOther.Instance.CurrentEditMode.Attributes.IsAnchorRelated)
        {
            EditModeManagerOther.Instance.CurrentEditMode = EditModeManager.Anchor;
            switchedEditMode = true;
        }

        if (SelectedAnchor != null)
        {
            UpdateBlockListInSelectedAnchor();

            SelectedAnchor.Animator.SetBool(selectedString, false);
            SelectedAnchor.SetLinesActive(false);
        }

        // deselect anchor if "selected" again by the user (but only if edit mode before was anchor or anchor ball, not sth else)
        if (toggleDeselect && SelectedAnchor == anchor && !switchedEditMode)
        {
            DeselectAnchor();
            return;
        }

        // continue only if in edit mode
        if (EditModeManagerOther.Instance.Playing) return;

        SelectedAnchor = anchor;
        anchor.Animator.SetBool(selectedString, true);
        anchor.SetLinesActive(true);

        ReferenceManager.Instance.AnchorBallContainer.BallFadeOut();

        // disable "no anchor selected" screen
        ReferenceManager.Instance.AnchorNoAnchorSelectedScreen.SetVisible(false);

        AnchorBlockManager.LoadAnchorBlocks(anchor);

        ReferenceManager.Instance.MainCameraJumper.SetTarget("Anchor", anchor.gameObject);
        ReferenceManager.Instance.AnchorCameraJumping.CameraJumpToAnchor();

        // play sfx
        AudioManager.Instance.Play("AnchorBlockButton");
    }

    public void DeselectAnchor()
    {
        if (SelectedAnchor == null) return;

        ReferenceManager.Instance.AnchorBallContainer.BallFadeIn();

        SelectedAnchor.Animator.SetBool(selectedString, false);
        SelectedAnchor.Animator.SetBool(playingString, EditModeManagerOther.Instance.Playing);
        SelectedAnchor.SetLinesActive(false);
        SelectedAnchor = null;

        AnchorBlockManager.EmptyAnchorChains();

        // enable "no anchor selected" screen
        ReferenceManager.Instance.AnchorNoAnchorSelectedScreen.SetVisible(true);

        ReferenceManager.Instance.MainCameraJumper.RemoveTarget("Anchor");

        EditMode currentEditMode = EditModeManagerOther.Instance.CurrentEditMode;
        if (currentEditMode != EditModeManager.Anchor && currentEditMode != EditModeManager.AnchorBall)
        {
            PanelController levelSettingsPanel = ReferenceManager.Instance.LevelSettingsPanelController;
            PanelManager.Instance.SetPanelOpen(levelSettingsPanel, true);
        }

        // play sfx
        AudioManager.Instance.Play("AnchorDeselect");
    }

    private static void CheckAnchorSelection()
    {
        // select anchor
        if (!Input.GetMouseButtonDown(0) || !KeyBinds.GetKeyBind("Editor_Modify")) return;

        Instance.SelectAnchor(MouseManager.Instance.MouseWorldPosGrid);
        AnchorBallManager.SelectAnchorBall(MouseManager.Instance.MouseWorldPosGrid);
    }
}