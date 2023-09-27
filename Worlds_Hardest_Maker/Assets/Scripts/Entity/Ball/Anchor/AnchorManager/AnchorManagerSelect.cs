using UnityEngine;

public partial class AnchorManager
{
    public AnchorController SelectedAnchor { get; private set; }

    public void SelectAnchor(Vector2 pos)
    {
        AnchorController anchor = GetAnchor(pos);

        Instance.SelectAnchor(anchor);
    }

    public void SelectAnchor(AnchorController anchor)
    {
        if (anchor == null) return;

        bool switchedEditMode = false;
        // switch to edit mode to anchor if not already on anchor or anchor ball
        if (!EditModeManager.Instance.CurrentEditMode.IsAnchorRelated())
        {
            EditModeManager.Instance.SetEditMode(EditMode.Anchor);
            switchedEditMode = true;
        }

        if (SelectedAnchor != null)
        {
            UpdateBlockListInSelectedAnchor();

            SelectedAnchor.Animator.SetBool(selected, false);
            SelectedAnchor.SetLinesActive(false);
        }

        // deselect anchor if "selected" again by the user (but only if edit mode before was anchor or anchor ball, not sth else)
        if (SelectedAnchor == anchor && !switchedEditMode)
        {
            DeselectAnchor();
            return;
        }

        // continue only if in edit mode
        if (EditModeManager.Instance.Playing) return;

        SelectedAnchor = anchor;
        anchor.Animator.SetBool(selected, true);
        anchor.SetLinesActive(true);

        ReferenceManager.Instance.AnchorBallContainer.BallFadeOut();

        // disable "no anchor selected" screen
        ReferenceManager.Instance.AnchorNoAnchorSelectedScreen.SetVisible(false);

        AnchorBlockManager.LoadAnchorBlocks(anchor);

        ReferenceManager.Instance.MainCameraJumper.AddTarget("Anchor", anchor.gameObject);
        ReferenceManager.Instance.AnchorCameraJumping.CameraJumpToAnchor();
    }

    public void DeselectAnchor()
    {
        if (SelectedAnchor == null) return;

        ReferenceManager.Instance.AnchorBallContainer.BallFadeIn();

        SelectedAnchor.Animator.SetBool(selected, false);
        SelectedAnchor.Animator.SetBool(playing, EditModeManager.Instance.Playing);
        SelectedAnchor.SetLinesActive(false);
        SelectedAnchor = null;

        AnchorBlockManager.EmptyAnchorChains();

        // enable "no anchor selected" screen
        ReferenceManager.Instance.AnchorNoAnchorSelectedScreen.SetVisible(true);

        ReferenceManager.Instance.MainCameraJumper.RemoveTarget("Anchor");

        if (EditModeManager.Instance.CurrentEditMode is not (EditMode.Anchor or EditMode.AnchorBall))
        {
            PanelController levelSettingsPanel = ReferenceManager.Instance.LevelSettingsPanelController;
            PanelManager.Instance.SetPanelOpen(levelSettingsPanel, true);
        }
    }
}