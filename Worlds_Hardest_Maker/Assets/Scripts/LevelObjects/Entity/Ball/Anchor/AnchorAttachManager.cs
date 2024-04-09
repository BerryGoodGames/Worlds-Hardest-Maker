using MyBox;
using UnityEngine;

public class AnchorAttachManager : MonoBehaviour
{
    public static AnchorAttachManager Instance { get; private set; }

    [ReadOnly] public bool InAttachMode;
    private static readonly int editingString = Animator.StringToHash("Editing");

    public void EnterAttachMode()
    {
        if (LevelSessionEditManager.Instance.Playing
            || !LevelSessionEditManager.Instance.CurrentEditMode.Attributes.IsAnchorRelated
            || AnchorManager.Instance.SelectedAnchor == null
            || AnchorPositionInputEditManager.Instance.IsEditing) return;

        PanelManager.Instance.SetPanelHidden(ReferenceManager.Instance.AnchorAttachButtonController, true);
        PanelManager.Instance.SetPanelHidden(ReferenceManager.Instance.AnchorAttachExitButtonController, false, false);

        InAttachMode = true;
    }

    public void ExitAttachMode()
    {
        bool isModeAnchorRelated = LevelSessionEditManager.Instance.CurrentEditMode.Attributes.IsAnchorRelated;

        PanelManager.Instance.SetPanelHidden(ReferenceManager.Instance.AnchorAttachButtonController, false, false);
        PanelManager.Instance.SetPanelHidden(ReferenceManager.Instance.AnchorAttachExitButtonController, true);
        if (!isModeAnchorRelated) PanelManager.Instance.SetPanelHidden(ReferenceManager.Instance.LevelSettingsPanelController, false);

        InAttachMode = false;

        foreach (GameObject anchor in GameObject.FindGameObjectsWithTag("Anchor"))
        {
            Animator anim = anchor.GetComponentInChildren<Animator>();
            anim.SetBool(editingString, isModeAnchorRelated);
        }

        if (AnchorManager.Instance.SelectedAnchor)
            AnchorManager.Instance.SelectedAnchor.GetComponent<Animator>().SetBool(editingString, isModeAnchorRelated);

        if (isModeAnchorRelated && AnchorManager.Instance.SelectedAnchor) ReferenceManager.Instance.AnchorBallContainer.BallFadeOut();
        else ReferenceManager.Instance.AnchorBallContainer.BallFadeIn();
    }

    private void Start() =>
        PlayManager.Instance.OnSwitchToPlay += () =>
        {
            if (!InAttachMode) return;

            PanelManager.Instance.SetPanelHidden(ReferenceManager.Instance.AnchorAttachExitButtonController, true);

            InAttachMode = false;
        };

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}